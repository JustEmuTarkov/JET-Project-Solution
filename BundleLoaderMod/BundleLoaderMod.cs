/*
 * Creator: TheMaoci
 */

using JET.Utility.Modding;
using System;
using System.Collections.Generic;

namespace BundleLoader
{
    public class BundleLoaderMod : JetMod
    {
        protected override void Initialize(IReadOnlyDictionary<Type, JetMod> dependencies, string gameVersion)
        {
            JET.Utility.Patching.HarmonyPatch.Patch<Patches.ManifestSetResults>();
        }
    }

}
