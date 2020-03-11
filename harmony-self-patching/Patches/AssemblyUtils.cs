using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

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
			try {
				if(assembly !=null && assembly.ReflectionOnly == false)
				{
					Version version = assembly.GetName().Version;
					if((version.Major == 1) & (version.Minor == 2))
					{
						return assembly.GetType("HarmonyInstance")?.FullName != null;
					}
				}
			} catch { }
			return false;
		}

	}
}
