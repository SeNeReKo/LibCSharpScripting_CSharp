using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using LibCSharpScripting.src;


namespace TestCSharpScripting.src
{

	public class SourceCodeGenerator : ISourceCodeGenerator
	{

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public SourceCodeGenerator()
		{
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public Type SourceObjectType
		{
			get {
				return typeof(SourceFile);
			}
		}

		/// <summary>
		/// Provide additional references to DLLs here if required.
		/// </summary>
		public IEnumerable<string> ReferencedAssemblies
		{
			get {
				yield break;
			}
		}


		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		/// <summary>
		/// Create the source code that is to be compiled.
		/// </summary>
		/// <param name="targetNameSpaceName"></param>
		/// <param name="targetClassName"></param>
		/// <returns></returns>
		public SourceCodeData CreateSourceContent(string filePath, object scriptObject, string targetNameSpaceName, string targetClassName)
		{
			SourceCodeData sourceCodeData = new SourceCodeData();
			sourceCodeData.Append("using System;");
			sourceCodeData.Append("using System.Text.RegularExpressions;");
			sourceCodeData.Append("using System.IO;");
			sourceCodeData.Append("using System.Collections.Generic;");
			sourceCodeData.Append("using System.Linq;");
			sourceCodeData.Append("using System.Text;");
			sourceCodeData.Append("namespace " + targetNameSpaceName + " {");
			sourceCodeData.Append("public class " + targetClassName + " {");
			sourceCodeData.Append("public " + targetClassName + "() {}");

			SourceFile sourceFile = (SourceFile)scriptObject;
			SrcMethod method = new SrcMethod("string", "Test", new SrcVariable("int", "something"), sourceFile.Text);
			method.WriteTo(sourceCodeData, "bla", -1);

			sourceCodeData.Append("}");
			sourceCodeData.Append("}");
			return sourceCodeData;
		}

	}

}
