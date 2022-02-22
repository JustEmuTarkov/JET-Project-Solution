/*
 * Original Creator: AppeazeTheCheese
 * Mutual Creators: TheMaoci
 */

using JET.Utility.Modding;
using System;
using System.Collections.Generic;

namespace BundleLoaderMod
{
    public class Initializator : JetMod
    {
        protected override void Initialize(IReadOnlyDictionary<Type, JetMod> dependencies, string gameVersion)
        {
            // this mod is still in progress and is not finished !!!!

            //JET.Utility.Patching.HarmonyPatch.Patch<Patches.DisplayMoneyPanel>();
        }
        public static string OriginalCreator = "AppeazeTheCheese";
        public static string MutualCreators = "AppeazeTheCheese,TheMaoci";
    }

}
