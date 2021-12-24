using System.Reflection;
using JET.Utility.Patching;
using JET.Utility.Reflection;

namespace SinglePlayerMod.Patches.Raid
{
    class OnShellEjectEventFix : GenericPatch<OnShellEjectEventFix>
    {
        public OnShellEjectEventFix() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod() => Constants.FirearmControllerType
            .GetMethod("OnShellEjectEvent");

        static bool PatchPrefix(object __instance)
        {
            var weaponController = PrivateValueAccessor.GetPrivateFieldValue(
                Constants.FirearmControllerType,
                Constants.WeaponControllerFieldName,
                __instance);
            return (weaponController.GetType().GetField("RemoveFromChamberResult").GetValue(weaponController) == null) ? false : true;
        }
    }
}
