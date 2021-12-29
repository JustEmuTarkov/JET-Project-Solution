using JET.Utility.Logger;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Logger
{
    /// <summary>
    /// Restores the hook after attaching was already done.
    /// </summary>
    class ResetHook : GenericPatch<ResetHook>
    {
        public ResetHook() : base(postfix: nameof(PatchPostifx)) { }
        protected override MethodBase GetTargetMethod()
        {
            return Constants.Instance.TargetAssemblyTypes
                .First(x => x.IsClass && x.GetProperty("UnityDebugLogsEnabled") != null)
                .GetNestedTypes(Constants.Instance.NonPublicFlag)
                .First(x => x.GetMethod("Release", Constants.Instance.PublicInstanceFlag) != null)
                .GetConstructors(Constants.Instance.NonPublicInstanceFlag)
                .First();
        }

        static void PatchPostifx() => UnityEngine.Debug.unityLogger.logHandler = UnityLogger.Instance;
    }
}
