using JET.Utility.Logger;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Logger
{
    class InitialHook : GenericPatch<InitialHook>
    {
        public InitialHook() : base(postfix: nameof(PatchPostifx)) { }

        protected override MethodBase GetTargetMethod() => Constants.MainApplicationConstructorInfo.First();

        static void PatchPostifx() => UnityEngine.Debug.unityLogger.logHandler = UnityLogger.Instance;
    }
}
