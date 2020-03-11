using HarmonyLib;

namespace harmony_self_patching.Patches
{
    public class HarmonyExtension
    {
        Harmony harmony;
        public const string HARMONY_ID = "CS.Kian.harmony_self_patching";

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
#endif
                harmony = new Harmony(HARMONY_ID);
                harmony.PatchAll(GetType().Assembly);
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