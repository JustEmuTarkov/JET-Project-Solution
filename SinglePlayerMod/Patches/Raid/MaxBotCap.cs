using JET.Utility;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace SinglePlayerMod.Patches.Raid
{
    /// <summary>
    /// This patch enables the ability to change max cap of bots in match from 20 to whatever you desire, if not set or error occured it will be set to 20
    /// </summary>
    class MaxBotCap : GenericPatch<MaxBotCap>
    {
        public MaxBotCap() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var methodName = "SetSettings";
            return Constants.Instance.TargetAssemblyTypes.Single(x => x.GetMethod(methodName, flags) != null && IsTargetMethod(x.GetMethod(methodName, flags)))
                .GetMethod(methodName, flags);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length == 3
                && parameters[0].Name == "maxCount"
                && parameters[1].Name == "botPresets"
                && parameters[2].Name == "botScatterings";
        }

        public static bool PatchPrefix(ref int maxCount)
        {
            var json = new JET.Utility.Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/settings/bot/maxCap");
            var isParsable = int.TryParse(json, out maxCount);
            maxCount = isParsable ? maxCount : 20;
            return false;
        }
    }
}