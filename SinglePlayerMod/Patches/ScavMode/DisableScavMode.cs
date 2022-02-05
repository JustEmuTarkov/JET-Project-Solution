using JET.Utility.Patching;
using System.Reflection;
using UnityEngine;

namespace SinglePlayerMod.Patches.ScavMode
{
    /// <summary>
    /// Disables Scav selection... Simply moves PMC to the center and disables whole box container for scav
    /// </summary>
    class DisableScavMode : GenericPatch<DisableScavMode>
    {
        //static Vector3 PMCs_NewPosition = new Vector3(732.3394f, 540f, 0f); // position of PMC box inside UI (global position)

        static DisableScavMode() { }

        public DisableScavMode() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Matchmaker.MatchMakerSideSelectionScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void PatchPostfix(
            EFT.UI.Matchmaker.MatchMakerSideSelectionScreen __instance,
            UnityEngine.UI.Button ____savagesBigButton, 
            UnityEngine.UI.Button ____pmcBigButton)
        {
            ____savagesBigButton.transform.parent.gameObject.SetActive(false);
            ____pmcBigButton.transform.parent.transform.localPosition = new Vector3(-220, 500, 0);
            RectTransform tempRectTransform = ____pmcBigButton.GetComponent<RectTransform>();
            tempRectTransform.anchoredPosition = new Vector2(-220, 0);
            tempRectTransform.offsetMax = new Vector2(-220, 0);
            tempRectTransform.offsetMin = new Vector2(-220, 0);
            tempRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);

        }
    }
}
//MatchMakerSideSelectionScreen