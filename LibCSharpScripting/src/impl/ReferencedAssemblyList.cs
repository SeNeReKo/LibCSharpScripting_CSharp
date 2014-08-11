using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LibCSharpScripting.src.impl
{

	public class ReferencedAssemblyList
	{

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		SortedList<string, string> loadedAssemblies;
		HashSet<string> loadedAssemblyNames;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public ReferencedAssemblyList()
		{
			loadedAssemblies = new SortedList<string, string>();
			loadedAssemblyNames = new HashSet<string>();

			// ----

			Assembly executingAssembly = Assembly.GetExecutingAssembly();

			loadedAssemblies.Add(executingAssembly.Location.ToLower(), executingAssembly.Location);

			string fileName = Path.GetFileName(executingAssembly.Location).ToLower();
			loadedAssemblyNames.Add(fileName);

			// ----

			foreach (AssemblyName assemblyName in executingAssembly.GetReferencedAssemblies()) {
				string path = Assembly.Load(assemblyName).Location;

				loadedAssemblies.Add(path.ToLower(), path);

				fileName = Path.GetFileName(path).ToLower();
				loadedAssemblyNames.Add(fileName);
			}
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public void AddLoadAssemblies(IEnumerable<string> assemblyPaths)
		{
			foreach (string path in assemblyPaths) {
				string fileName = Path.GetFileName(path).ToLower();
				if (loadedAssemblyNames.Contains(fileName)) continue;

				string pathLower = path.ToLower();
				if (loadedAssemblies.ContainsKey(pathLower)) continue;
				if (!File.Exists(path)) continue;
				string path2 = Assembly.LoadFrom(path).Location;
				pathLower = path2.ToLower();
				if (loadedAssemblies.ContainsKey(pathLower)) continue;
				loadedAssemblies.Add(pathLower, path2);
			}
		}

		public void AddLoadAssemblies(params string[] assemblyPaths)
		{
			foreach (string path in assemblyPaths) {
				string fileName = Path.GetFileName(path).ToLower();
				if (loadedAssemblyNames.Contains(fileName)) continue;

				string pathLower = path.ToLower();
				if (loadedAssemblies.ContainsKey(pathLower)) continue;
				if (!File.Exists(path)) continue;
				string path2 = Assembly.LoadFrom(path).Location;
				pathLower = path2.ToLower();
				if (loadedAssemblies.ContainsKey(pathLower)) continue;
				loadedAssemblies.Add(pathLower, path2);
			}
		}

		public string[] ToArray()
		{
			return loadedAssemblies.Values.ToArray();
		}

	}

}
