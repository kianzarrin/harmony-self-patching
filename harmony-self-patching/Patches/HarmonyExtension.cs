
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

        public void InstallHarmony()
        {

            if (harmony == null)
            {
                Log.Info("harmony_self_patching Patching...");

                harmony = new Harmony(HARMONY_ID);
                harmony.PatchAll();
            }
        }

        public void UninstallHarmony()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll(HARMONY_ID);
                harmony = null;
                Log.Info("harmony_self_patching patches Reverted.");
            }
        }
    }
}