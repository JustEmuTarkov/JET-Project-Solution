using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT;
using EFT.UI.Screens;
using HarmonyLib;
using JET.Utility.Patching;
using JET.Utility.Reflection;

using MenuController = GClass1504; // .SelectedKeyCard
using WeatherSettings = GStruct97; // IsRandomTime and IsRandomWeather
using BotsSettings = GStruct239; // IsScavWars and BotAmount
using WavesSettings = GStruct99; // IsTaggedAndCursed and IsBosses
using MatchmakerScreenCreator = EFT.UI.Matchmaker.MatchmakerOfflineRaid.GClass2394; // simply go to class below and search for new gclass, simple as that...
using JET.Utility;

namespace SinglePlayerMod.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;
    class LoadOfflineRaidScreen : GenericPatch<LoadOfflineRaidScreen>
    {

        private const string loadReadyScreenMethod = "method_39";
        private const string readyMethod = "method_60";

        public LoadOfflineRaidScreen() : base(transpiler: nameof(PatchTranspiler)) { }

        protected override MethodBase GetTargetMethod()
        {
            /*
             search for Class816 but actually its Class1010
             so i made it so it searches for that thing automatickly less burden later on ...
            */
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.IsNested && x.GetField("selectLocationScreenController", BindingFlags.Public | BindingFlags.Instance) != null)
                .GetMethod("method_2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var index = 26;
            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreen), "LoadOfflineRaidScreenForScav"));

            codes[index].opcode = OpCodes.Nop;
            codes[index + 1] = callCode;
            codes.RemoveAt(index + 2);

            return codes.AsEnumerable();
        }


        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            var menuController = PrivateValueAccessor.GetPrivateFieldValue(typeof(MainApplication),
                                    $"{typeof(MenuController).Name.ToLower()}_0", ClientAccesor.GetMainApp()) as MenuController;

            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), "bool_0", menuController, local);
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), $"{typeof(BotsSettings).Name.ToLower()}_0", menuController, botsSettings);
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), $"{typeof(WeatherSettings).Name.ToLower()}_0", menuController, weatherSettings);
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), $"{typeof(WavesSettings).Name.ToLower()}_0", menuController, wavesSettings);

            typeof(MenuController).GetMethod(loadReadyScreenMethod, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menuController, null);
        }

        public static void LoadOfflineRaidScreenForScav()
        {
            var menuController = PrivateValueAccessor.GetPrivateFieldValue(Constants.MainApplicationType,
                                    $"{Constants.MainApplicationType.Name.ToLower()}_0", ClientAccesor.GetMainApp());
            var gclass = new MatchmakerScreenCreator();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, readyMethod);
            gclass.ShowScreen(EScreenState.Queued);
        }
    }
}
