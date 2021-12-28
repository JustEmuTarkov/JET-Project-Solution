using EFT;
using JET.Utility;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    class LoadBotDifficultyFromServer : GenericPatch<LoadBotDifficultyFromServer>
    {
        public LoadBotDifficultyFromServer() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var getBotDifficultyHandler = typeof(EFT.MainApplication).Assembly.GetTypes().Where(type => type.Name.StartsWith("GClass") && type.GetMethod("CheckOnExcude", BindingFlags.Public | BindingFlags.Static) != null).First();
            if (getBotDifficultyHandler == null)
                return null;
            //return getBotDifficultyHandler.GetMethod("LoadInternal", BindingFlags.Public | BindingFlags.Static);
            return getBotDifficultyHandler.GetMethod("LoadDifficultyStringInternal", BindingFlags.Public | BindingFlags.Static);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = Request(role, botDifficulty);
            return string.IsNullOrWhiteSpace(__result);
        }

        private static string Request(WildSpawnType role, BotDifficulty botDifficulty)
        {
            var json = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/settings/bot/difficulty/" + role.ToString() + "/" + botDifficulty.ToString());

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JET]: Received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("[JET]: Successfully received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data");
            return json;
        }
    }
}
