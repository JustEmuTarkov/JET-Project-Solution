using JET.Utility.Patching;
using System.Reflection;

namespace SinglePlayerMod.Patches.MatchMaker
{
    /// <summary>
    /// This Patch Disables the Ready button after you select the location so you will not be jumping to online match by default
    /// Game needs to initialize the offline match variables first unfortunatly
    /// </summary>
    class DisableReadyButtonOnFirstScreen : GenericPatch<DisableReadyButtonOnFirstScreen>
    {
        public DisableReadyButtonOnFirstScreen() : base(postfix: nameof(PatchPostfix))
        {
        }
        public static void PatchPostfix(ref EFT.UI.DefaultUIButton ____readyButton)
        {
            ____readyButton.gameObject.SetActive(false);
        }

        protected override MethodBase GetTargetMethod()
        {
            foreach (var method in typeof(EFT.UI.Matchmaker.MatchMakerSelectionLocationScreen).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                return method?.SetMethod; // there is only 1 here so lets just return first one...
            }
            return null;
            //return typeof(MatchMakerSelectionLocationScreen).GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Instance)?.SetMethod; 
        }
    }
}
