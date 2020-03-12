
namespace PatchOldHarmony.Patches
{
    using HarmonyLib;
    using PatchOldHarmony.Utils;
    using System.Collections.Generic;
    using System.Reflection;


    public class HarmonyExtension
    {
        Harmony harmony;
        public const string HARMONY_ID = "CS.Kian.harmony_self_patching";
        struct PatchPair
        {
            public MethodBase Original;
            public MethodInfo Patch;
        }
        private List<PatchPair> patches_;

        public void InstallHarmony()
        {
#if !DEBUG
            // TODO: this does not work because Before OnCreate we don't know if we are in asset editor.
            if (Extensions.InAssetEditor) {
                Log.Info("skipped InstallHarmony in asset editor release build");
                return;
            }
#endif
            if (harmony == null)
            {
                Log.Info("harmony_self_patching Patching...");
#if DEBUG
                Harmony.DEBUG = true;
                CreatePatchedMethod_Patch.Test();
#endif
                harmony = new Harmony(HARMONY_ID);

                var prefix = typeof(CreatePatchedMethod_Patch).GetMethod("Prefix");
                patches_ = new List<PatchPair>();
                foreach (var original in CreatePatchedMethod_Patch.TargetMethods())
                {
                    var patch = harmony.Patch(original, new HarmonyMethod(prefix));
                    
                    if (patch != null)
                    {
                        Log._Debug("Added prefix to: " + original);
                        patches_.Add(new PatchPair { Original = original, Patch = patch });
                    }
                    else
                    {
                        Log.Error("FAILED to Add prefix to: " + original);
                    }
                }
                harmony.PatchAll();
            }
        }

        public void UninstallHarmony()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll(HARMONY_ID);
                foreach (var patchPair in patches_)
                    harmony.Unpatch(patchPair.Original, patchPair.Patch);
                patches_ = null;
                harmony = null;
                Log.Info("harmony_self_patching patches Reverted.");
            }
        }
    }
}