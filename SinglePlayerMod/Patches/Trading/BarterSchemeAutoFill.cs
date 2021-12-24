using System.Linq;
using System.Reflection;
using JET.Utility.Patching;
using UnityEngine;

namespace SinglePlayerMod.Patches.Trading
{
    class BarterSchemeAutoFill : GenericPatch<BarterSchemeAutoFill>
    {
        public BarterSchemeAutoFill() : base(postfix: nameof(PatchPostfix)) { }
        public static void PatchPostfix(ref bool ___bool_0)
        {
            ___bool_0 = true;
            PlayerPrefs.SetInt("AutoFillRequirements", 1);
        }
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.BarterSchemePanel).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
    class BarterSchemeAutoFillPersist : GenericPatch<BarterSchemeAutoFillPersist>
    {
        public BarterSchemeAutoFillPersist() : base(prefix: nameof(PatchPrefix)) { }
        public static bool PatchPrefix(ref bool ___bool_0)
        {
            ___bool_0 = true;
            PlayerPrefs.SetInt("AutoFillRequirements", 1);
            PlayerPrefs.Save();
            return true;
        }
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.BarterSchemePanel).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == "method_6");
        }
    }
}
