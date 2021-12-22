using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET
{
    class PatchRunner
    {
        internal static void ExecuteCorePatches() 
        {
            // Order is crucial
            HarmonyPatch.Patch<Patches.Core.EnsureConsistency>();
            HarmonyPatch.Patch<Patches.Core.BattlEye>();
            HarmonyPatch.Patch<Patches.Core.NotificationSsl>();
            HarmonyPatch.Patch<Patches.Core.SslCertificate>();
            HarmonyPatch.Patch<Patches.Core.UnityWebRequest>();
        }
        internal static void ExecuteLoggerPatches() 
        {
            HarmonyPatch.Patch<Patches.Logger.InitialHook>();
            HarmonyPatch.Patch<Patches.Logger.LoggerOverride>();
            HarmonyPatch.Patch<Patches.Logger.ResetHook>();
        }
    }
}
