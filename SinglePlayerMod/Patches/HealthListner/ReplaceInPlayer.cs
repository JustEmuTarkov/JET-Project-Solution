using EFT;
using JET.Utility;
using JET.Utility.Patching;
using System.Reflection;
using System.Threading.Tasks;

namespace SinglePlayerMod.Patches.HealthListner
{
    class ReplaceInPlayer : GenericPatch<ReplaceInPlayer>
    {
        private static string _playerAccountId;

        public ReplaceInPlayer() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static async void PatchPostfix(Player __instance, Task __result)
        {
            if (_playerAccountId == null)
            {
                var backendSession = ClientAccesor.GetClientApp().GetClientBackEndSession();
                var profile = backendSession.Profile;
                _playerAccountId = profile.AccountId;
            }

            if (__instance.Profile.AccountId != _playerAccountId)
            {
                return;
            }

            await __result;

            var listener = Utility.Progression.HealthListener.Instance;
            listener.Init(__instance.HealthController, true);
        }
    }
}
