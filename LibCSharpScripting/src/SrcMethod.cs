using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LibCSharpScripting.src
{

	public class SrcMethod
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

		public SrcMethod(string methodName, SrcVariable methodVariable, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = (methodVariable == null) ? new SrcVariable[0] : new SrcVariable[] { methodVariable };
			this.MethodBody = methodBody;
			this.ReturnType = "void";
		}

		public SrcMethod(string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB };
			this.MethodBody = methodBody;
			this.ReturnType = "void";
		}

		public SrcMethod(string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, SrcVariable methodVariableC, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB, methodVariableC };
			this.MethodBody = methodBody;
			this.ReturnType = "void";
		}

		public SrcMethod(string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, SrcVariable methodVariableC, SrcVariable methodVariableD, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB, methodVariableC, methodVariableD };
			this.MethodBody = methodBody;
			this.ReturnType = "void";
		}

		public SrcMethod(string methodName, SrcVariable[] methodVariables, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = (methodVariables == null) ? new SrcVariable[0] : methodVariables;
			this.MethodBody = methodBody;
			this.ReturnType = "void";
		}

		public SrcMethod(string returnType, string methodName, SrcVariable methodVariable, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = (methodVariable == null) ? new SrcVariable[0] : new SrcVariable[] { methodVariable };
			this.MethodBody = methodBody;
			this.ReturnType = returnType;
		}

		public SrcMethod(string returnType, string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB };
			this.MethodBody = methodBody;
			this.ReturnType = returnType;
		}

		public SrcMethod(string returnType, string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, SrcVariable methodVariableC, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB, methodVariableC };
			this.MethodBody = methodBody;
			this.ReturnType = returnType;
		}

		public SrcMethod(string returnType, string methodName, SrcVariable methodVariableA, SrcVariable methodVariableB, SrcVariable methodVariableC, SrcVariable methodVariableD, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = new SrcVariable[] { methodVariableA, methodVariableB, methodVariableC, methodVariableD };
			this.MethodBody = methodBody;
			this.ReturnType = returnType;
		}

		public SrcMethod(string returnType, string methodName, SrcVariable[] methodVariables, string methodBody)
		{
			this.MethodName = methodName;
			this.MethodVariables = (methodVariables == null) ? new SrcVariable[0] : methodVariables;
			this.MethodBody = methodBody;
			this.ReturnType = returnType;
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public string ReturnType
		{
			get;
			private set;
		}

		public string MethodName
		{
			get;
			private set;
		}

		public string MethodBody
		{
			get;
			private set;
		}

		public SrcVariable[] MethodVariables
		{
			get;
			private set;
		}

		private string MethodVariablesStr
		{
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < MethodVariables.Length; i++) {
					if (i > 0) sb.Append(", ");
					sb.Append(MethodVariables[i].ToString());
				}
				return sb.ToString();
			}
		}

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public void WriteTo(SourceCodeData sourceCodeData, string fileName, int filePart)
		{
			sourceCodeData.Append("public " + ReturnType + " " + MethodName + "(" + MethodVariablesStr + ") {");
			string[] lines = SourceCodeData.SplitLines(MethodBody);
			sourceCodeData.Append(fileName, filePart, 0, lines);
			sourceCodeData.Append("}");
		}

	}

}
