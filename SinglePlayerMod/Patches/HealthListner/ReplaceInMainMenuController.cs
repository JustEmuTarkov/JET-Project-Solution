using MainMenuController = GClass1504; // SelectedDateTime
using IHealthController = GInterface195; // CarryingWeightAbsoluteModifier
using JET.Utility.Patching;
using System.Reflection;

namespace SinglePlayerMod.Patches.HealthListner
{
    class ReplaceInMainMenuController : GenericPatch<ReplaceInMainMenuController>
    {
        static ReplaceInMainMenuController()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        public ReplaceInMainMenuController() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utility.Progression.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
