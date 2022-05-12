using JET.Patches.Core;
using JET.Utility.Patching;

namespace JET
{
    class PatchRunner
    {
        /// <summary>
        /// Method calling Core Patches to override Filechecker/Battleye/SSL/WebRequests etc.
        /// </summary>
        internal static void ExecuteCorePatches() 
        {
            // Order is crucial
            HarmonyPatch.Patch<FileChecker_Override>();
            HarmonyPatch.Patch<BattleEye>();
            //HarmonyPatch.Patch<NotificationSsl>();
            HarmonyPatch.Patch<SslCertificate>();
            HarmonyPatch.Patch<UnityWebRequest>();
        }
        /// <summary>
        /// Method that runs Patches to Override the logger ingame and adds a custom logger inside enabling full logging for debugging
        /// </summary>
        internal static void ExecuteLoggerPatches() 
        {
            HarmonyPatch.Patch<Patches.Logger.InitialHook>();
            HarmonyPatch.Patch<Patches.Logger.LoggerOverride>();
            HarmonyPatch.Patch<Patches.Logger.ResetHook>();
        }
    }
}
