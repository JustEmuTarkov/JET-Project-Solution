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
            // this mod is still in progress and is not finished !!!!

            HarmonyPatch.Patch<BundleLoad>();
            HarmonyPatch.Patch<EasyAssets>();
            HarmonyPatch.Patch<EasyBundle>();
        }
    }

}
