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

namespace PatchOldHarmony.Patches.OldHarmony { 
    using HarmonyLib;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using Utils;

    public static class CreatePatchedMethod_Patch
    {
        // public static DynamicMethod MethodPatcher:CreatePatchedMethod(
        // MethodBase original, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)

        public static List<MethodBase> TargetMethods()
        {
            var ret = new List<MethodBase>();
            foreach( var assembly in AssemblyUtils.Harmony12Assemblies)
            {
                var targetMethod = assembly.GetType("MethodPatcher").GetMethod(
                    "CreatePatchedMethod",
                    new Type[]
                    {
                        typeof(MethodBase),
                        typeof(List<MethodInfo>),
                        typeof(List<MethodInfo>),
                        typeof(List<MethodInfo>)
                    });
                ret.Add(targetMethod);
            }
            return ret;
        }

        //
        //internal MethodInfo MethodPatcher:CreateReplacement(out Dictionary<int, CodeInstruction> finalInstructions)
        public static MethodInfo MethodPatcher_CreateReplacement =>
            AssemblyUtils.ourAssembly.GetType("HarmonyLib.MethodPatcher").
            GetMethod("CreateReplacement",BindingFlags.NonPublic|BindingFlags.Instance) ??
            throw new Exception("could not find CreateReplacement");


        //internal MethodPatcher(
        // MethodBase original, MethodBase source, 
        // List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers, List<MethodInfo> finalizers, 
        // bool debug)
        public static ConstructorInfo MethodPatcher_Ctor =>
            AssemblyUtils.ourAssembly.GetType("HarmonyLib.MethodPatcher").GetConstructor(
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

        public static void Prefix(
            //ref DynamicMethod __result,
            MethodBase original, 
            List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers) {

            //var patcher = new MethodPatcher(original, null, sortedPrefixes, sortedPostfixes, sortedTranspilers, sortedFinalizers, debug);
            //var replacement = patcher.CreateReplacement(out var finalInstructions);
            //if (replacement == null) throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");

            object patcher = MethodPatcher_Ctor.Invoke(
                new object[] { original, null, prefixes, postfixes, transpilers, null, Harmony.DEBUG });

            object[] arguments = new object[] { null };
            MethodInfo replacement = MethodPatcher_CreateReplacement.
                Invoke(patcher, arguments) as MethodInfo
                ?? throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");

            Log.Info("KIAN DEBUG> replacement type is :" + replacement.GetType());

            //__result = ref (replacement as DynamicILInfo);
            //return false;
        }

    } // end class
} // end name space