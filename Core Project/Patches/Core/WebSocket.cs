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
            var targetInterface = Constants.Instance.TargetAssemblyTypes
                .Single(
                x => 
                x.GetProperty("TransportType", BindingFlags.Public) != null
                //x.GetMethod("SetRequestHeader", BindingFlags.NonPublic | BindingFlags.Instance) != null
                && x.IsInterface);
            var typeThatMatches = Constants.Instance.TargetAssemblyTypes.Single(x => targetInterface.IsAssignableFrom(x) && x.IsAbstract && !x.IsInterface);
            var wsMethod = typeThatMatches.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(x => x.ReturnType == typeof(Uri));

            if (wsMethod == null)
                UnityEngine.Debug.LogError("WebSocket Patch:: Cannot find WebSocket TargetMethod");
            else
            {
                UnityEngine.Debug.LogError("WebSocket Patch:: WebSocket TargetMethod ::" + typeThatMatches.FullName + "." + wsMethod.Name + "");
            }

            return wsMethod;
        }

        private static Uri PatchPostfix(Uri __instance)
        {
            return new Uri(__instance.ToString().Replace("wss:", "ws:"));
        }
    }
}
