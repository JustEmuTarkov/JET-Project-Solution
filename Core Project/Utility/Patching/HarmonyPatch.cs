using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace JET.Utility.Patching
{
    class HarmonyPatch
    {
        private static Harmony harmony;

        static HarmonyPatch()
        {
            harmony = new Harmony("JET.Harmony");
        }

        public static MethodInfo GetOriginalMethod<T>(string methodName)
        {
            return AccessTools.Method(typeof(T), methodName);
        }

        public static void PatchPrefix<T>() where T : AbstractPatch, new()
        {
            harmony.Patch(new T().TargetMethod(), prefix: new HarmonyMethod(typeof(T).GetMethod("Prefix")));
            Debug.Log("[JET]: Applied prefix patch " + typeof(T).Name);
        }

        public static void PatchPostfix<T>() where T : AbstractPatch, new()
        {
            harmony.Patch(new T().TargetMethod(), postfix: new HarmonyMethod(typeof(T).GetMethod("Postfix")));
            Debug.Log("[JET]: Applied postfix patch " + typeof(T).Name);
        }

        public static void Patch<T>() where T : GenericPatch<T>, new()
        {
            try
            {
                var patch = new T();
                if (patch.TargetMethod == null)
                    throw new InvalidOperationException("TargetMethod is null");

                harmony.Patch(patch.TargetMethod,
                              prefix: patch.Prefix.ToHarmonyMethod(),
                              postfix: patch.Postfix.ToHarmonyMethod(),
                              transpiler: patch.Transpiler.ToHarmonyMethod(),
                              finalizer: patch.Finalizer.ToHarmonyMethod());
                Debug.LogError("[JET]: Applied patch " + typeof(T).Name);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JET]: Error in patch {typeof(T).Name}{Environment.NewLine}{ex}");
            }
        }
    }
}
