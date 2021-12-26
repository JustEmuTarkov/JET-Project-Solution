using JET.Utility;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    class CoreDifficulty : GenericPatch<CoreDifficulty>
    {
        public CoreDifficulty() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var getBotDifficultyHandler = typeof(EFT.MainApplication).Assembly.GetTypes().Where(type => type.Name.StartsWith("GClass") && type.GetMethod("CheckOnExcude", BindingFlags.Public | BindingFlags.Static) != null).First();
            if (getBotDifficultyHandler == null)
                return null;
            return getBotDifficultyHandler.GetMethod("LoadCoreByString", BindingFlags.Public | BindingFlags.Static);
        }

        public static bool PatchPrefix(ref string __result)
        {
            __result = Request();

            return string.IsNullOrWhiteSpace(__result);
        }

        private static string Request()
        {
            var json = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/settings/bot/difficulty/core/core");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JET]: Received core bot difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("[JET]: Successfully received core bot difficulty data");
            return json;
        }
    }
}
