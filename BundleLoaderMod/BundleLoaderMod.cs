/*
 * Original Creator: AppeazeTheCheese
 * Mutual Creators: TheMaoci
 */

using JET.Utility.Modding;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using BundleLoader.Patches;

namespace BundleLoader
{
    public class BundleLoaderMod : JetMod
    {
        protected override void Initialize(IReadOnlyDictionary<Type, JetMod> dependencies, string gameVersion)
        {
            JET.Utility.Patching.HarmonyPatch.Patch<Patches.BundleLoad>();
            //JET.Utility.Patching.HarmonyPatch.Patch<Patches.EasyAssets>();
            //JET.Utility.Patching.HarmonyPatch.Patch<Patches.EasyBundle>();
        }
    }

}
