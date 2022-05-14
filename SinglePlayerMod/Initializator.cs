

using JET.Utility;
using JET.Utility.Modding;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;

namespace SinglePlayerMod
{
    public class Initializator : JetMod
    {
        protected override void Initialize(IReadOnlyDictionary<Type, JetMod> dependencies, string gameVersion)
        {
            //try
            //{
                LoadPatchesFromServer.Call(); // initiate the "should i load this patch or not"
                Patch_Flea();
                Patch_HealthListner();
                Patch_MatchMaker();
                Patch_Other();
                Patch_Progression();
                Patch_Quests();
                Patch_Raid();
                Patch_ScavMode();
                Patch_Trading();
            //}
            //catch (Exception e)
            //{
            //    System.IO.File.AppendAllText(JET.Utility.Paths.GetGameDirectory + "/ErrorInSinglePlayer.log", e.Message + " || " + e.StackTrace + "\n" +
            //        EFTBackendSettings.Instance.SelectedBackendUrl);
            //}
        }
        private void Patch_Flea() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Flea)
            {
                if (LoadPatchesFromServer.List_Patch.RemoveAddOfferButton_Awake)
                    HarmonyPatch.Patch<Patches.Flea.RemoveAddOfferButton_Awake>();
                if (LoadPatchesFromServer.List_Patch.RemoveAddOfferButton_Call)
                    HarmonyPatch.Patch<Patches.Flea.RemoveAddOfferButton_Call>();
            }
        }
        private void Patch_HealthListner() 
        {
            if (LoadPatchesFromServer.List_PatchNode.HealthListner)
            {
                if (LoadPatchesFromServer.List_Patch.ReplaceInMainMenuController)
                    HarmonyPatch.Patch<Patches.HealthListner.ReplaceInMainMenuController>();
                if (LoadPatchesFromServer.List_Patch.ReplaceInPlayer)
                    HarmonyPatch.Patch<Patches.HealthListner.ReplaceInPlayer>();
            }
        }
        private void Patch_MatchMaker() 
        {
            if (LoadPatchesFromServer.List_PatchNode.MatchMaker)
            {
                //if (LoadPatchesFromServer.List_Patch.AutoSetOfflineMatch)
                //    HarmonyPatch.Patch<Patches.MatchMaker.AutoSetOfflineMatch>();
                //if (LoadPatchesFromServer.List_Patch.BringBackInsuranceScreen)
                //    HarmonyPatch.Patch<Patches.MatchMaker.BringBackInsuranceScreen>();
                if (LoadPatchesFromServer.List_Patch.DisableReadyButtonOnFirstScreen)
                    HarmonyPatch.Patch<Patches.MatchMaker.DisableReadyButtonOnFirstScreen>();
                if (LoadPatchesFromServer.List_Patch.DisableReadyButtonOnSelectLocation)
                    HarmonyPatch.Patch<Patches.MatchMaker.DisableReadyButtonOnSelectLocation>();
            }
        }
        private void Patch_Other() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Other)
            {
                if (LoadPatchesFromServer.List_Patch.NoFilters)
                    HarmonyPatch.Patch<Patches.Other.NoFilters>();
                if (LoadPatchesFromServer.List_Patch.OldStreamerMode)
                    HarmonyPatch.Patch<Patches.Other.OldStreamerMode>();
                if (LoadPatchesFromServer.List_Patch.UnlockItem24CharId)
                    HarmonyPatch.Patch<Patches.Other.UnlockItem24CharId>();
            }
        }
        private void Patch_Progression() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Progression)
            {
                if (LoadPatchesFromServer.List_Patch.EndByTimer)
                    HarmonyPatch.Patch<Patches.Progression.EndByTimer>();
                if (LoadPatchesFromServer.List_Patch.ExperienceGainFix)
                    HarmonyPatch.Patch<Patches.Progression.ExperienceGainFix>();
                if (LoadPatchesFromServer.List_Patch.OfflineSaveProfile)
                    HarmonyPatch.Patch<Patches.Progression.OfflineSaveProfile>();
                //if (LoadPatchesFromServer.List_Patch.OfflineSpawnPoint)
                //    HarmonyPatch.Patch<Patches.Progression.OfflineSpawnPoint>();
            }
        }
        private void Patch_Quests() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Quests)
            {
                //HarmonyPatch.Patch<Patches.Quests.ItemDroppedAtPlace_Beacon>(); // Disabled due to it being already fixed in the client
                if (LoadPatchesFromServer.List_Patch.UpdateDogtagOnKill)
                    HarmonyPatch.Patch<Patches.Quests.UpdateDogtagOnKill>();
            }
        }
        private void Patch_Raid() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Raid)
            {
                //if (LoadPatchesFromServer.List_Patch.BossSpawnChance)
                //    HarmonyPatch.Patch<Patches.Raid.BossSpawnChance>();
                //if (LoadPatchesFromServer.List_Patch.BotSettingsLoad)
                //    HarmonyPatch.Patch<Patches.Raid.BotSettingsLoad>();
                //if (LoadPatchesFromServer.List_Patch.BotTemplateLimit)
                //    HarmonyPatch.Patch<Patches.Raid.BotTemplateLimit>();
                if (LoadPatchesFromServer.List_Patch.CoreDifficulty)
                    HarmonyPatch.Patch<Patches.Raid.CoreDifficulty>();
                if (LoadPatchesFromServer.List_Patch.LoadBotDifficultyFromServer)
                    HarmonyPatch.Patch<Patches.Raid.LoadBotDifficultyFromServer>();
                if (LoadPatchesFromServer.List_Patch.LoadBotTemplatesFromServer)
                    HarmonyPatch.Patch<Patches.Raid.LoadBotTemplatesFromServer>();
                //if (LoadPatchesFromServer.List_Patch.MaxBotCap)
                //    HarmonyPatch.Patch<Patches.Raid.MaxBotCap>();
                if (LoadPatchesFromServer.List_Patch.RemoveUsedBotProfile)
                    HarmonyPatch.Patch<Patches.Raid.RemoveUsedBotProfile>();
                //if (LoadPatchesFromServer.List_Patch.SpawnPmc)
                //    HarmonyPatch.Patch<Patches.Raid.SpawnPmc>();
                //if (LoadPatchesFromServer.List_Patch.SpawnRandomizationMod)
                //    HarmonyPatch.Patch<Patches.Raid.SpawnRandomizationMod>();
                //if (LoadPatchesFromServer.List_Patch.TinnitusFix)
                //    HarmonyPatch.Patch<Patches.Raid.TinnitusFix>();
            }
        }
        private void Patch_ScavMode() 
        {
            if (LoadPatchesFromServer.List_PatchNode.ScavMode)
            {
                if (LoadPatchesFromServer.List_Patch.DisableScavMode)
                    HarmonyPatch.Patch<Patches.ScavMode.DisableScavMode>();
                if (LoadPatchesFromServer.List_Patch.LoadOfflineRaidScreen)
                    HarmonyPatch.Patch<Patches.ScavMode.LoadOfflineRaidScreen>();
                if (LoadPatchesFromServer.List_Patch.ScavExfilFix)
                    HarmonyPatch.Patch<Patches.ScavMode.ScavExfilFix>();
                if (LoadPatchesFromServer.List_Patch.ScavPrefabLoad)
                    HarmonyPatch.Patch<Patches.ScavMode.ScavPrefabLoad>();
                if (LoadPatchesFromServer.List_Patch.ScavProfileLoad)
                    HarmonyPatch.Patch<Patches.ScavMode.ScavProfileLoad>();
                if (LoadPatchesFromServer.List_Patch.ScavSpawnPoint)
                    HarmonyPatch.Patch<Patches.ScavMode.ScavSpawnPoint>();
            }
        }
        private void Patch_Trading() 
        {
            if (LoadPatchesFromServer.List_PatchNode.Trading)
            {
                if (LoadPatchesFromServer.List_Patch.BarterSchemeAutoFill)
                    HarmonyPatch.Patch<Patches.Trading.BarterSchemeAutoFill>();
                if (LoadPatchesFromServer.List_Patch.BarterSchemeAutoFillPersist)
                    HarmonyPatch.Patch<Patches.Trading.BarterSchemeAutoFillPersist>();
            }

        }
    }
}
