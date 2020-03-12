namespace PatchOldHarmony.Patches{
    using HarmonyLib;
    using JetBrains.Annotations;
    using Utils;

    [HarmonyPatch(typeof(CitizenManager), "ReleaseCitizen")]
    [UsedImplicitly]
    public static class ReleaseCitizenPatch {
        /// <summary>
        /// Notifies the extended citizen manager about a released citizen.
        /// </summary>
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Postfix(CitizenManager __instance, uint citizen) {
            Log.Info("PatchOldHarmony.ReleaseCitizenPatch.Postfix() called ");
        }
    }
}