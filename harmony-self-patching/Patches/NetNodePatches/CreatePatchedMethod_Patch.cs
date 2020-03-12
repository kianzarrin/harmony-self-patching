/*
 * - Check call hirarchy for public static object ConvertInstruction(Type type, object op, out Dictionary<string, object> unassigned)
 *     > 	public static class MethodPatcher
 *          [UpgradeToLatestVersion(1)]
 *          public static DynamicMethod CreatePatchedMethod(MethodBase original, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
 * - PatchOldHarmonyMethods 
 *
 * - I need to patch old harmony before `OnEnable` but after the dll is loaded. I think I need to patch CS too.
 * 
 */

namespace PatchOldHarmony.Patches { 
    using HarmonyLib;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using Utils;

    public class CreatePatchedMethod_Patch
    {
        //public static DynamicMethod MethodPatcher:CreatePatchedMethod(
        // MethodBase original, 
        // string harmonyInstanceID, 
        // List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
        public static List<MethodBase> TargetMethods()
        {
            var ret = new List<MethodBase>();
            foreach ( var assembly in AssemblyUtils.Harmony12Assemblies)
            {
                var targetMethod = assembly.GetType("Harmony.MethodPatcher")?.GetMethod(
                    "CreatePatchedMethod",
                    new Type[]
                    {
                        typeof(MethodBase),
                        typeof(string),
                        typeof(List<MethodInfo>),
                        typeof(List<MethodInfo>),
                        typeof(List<MethodInfo>)
                    });
                if(targetMethod!=null)
                    ret.Add(targetMethod);
            }
            Log._Debug("TargetMethods count = " + ret.Count);
            return ret;
        }

        //internal MethodInfo MethodPatcher:CreateReplacement(out Dictionary<int, CodeInstruction> finalInstructions)
        static MethodInfo MethodPatcher_CreateReplacement =>
            AssemblyUtils.HarmonyCentralAssembly.GetType("HarmonyLib.MethodPatcher").
            GetMethod("CreateReplacement",BindingFlags.NonPublic|BindingFlags.Instance) ??
            throw new Exception("could not find CreateReplacement");


        //internal MethodPatcher(
        // MethodBase original, MethodBase source, 
        // List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers, List<MethodInfo> finalizers, 
        // bool debug)
        static ConstructorInfo MethodPatcher_Ctor =>
            AssemblyUtils.HarmonyCentralAssembly.GetType("HarmonyLib.MethodPatcher").GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                {
                    typeof(MethodBase),
                    typeof(MethodBase),
                    typeof(List<MethodInfo>),
                    typeof(List<MethodInfo>),
                    typeof(List<MethodInfo>),
                    typeof(List<MethodInfo>),
                    typeof(bool),
                },
                null) ?? throw new Exception("could not find MethodPatcher.Ctor");

        public static void Test()
        {
            var b = MethodPatcher_CreateReplacement;
            var a = MethodPatcher_Ctor;
            Log._Debug("static load test: found " + a + " and " + b);
        }

        public static bool Prefix(
            ref DynamicMethod __result,
            MethodBase original, 
            List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers) {
            try
            {
                Log.Info("KIAN DEBUG> CreatePatchedMethod_Patch.Prefix() called");

                //var patcher = new MethodPatcher(original, null, sortedPrefixes, sortedPostfixes, sortedTranspilers, sortedFinalizers, debug);
                //var replacement = patcher.CreateReplacement(out var finalInstructions);
                //if (replacement == null) throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");

                List<MethodInfo> sources = null;
                List<MethodInfo> finalizers = new List<MethodInfo>();
                object patcher = MethodPatcher_Ctor.Invoke(
                    new object[] { original, sources, prefixes, postfixes, transpilers, finalizers, Harmony.DEBUG });
                Log.Info("KIAN DEBUG> patcher = new MethodPatcher() returned " + patcher);

                object[] arguments = new object[] { null };
                MethodInfo replacement = MethodPatcher_CreateReplacement.
                    Invoke(patcher, arguments) as MethodInfo
                    ?? throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");
                Log.Info("KIAN DEBUG> replacement = patcher.CreateReplacement() returned " + replacement);
  
                __result = replacement as DynamicMethod;
                return false;
            }
            catch(Exception e)
            {
                Log.Error(e.ToString());
                return true;
            }
        }

    } // end class
} // end name space