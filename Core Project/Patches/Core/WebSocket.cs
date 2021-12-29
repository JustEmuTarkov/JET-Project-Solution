using JET.Utility.Patching;
using System;
using System.Linq;
using System.Reflection;

//x == typeof(GInterface141)
namespace JET.Patches.Core
{
    /// <summary>
    /// Overrides the websocket from "wss:" to "ws:"
    /// </summary>
    class WebSocket : GenericPatch<WebSocket>
    {
        public WebSocket() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            var targetInterface = Constants.Instance.TargetAssemblyTypes.Single(x => x.GetMethod("SetRequestHeader", BindingFlags.NonPublic | BindingFlags.Instance) != null && x.IsInterface);
            var typeThatMatches = Constants.Instance.TargetAssemblyTypes.Single(x => targetInterface.IsAssignableFrom(x) && x.IsAbstract && !x.IsInterface);
            return typeThatMatches.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.ReturnType == typeof(Uri));
        }

        private static Uri PatchPostfix(Uri __instance)
        {
            return new Uri(__instance.ToString().Replace("wss:", "ws:"));
        }
    }
}
