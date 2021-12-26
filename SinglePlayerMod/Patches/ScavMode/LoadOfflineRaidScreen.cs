using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT.UI.Screens;
using HarmonyLib;
using JET.Utility.Patching;
using JET.Utility.Reflection;
using JET.Utility;

using MenuController = GClass1504; // .SelectedKeyCard
using WeatherSettings = GStruct97; // IsRandomTime and IsRandomWeather
using BotsSettings = GStruct239; // IsScavWars and BotAmount
using WavesSettings = GStruct99; // IsTaggedAndCursed and IsBosses
using MatchmakerScreenCreator = EFT.UI.Matchmaker.MatchmakerOfflineRaid.GClass2394; // simply go to class below and search for new gclass, simple as that...

namespace SinglePlayerMod.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;
    class LoadOfflineRaidScreen : GenericPatch<LoadOfflineRaidScreen>
    {

        private const string loadReadyScreenMethod = "method_39";
        private const string readyMethod = "method_61";

        public LoadOfflineRaidScreen() : base(transpiler: nameof(PatchTranspiler)){}

        protected override MethodBase GetTargetMethod()
        {
            /*
             search for Class816 but actually its Class1010
             so i made it so it searches for that thing automatickly less burden later on ...
            */
            return Constants.MenuControllerType.GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.IsNested && x.GetField("selectLocationScreenController", Constants.PublicInstanceFlag) != null)
                .GetMethod("method_2", Constants.NonPublicInstanceDeclaredOnlyFlag);
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
        public static void LoadOfflineRaidScreenForScav()
        {
            var menuController = PrivateValueAccessor.GetPrivateFieldValue(
                Constants.MainApplicationType,
                $"{Constants.MainApplicationType.Name.ToLower()}_0",
                ClientAccesor.GetMainApp());

            var gclass = new MatchmakerScreenCreator();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, readyMethod);
            gclass.ShowScreen(EScreenState.Queued);
        }

        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            var MenuControllerObject = PrivateValueAccessor.GetPrivateFieldValue(
                Constants.MainApplicationType,
                $"{Constants.MenuControllerType.Name.ToLower()}_0", 
                ClientAccesor.GetMainApp()) as MenuController;
            // if we get rid of that menu controller object from here we can get rid of the using of gclass which will lessen the fuckery :)
            // also same goes for field accessing in this function.
            //Debug.LogError("Traverse: " + Traverse.Create(MenuControllerObject).Field("SelectedLocation").Field("Id").GetValue());
            // check if we can use Traverse...

            // will be great to rewrite this in more reflection version so we can disband the use of "using MenuController = GClass1504;"
            if (MenuControllerObject.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            PrivateValueAccessor.SetPrivateFieldValue(Constants.MenuControllerType, "bool_0", MenuControllerObject, local);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.MenuControllerType, $"{typeof(BotsSettings).Name.ToLower()}_0", MenuControllerObject, botsSettings);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.MenuControllerType, $"{typeof(WeatherSettings).Name.ToLower()}_0", MenuControllerObject, weatherSettings);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.MenuControllerType, $"{typeof(WavesSettings).Name.ToLower()}_0", MenuControllerObject, wavesSettings);

            Constants.MenuControllerType.GetMethod(loadReadyScreenMethod, Constants.NonPublicInstanceFlag).Invoke(MenuControllerObject, null);
        }

    }
}
