using PatchOldHarmony.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PatchOldHarmony.Patches
{
	internal static class AssemblyUtils
	{
		internal static Assembly ourAssembly => new StackTrace(true).GetFrame(1).GetMethod().DeclaringType.Assembly;

		internal static List<Assembly> Harmony12Assemblies =>
			AppDomain.CurrentDomain.GetAssemblies()
			.Where(assembly => IsHarmony12Assembly(assembly) && assembly != ourAssembly)
			.ToList();

		static bool IsHarmony12Assembly(Assembly assembly)
		{
			try
			{
				if (assembly.GetName().Name == "0Harmony" &&
					assembly != null &&
					assembly.ReflectionOnly == false &&
					assembly.GetType("Harmony.HarmonyInstance")?.FullName != null) {
					Version version = assembly.GetName().Version;
					if((version.Major == 1) & (version.Minor == 2)) {
						Log._Debug(assembly.ToString());
						return true;
					}
				}
			}
			catch { }
			return false;
		}

	}
}
