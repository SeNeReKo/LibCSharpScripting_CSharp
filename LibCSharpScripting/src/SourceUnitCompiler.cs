using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CSharp;

using LibCSharpScripting.src.impl;


namespace LibCSharpScripting.src
{

	public class SourceUnitCompiler
	{

		public interface ICodeGenList
		{
			int Count
			{
				get;
			}

			ISourceCodeGenerator this[Type type]
			{
				get;
			}

			ISourceCodeGenerator this[string typeName]
			{
				get;
			}

			void Add(ISourceCodeGenerator generator);

			bool Remove(ISourceCodeGenerator generator);

			bool Remove(string typeName);

			void Clear();
		}

		private class CodeGenList : ICodeGenList
		{

			private Dictionary<string, ISourceCodeGenerator> codeGens;

			public CodeGenList()
			{
				codeGens = new Dictionary<string, ISourceCodeGenerator>();
			}

			public int Count
			{
				get {
					return codeGens.Count;
				}
			}

			public ISourceCodeGenerator this[Type type]
			{
				get {
					string typeName = type.FullName;
					ISourceCodeGenerator g;
					if (codeGens.TryGetValue(typeName, out g)) return g;
					throw new Exception("No source code generator for type: " + typeName);
				}
			}

			public ISourceCodeGenerator this[string typeName]
			{
				get {
					ISourceCodeGenerator g;
					if (codeGens.TryGetValue(typeName, out g)) return g;
					throw new Exception("No source code generator for type: " + typeName);
				}
			}

			public void Add(ISourceCodeGenerator generator)
			{
				string typeName = generator.SourceObjectType.FullName;
				if (codeGens.ContainsKey(typeName))
					throw new Exception("A source code generator is already registered for this type: "
						+ typeName);
				codeGens.Add(typeName, generator);
			}

			public bool Remove(ISourceCodeGenerator generator)
			{
				return Remove(generator.SourceObjectType.FullName);
			}

			public bool Remove(string typeName)
			{
				return codeGens.Remove(typeName);
			}

			public void Clear()
			{
				codeGens.Clear();
			}
		}

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		private static readonly Regex NameSpaceReference = new Regex("^[a-z\\.0-9]*$", RegexOptions.IgnoreCase);

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		private string tempDir;
		private CodeDomProvider csharpCompiler;
		private SourceUnitSpecification sourceSpec;
		private int counter;
		private string tempDir2;
		private CodeGenList codeGenList;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public SourceUnitCompiler(string tempDir, SourceUnitSpecification sourceSpec)
		{
			this.sourceSpec = sourceSpec;
			this.tempDir = tempDir;

			if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

			this.tempDir2 = tempDir;
			if (!this.tempDir2.EndsWith("" + Path.DirectorySeparatorChar))
				this.tempDir2 += Path.DirectorySeparatorChar;

			csharpCompiler = CSharpCodeProvider.CreateProvider("cs");

			codeGenList = new CodeGenList();

			this.AssemblyManager = new AssemblyManager();
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public AssemblyManager AssemblyManager
		{
			get;
			private set;
		}

		public ICodeGenList CodeGenerators
		{
			get {
				return codeGenList;
			}
		}

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		private Error[] __TransformErrors(CompilerErrorCollection errors, SourceCodeData sourceCode)
		{
			List<Error> ret = new List<Error>();

			foreach (CompilerError e in errors) {
				if (e.IsWarning) continue;
				SourceCodeLocation location = sourceCode.GetLocation(e.Line);
				ret.Add(new Error(e.ErrorNumber, e.ErrorText, location));
			}

			return ret.ToArray();
		}

		/// <summary>
		/// Compiles given source file.
		/// </summary>
		/// <param name="scriptFileObject">The source file to be compiled.</param>
		public CompiledUnit Compile(string filePath, object scriptFileObject)
		{
			if (filePath == null) throw new Exception("No file path specified!");
			if (scriptFileObject == null) throw new Exception("No script file object specified!");

			ISourceCodeGenerator gen = codeGenList[scriptFileObject.GetType()];

			counter++;

			// compile the code

			SourceCodeData sourceCode = gen.CreateSourceContent(filePath, scriptFileObject, "ScriptedNameSpace" + counter, "ScriptedClass" + counter);

			// extract all using-information

			HashSet<string> usings = new HashSet<string>();
			foreach (ISourceCodeLine line in sourceCode) {
				string s = line.LineText.Trim();
				if (!s.StartsWith("using")) continue;
				if (!s.EndsWith(";")) continue;
				s = s.Substring("using".Length);
				if (!char.IsWhiteSpace(s[0])) continue;
				s = s.Trim();
				s = s.Substring(0, s.Length - 1);
				if (!NameSpaceReference.IsMatch(s)) continue;
				usings.Add(s);
			}

			// prepare compilation

			CompilerParameters compilerParameters = new CompilerParameters {
				GenerateExecutable = false,
				GenerateInMemory = true,
				// CompilerOptions = "/o+",
				IncludeDebugInformation = true,
				CompilerOptions = " /debug:pdbonly",
				OutputAssembly = tempDir2 + "ScriptedAssembly" + counter + ".dll",
			};

			// add additional assemblies

			List<string> referencedAssemblies = new List<string>();
			foreach (AssemblyManager.NamespaceAssemblyInfo ai in AssemblyManager.GetNamespaceAssemblyInfos(usings)) {
				compilerParameters.ReferencedAssemblies.Add(ai.Path);
				referencedAssemblies.Add(ai.Path);
			}

			/*
			ReferencedAssemblyList referencedAssemblyList = new ReferencedAssemblyList();
			if (gen.ReferencedAssemblies != null) {
				referencedAssemblyList.AddLoadAssemblies(gen.ReferencedAssemblies);
			}
			compilerParameters.ReferencedAssemblies.AddRange(referencedAssemblyList.ToArray());
			*/

			/* does not work
			foreach (string assemblyPath in compilerParameters.ReferencedAssemblies) {
				Assembly.LoadFrom(assemblyPath);
			}			*/

			// check compilation result

			CompilerResults results = csharpCompiler.CompileAssemblyFromSource(compilerParameters, sourceCode.ToString());
			if (results.Errors.Count > 0) {
				return new CompiledUnit(sourceSpec, filePath, referencedAssemblies.ToArray(), __TransformErrors(results.Errors, sourceCode));
			} else
			if (results.CompiledAssembly == null) {
				throw new InvalidOperationException("Unable to compile scripts: No result assembly from compiler!");
			}
 
			// try to get main class

			Assembly compiledAssembly = results.CompiledAssembly;
			Type scriptClass = compiledAssembly.GetType("ScriptedNameSpace" + counter + ".ScriptedClass" + counter);
			if (scriptClass == null) {
				throw new InvalidOperationException("Unable to compile scripts: scripted class not found!");
			}

			return new CompiledUnit(sourceSpec, filePath, referencedAssemblies.ToArray(), compiledAssembly, scriptClass);
		}

	}

}
