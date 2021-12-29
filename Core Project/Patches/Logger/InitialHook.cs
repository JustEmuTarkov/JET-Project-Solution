using JET.Utility.Logger;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Logger
{
    /// <summary>
    /// Initial hook of the ingame logger - overrides instance of logger with custom one
    /// </summary>
    class InitialHook : GenericPatch<InitialHook>
    {
        public InitialHook() : base(postfix: nameof(PatchPostifx)) { }

        protected override MethodBase GetTargetMethod() => Constants.Instance.MainApplicationConstructorInfo.First();

        static void PatchPostifx() => UnityEngine.Debug.unityLogger.logHandler = UnityLogger.Instance;
    }
}
