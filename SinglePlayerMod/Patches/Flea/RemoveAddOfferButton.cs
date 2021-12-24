using System.Reflection;
using JET.Utility.Patching;
using UI_Button = EFT.UI.DefaultUIButton;

namespace SinglePlayerMod.Patches.Flea
{
    /// <summary>
    /// Overriding Awake Method
    /// </summary>
    class RemoveAddOfferButton_Awake : GenericPatch<RemoveAddOfferButton_Awake>
    {
        public RemoveAddOfferButton_Awake() : base(postfix: nameof(PatchPostfix))
        {
        }

        public static void PatchPostfix(ref UI_Button ____addOfferButton)
        {
            //____addOfferButton.gameObject.SetActive(false);
            ____addOfferButton.Interactable = false;
            ____addOfferButton.OnClick.RemoveAllListeners();
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Ragfair.RagfairScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
    /// <summary>
    /// Overriding a method that "tries" to enable button
    /// </summary>
    class RemoveAddOfferButton_Call : GenericPatch<RemoveAddOfferButton_Call>
    {
        public RemoveAddOfferButton_Call() : base(postfix: nameof(PatchPostfix))
        {
        }

        public static void PatchPostfix(ref UI_Button ____addOfferButton)
        {
            //____addOfferButton.gameObject.SetActive(false);
            ____addOfferButton.Interactable = false;
            ____addOfferButton.OnClick.RemoveAllListeners();
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Ragfair.RagfairScreen).GetMethod("method_6", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
