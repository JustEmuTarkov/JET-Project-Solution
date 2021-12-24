using Comfort.Common;
using JET.Utility;
using JET.Utility.Patching;
using System;
using System.Reflection;

namespace SinglePlayerMod.Patches.Progression
{
    class OfflineSaveProfile : GenericPatch<OfflineSaveProfile>
    {

        public OfflineSaveProfile() : base(prefix: nameof(PatchPrefix))
        {
            // compile-time check
            //_ = nameof(ClientMetrics.Metrics);
        }

        protected override MethodBase GetTargetMethod()
        {
            foreach (var method in Constants.MainApplicationType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                //if (method.Name == "method_44") {
                //    Debug.LogError($"{method.GetParameters().Length} {method.GetParameters()[0].ParameterType.Name} {method.GetParameters()[3].Name} {method.GetParameters()[3].ParameterType.Name}");
                //}
                if (method.Name.StartsWith("method") &&
                    method.GetParameters().Length == 6 &&
                    method.GetParameters()[0].ParameterType.Name == "String" &&
                    method.GetParameters()[3].Name == "isLocal" &&
                    method.GetParameters()[3].ParameterType.Name == "Boolean")
                {
                    return method;
                }
            }
            return null;
        }

        public static void PatchPrefix(ESideType ___esideType_0, Result<EFT.ExitStatus, TimeSpan, object> result)
        {
            var session = ClientAccesor.GetClientApp().GetClientBackEndSession();
            var isPlayerScav = false;
            var profile = session.Profile;

            if (___esideType_0 == ESideType.Savage)
            {
                profile = session.ProfileOfPet;
                isPlayerScav = true;
            }

            var currentHealth = Utility.Progression.HealthListener.Instance.CurrentHealth;

            Utility.Progression.SaveLootUtil.SaveProfileProgress(ClientAccesor.BackendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
        }
    }
}
