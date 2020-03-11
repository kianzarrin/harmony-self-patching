using ICities;
using JetBrains.Annotations;
using harmony_self_patching.Utils;
using harmony_self_patching.Patches;
using System;

namespace harmony_self_patching
{
    public class HarmonySelfPatchingMod : IUserMod
    {
        public string Name => "Patch Harmony" + VersionString + " " + BRANCH;
        public string Description => "Patches harmony 1.2 to become like latest harmony.";

#if DEBUG
        public const string BRANCH = "DEBUG";
#else
        public const string BRANCH = "";
#endif

        public static Version ModVersion => typeof(HarmonySelfPatchingMod).Assembly.GetName().Version;

        // used for in-game display
        public static string VersionString => ModVersion.ToString(2);

        HarmonyExtension harmonyExt;
        [UsedImplicitly]
        public void OnEnabled()
        {
            harmonyExt = new HarmonyExtension();
            harmonyExt.InstallHarmony();
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            harmonyExt?.UninstallHarmony();
            harmonyExt = null;
        }
    }
}