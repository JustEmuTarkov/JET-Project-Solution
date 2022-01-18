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

using MainMenuController = GClass1528; // .SelectedKeyCard
using WeatherSettings = GStruct101;
using BotsSettings = GStruct244;
using WavesSettings = GStruct103;
//using WeatherSettings  IsRandomTime and IsRandomWeather
//using BotsSettings  IsScavWars and BotAmount
//using WavesSettings  IsTaggedAndCursed and IsBosses
using MatchmakerScreenCreator = EFT.UI.Matchmaker.MatchmakerOfflineRaid.GClass2423; // simply go to class below and search for new gclass, simple as that...

namespace SinglePlayerMod.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;
    class LoadOfflineRaidScreen : GenericPatch<LoadOfflineRaidScreen>
    {
        /*
         // GClass1528
// Token: 0x06006CD2 RID: 27858 RVA: 0x002DCEF8 File Offset: 0x002DB0F8
private void method_39()
{
	if (this.method_41() && this.method_42())
	{
		MatchMakerAcceptScreen.GClass2419 gclass = new MatchMakerAcceptScreen.GClass2419(this.ginterface30_0.Session, this.esideType_0, this.SelectedDateTime, this.SelectedLocation, this.bool_0, this.SelectedKeyCard);
		gclass.OnShowNextScreen += this.method_63;
		gclass.ShowScreen(EScreenState.Queued);
		return;
	}
}
         */
        private const string loadReadyScreenMethod = "method_39";
        /*
        // GClass1528
// Token: 0x06006CEC RID: 27884 RVA: 0x002DD388 File Offset: 0x002DB588
[CompilerGenerated]
private void method_61(bool local, GStruct101 weatherSettings, GStruct244 botsSettings, GStruct103 wavesSettings)
{
	if (this.SelectedLocation.Id == "laboratory")
	{
		wavesSettings.IsBosses = true;
	}
	this.bool_0 = local;
	this.gstruct244_0 = botsSettings;
	this.gstruct103_0 = wavesSettings;
	this.gstruct101_0 = weatherSettings;
	this.method_16(null, EMatchingType.Single).HandleExceptions();
}
        */
        private const string readyMethod = "method_61";

        public LoadOfflineRaidScreen() : base(transpiler: nameof(PatchTranspiler)){}

        protected override MethodBase GetTargetMethod()
        {
            /*
             search for Class816 but actually its Class1010
             so i made it so it searches for that thing automatickly less burden later on ...
            */
            return Constants.Instance.MenuControllerType.GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.IsNested && x.GetField("selectLocationScreenController", Constants.Instance.PublicInstanceFlag) != null)
                .GetMethod("method_2", Constants.Instance.NonPublicInstanceDeclaredOnlyFlag);
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
                Constants.Instance.MainApplicationType,
                $"{Constants.Instance.MainApplicationType.Name.ToLower()}_0",
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
                Constants.Instance.MainApplicationType,
                $"{Constants.Instance.MenuControllerType.Name.ToLower()}_0",
                ClientAccesor.GetMainApp()) as MainMenuController;
            // if we get rid of that menu controller object from here we can get rid of the using of gclass which will lessen the fuckery :)
            // also same goes for field accessing in this function.
            //Debug.LogError("Traverse: " + Traverse.Create(MenuControllerObject).Field("SelectedLocation").Field("Id").GetValue());
            // check if we can use Traverse...

            // will be great to rewrite this in more reflection version so we can disband the use of "using MainMenuController = GClass1504;"
            if (MenuControllerObject.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            PrivateValueAccessor.SetPrivateFieldValue(Constants.Instance.MenuControllerType, "bool_0", MenuControllerObject, local);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.Instance.MenuControllerType, $"{typeof(BotsSettings).Name.ToLower()}_0", MenuControllerObject, botsSettings);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.Instance.MenuControllerType, $"{typeof(WeatherSettings).Name.ToLower()}_0", MenuControllerObject, weatherSettings);
            PrivateValueAccessor.SetPrivateFieldValue(Constants.Instance.MenuControllerType, $"{typeof(WavesSettings).Name.ToLower()}_0", MenuControllerObject, wavesSettings);

            Constants.Instance.MenuControllerType.GetMethod(loadReadyScreenMethod, Constants.Instance.NonPublicInstanceFlag).Invoke(MenuControllerObject, null);
        }

    }
}
