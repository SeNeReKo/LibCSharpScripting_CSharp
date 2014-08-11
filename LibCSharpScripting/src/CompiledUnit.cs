using System;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CSharp;


namespace LibCSharpScripting.src
{

	/// <summary>
	/// This is the result of a compilation process.
	/// </summary>
	public class CompiledUnit
	{

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		private SourceUnitSpecification specification;
		private object scriptObj;
		private Assembly compiledAssembly;

		private Dictionary<string, CompiledMethodWrapper> methods;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public CompiledUnit(SourceUnitSpecification specification, string scriptFileName, string[] referencedAssemblies, Error[] errors)
		{
			this.methods = new Dictionary<string, CompiledMethodWrapper>();

			this.ReferencedAssemblies = referencedAssemblies;
			this.specification = specification;
			this.ScriptFileName = scriptFileName;
			this.Errors = errors;
		}

		public CompiledUnit(SourceUnitSpecification specification,
			string scriptFileName, string[] referencedAssemblies, Assembly compiledAssembly, Type compiledClass)
		{
			this.methods = new Dictionary<string, CompiledMethodWrapper>();

			this.ReferencedAssemblies = referencedAssemblies;
			this.specification = specification;
			this.ScriptFileName = scriptFileName;
			this.Errors = new Error[0];
			this.compiledAssembly = compiledAssembly;

			ConstructorInfo ci = compiledClass.GetConstructor(Type.EmptyTypes);
			if (ci == null)
				throw new InvalidOperationException(
					"Unable to compile scripts: No empty constructor defined within class based on " + scriptFileName + "!");
			scriptObj = ci.Invoke(new object[0]);

			foreach (MethodSpecification m in specification.MethodSpecs) {
				if (methods.ContainsKey(m.Name)) throw new Exception("Method overloading not supported!");

				MethodInfo methodInfo = compiledClass.GetMethod(m.Name, BindingFlags.Public | BindingFlags.Instance);
				if (methodInfo == null) {
					throw new InvalidOperationException(
						"Unable to compile scripts: Method '" + m.Name + "' not found within class based on " + scriptFileName + "!");
				}

				methods.Add(m.Name, new CompiledMethodWrapper(scriptFileName, scriptObj, compiledClass, m, methodInfo));
			}
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public string[] ReferencedAssemblies
		{
			get;
			private set;
		}

		public string ScriptFileName
		{
			get;
			private set;
		}

		public CompiledMethodWrapper this[string methodName]
		{
			get {
				CompiledMethodWrapper w;
				if (!methods.TryGetValue(methodName, out w)) {
					return null;
				}
				return w;
			}
		}

		public Error[] Errors
		{
			get;
			private set;
		}

		public bool IsSuccess
		{
			get {
				return (Errors == null) || (Errors.Length == 0);
			}
		}

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		/*
		public void ExecuteMain(params KeyValuePair<string, object>[] kvps)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in kvps) {
				dict.Add(kvp.Key, kvp.Value);
			}
			ExecuteMain(dict);
		}

		public void ExecuteMain(IDictionary<string, object> kvps)
		{
			if (MainMethod == null) throw new Exception("No assembly.");
			object[] parameters = new object[MainMethodArguments.Length];
			int i = 0;
			foreach (SrcVariable v in MainMethodArguments) {
				object value;
				if (kvps.TryGetValue(v.Name, out value)) {
					parameters[i] = value;
				}
				i++;
			}
			MainMethod.Invoke(scriptObj, parameters);
		}

		public void ExecuteCompleted(params KeyValuePair<string, object>[] kvps)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in kvps) {
				dict.Add(kvp.Key, kvp.Value);
			}
			ExecuteCompleted(dict);
		}

		public void ExecuteCompleted(IDictionary<string, object> kvps)
		{
			if (MainMethod == null) throw new Exception("No assembly.");
			object[] parameters = new object[CompletedMethodArguments.Length];
			int i = 0;
			foreach (SrcVariable v in CompletedMethodArguments) {
				object value;
				if (kvps.TryGetValue(v.Name, out value)) {
					parameters[i] = value;
				}
				i++;
			}
			CompletedMethod.Invoke(scriptObj, parameters);
		}

		public void ExecuteInitialize(params KeyValuePair<string, object>[] kvps)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in kvps) {
				dict.Add(kvp.Key, kvp.Value);
			}
			ExecuteInitialize(dict);
		}

		public void ExecuteInitialize(IDictionary<string, object> kvps)
		{
			if (MainMethod == null) throw new Exception("No assembly.");
			object[] parameters = new object[InitMethodArguments.Length];
			int i = 0;
			foreach (SrcVariable v in InitMethodArguments) {
				object value;
				if (kvps.TryGetValue(v.Name, out value)) {
					parameters[i] = value;
				}
				i++;
			}
			InitializeMethod.Invoke(scriptObj, parameters);
		}
		*/

	}

}
