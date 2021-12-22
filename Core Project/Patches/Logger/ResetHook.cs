using JET.Utility.Logger;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Logger
{
    class ResetHook : GenericPatch<ResetHook>
    {
        public ResetHook() : base(postfix: nameof(PatchPostifx)) { }
        protected override MethodBase GetTargetMethod()
        {
            return Constants.TargetAssemblyTypes
                .First(x => x.IsClass && x.GetProperty("UnityDebugLogsEnabled") != null)
                .GetNestedTypes(Constants.NonPublicFlag)
                .First(x => x.GetMethod("Release", Constants.PublicInstanceFlag) != null)
                .GetConstructors(Constants.NonPublicInstanceFlag)
                .First();
        }

        static void PatchPostifx() => UnityEngine.Debug.unityLogger.logHandler = UnityLogger.Instance;
    }
}
