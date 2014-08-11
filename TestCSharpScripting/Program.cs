using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using LibCSharpScripting.src;

using TestCSharpScripting.src;


namespace TestCSharpScripting
{

	public class Program
	{

		public static void Main(string[] args)
		{
			SourceUnitCompiler compiler = new SourceUnitCompiler("temp",
				new SourceUnitSpecification(
					new ConstructorSpecification(),
					new MethodSpecification("Test", new SrcVariable("int", "data"))
					)
				);

			compiler.AssemblyManager.RegisterAssembly(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.Drawing.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.Data.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Data.DataSetExtensions.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.Xml.dll");
			compiler.AssemblyManager.RegisterAssembly(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Xml.Linq.dll");

			compiler.CodeGenerators.Add(new SourceCodeGenerator());

			CompiledUnit cu = compiler.Compile("foo", new SourceFile());
			object ret = cu["Test"].Execute(new KeyValuePair<string, object>("data", 5));	

		}

	}

}
