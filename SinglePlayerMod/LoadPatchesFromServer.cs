using JET.Utility;
using Newtonsoft.Json;

namespace SinglePlayerMod
{
    class LoadPatchesFromServer
    {
        public static PatchList List_Patch = new PatchList();
        public static PatchNodeList List_PatchNode = new PatchNodeList();

        public static void Call()
        {
            var request = new Request(null, ClientAccesor.BackendUrl);
            try
            {
                var json = request.GetJson("/mode/offline");
                List_Patch = JsonConvert.DeserializeObject<PatchList>(json);
            }
            catch
            {
                List_Patch = new PatchList();
            }
            try
            {
                var json = request.GetJson("/mode/offlineNodes");
                List_PatchNode = JsonConvert.DeserializeObject<PatchNodeList>(json);
            }
            catch
            {
                List_PatchNode = new PatchNodeList();
            }
        }

        public class PatchList {
            public bool RemoveAddOfferButton_Awake = true;
            public bool RemoveAddOfferButton_Call = true;
            public bool ReplaceInMainMenuController = true;
            public bool ReplaceInPlayer = true;
            public bool AutoSetOfflineMatch = true;
            public bool BringBackInsuranceScreen = true;
            public bool DisableReadyButtonOnFirstScreen = true;
            public bool DisableReadyButtonOnSelectLocation = true;
            public bool NoFilters = true;
            public bool OldStreamerMode = true;
            public bool UnlockItem24CharId = true;
            public bool EndByTimer = true;
            public bool ExperienceGainFix = true;
            public bool OfflineSaveProfile = true;
            public bool OfflineSpawnPoint = true;
            //public bool ItemDroppedAtPlace_Beacon = true; // Disabled due to it being already fixed in the client
            public bool UpdateDogtagOnKill = true;
            public bool BossSpawnChance = true;
            public bool BotSettingsLoad = true;
            public bool BotTemplateLimit = true;
            public bool CoreDifficulty = true;
            public bool LoadBotDifficultyFromServer = true;
            public bool LoadBotTemplatesFromServer = true;
            public bool MaxBotCap = true;
            public bool RemoveUsedBotProfile = true;
            public bool SpawnPmc = true;
            public bool SpawnRandomizationMod = true;
            public bool TinnitusFix = true;
            public bool LoadOfflineRaidScreen = true;
            public bool ScavExfilFix = true;
            public bool ScavPrefabLoad = true;
            public bool ScavProfileLoad = true;
            public bool ScavSpawnPoint = true;
            public bool BarterSchemeAutoFill = true;
            public bool BarterSchemeAutoFillPersist = true;
        }
        public class PatchNodeList {
            public bool Flea = true;
            public bool HealthListner = true;
            public bool MatchMaker = true;
            public bool Other = true;
            public bool Progression = true;
            public bool Quests = true;
            public bool Raid = true;
            public bool ScavMode = true;
            public bool Trading = true;
        }
    }
}
