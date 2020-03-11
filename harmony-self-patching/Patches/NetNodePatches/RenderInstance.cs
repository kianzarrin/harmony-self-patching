using HarmonyLib;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace harmony_self_patching.Patches.NetNodePatches {
    using Utils;

    [HarmonyPatch()]
    public static class RenderInstance {
        static void Log(string m) => Utils.Log.Info("NetNode_RenderInstance Transpiler: " + m);

        // RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, Flags flags, ref uint instanceIndex, ref RenderManager.Instance data)
        static MethodInfo Target => typeof(global::NetNode).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodBase TargetMethod() {
            var ret = Target;
            Extensions.Assert(ret != null, "did not manage to find original function to patch");
            Log("aquired method " + ret);
            return ret;
        }

        //static bool Prefix(ushort nodeID){}
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions) {
            try {
                var codes = TranspilerUtils.ToCodeList(instructions);
                CalculateMaterialCommons.PatchCheckFlags(codes, occurance: 2, Target);

                Log("successfully patched NetNode.RenderInstance");
                return codes;
            }catch(Exception e) {
                Utils.Log.Error(e.ToString());
                throw e;
            }
        }
    } // end class
} // end name space