using System.Reflection;
using HarmonyLib;

namespace JET.Utility.Patching
{
    static class Extensions
    {
        // Extension for patches to return Harmony method or null depending on methodInfo
        public static HarmonyMethod ToHarmonyMethod(this MethodInfo methodInfo)
        {
            return methodInfo != null
                ? new HarmonyMethod(methodInfo)
                : null;
        }
    }
}
