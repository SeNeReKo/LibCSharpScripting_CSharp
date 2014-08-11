using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LibCSharpScripting.src
{

	/// <summary>
	/// This class holds information about registered assemblies to support source code generation.
	/// </summary>
	public class AssemblyManager
	{

		public class NamespaceAssemblyInfo
		{
			internal NamespaceAssemblyInfo(string path, Assembly assembly)
			{
				this.Path = path;
				this.Assembly = assembly;
			}

			public string Path
			{
				get;
				private set;
			}

			public Assembly Assembly
			{
				get;
				private set;
			}
		}

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		Dictionary<string, List<NamespaceAssemblyInfo>> namespaceRegistrationDB;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public AssemblyManager()
		{
			namespaceRegistrationDB = new Dictionary<string, List<NamespaceAssemblyInfo>>();
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public NamespaceAssemblyInfo[] GetNamespaceAssemblyInfos(string fullNamespace)
		{
			List<NamespaceAssemblyInfo> ret;
			if (namespaceRegistrationDB.TryGetValue(fullNamespace, out ret)) {
				return ret.ToArray();
			}
			return null;
		}

		public NamespaceAssemblyInfo[] GetNamespaceAssemblyInfos(params string[] fullNamespaces)
		{
			Dictionary<string, NamespaceAssemblyInfo> assemblies = new Dictionary<string, NamespaceAssemblyInfo>();
			foreach (string fullNamespace in fullNamespaces) {
				List<NamespaceAssemblyInfo> ret;
				if (namespaceRegistrationDB.TryGetValue(fullNamespace, out ret)) {
					foreach (NamespaceAssemblyInfo ai in ret) {
						if (assemblies.ContainsKey(ai.Path)) continue;
						else assemblies.Add(ai.Path, ai);
					}
				}
			}
			return assemblies.Values.ToArray();
		}

		public NamespaceAssemblyInfo[] GetNamespaceAssemblyInfos(IEnumerable<string> fullNamespaces)
		{
			Dictionary<string, NamespaceAssemblyInfo> assemblies = new Dictionary<string, NamespaceAssemblyInfo>();
			foreach (string fullNamespace in fullNamespaces) {
				List<NamespaceAssemblyInfo> ret;
				if (namespaceRegistrationDB.TryGetValue(fullNamespace, out ret)) {
					foreach (NamespaceAssemblyInfo ai in ret) {
						if (assemblies.ContainsKey(ai.Path)) continue;
						else assemblies.Add(ai.Path, ai);
					}
				}
			}
			return assemblies.Values.ToArray();
		}

		private static T[] AddToArray<T>(T[] array, T element)
		{
			T[] newArray = new T[array.Length + 1];
			array.CopyTo(newArray, 0);
			newArray[array.Length] = element;
			return newArray;
		}

		/*
		private string GetTypeName(Type type)
		{
			if (type.DeclaringType == null) return type.Name;

			string s = type.Name;
			Type t = type.DeclaringType;
			while (t != null) {
				s = t.Name + "." + s;
				t = t.DeclaringType;
			}
			return s;
		}
		*/

		public void RegisterAssembly(string path)
		{
			// SortedList<string, Type[]> typeMap = new SortedList<string, Type[]>();

			Assembly a = Assembly.LoadFile(path);
			HashSet<string> namespaces = new HashSet<string>();
			foreach (Type t in a.GetTypes()) {
				if (t.Namespace != null) {
					namespaces.Add(t.Namespace);
				}

				/*
				string typeName = GetTypeName(t);
				string typeName2 = t.Name;
				char c = typeName[0];
				char c2 = typeName2[0];
				if ((char.IsLetter(c) || (c == '_')) && (char.IsLetter(c2) || (c2 == '_'))) {
					Type[] types;
					if (typeMap.TryGetValue(typeName, out types)) {
						typeMap[typeName] = AddToArray<Type>(types, t);
					} else {
						typeMap.Add(typeName, new Type[] { t });
					}
				}
				*/
			}

			foreach (string s in namespaces) {
				List<NamespaceAssemblyInfo> l;
				if (!namespaceRegistrationDB.TryGetValue(s, out l)) {
					l = new List<NamespaceAssemblyInfo>();
					namespaceRegistrationDB.Add(s, l);
				}
				l.Add(new NamespaceAssemblyInfo(path, a));
			}
		}

		public string[] GetNamespaces()
		{
			HashSet<string> namespaces = new HashSet<string>();
			foreach (string key in namespaceRegistrationDB.Keys) {
				namespaces.Add(key);
			}
			string[] a = namespaces.ToArray();
			Array.Sort(a);
			return a;
		}

	}

}
