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

	public class CompiledMethodWrapper
	{

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		private object scriptObj;
		private MethodInfo method;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		internal CompiledMethodWrapper(string scriptFileName, object scriptObj, Type compiledClass,
			MethodSpecification methodSpecification, MethodInfo method)
		{
			this.scriptObj = scriptObj;
			this.method = method;
			this.MethodSpecification = methodSpecification;
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public MethodSpecification MethodSpecification
		{
			get;
			private set;
		}

		public SrcVariable[] Arguments
		{
			get {
				return MethodSpecification.Arguments;
			}
		}

		public string Name
		{
			get {
				return MethodSpecification.Name;
			}
		}

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public object Execute(params KeyValuePair<string, object>[] kvps)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in kvps) {
				dict.Add(kvp.Key, kvp.Value);
			}
			return Execute(dict);
		}

		public object Execute(IDictionary<string, object> kvps)
		{
			object[] parameters = new object[Arguments.Length];
			int i = 0;
			foreach (SrcVariable v in Arguments) {
				object value;
				if (kvps.TryGetValue(v.Name, out value)) {
					parameters[i] = value;
				}
				i++;
			}
			return method.Invoke(scriptObj, parameters);
		}

	}

}
