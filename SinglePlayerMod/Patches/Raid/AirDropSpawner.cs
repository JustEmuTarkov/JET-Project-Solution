using Comfort.Common;
using EFT;
using JET.Utility;
using JET.Utility.Patching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    /*
     * Used CWX old bepinex code as reference: https://dev.sp-tarkov.com/CWX/AirDrops
     * Additionally using reflection and no GInterface references as much as possible
     * !TheMaoci!
     */
    class AirDropSpawner : GenericPatch<AirDropSpawner>
    {
        //private Request request;
        private static GameWorld gameWorld { get => Singleton<GameWorld>.Instance; }
        private static bool points;
        public AirDropSpawner() : base(postfix: nameof(PostfixPatch)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }
        public static void PostfixPatch()
        {
            points = LocationScene.GetAll<AirdropPoint>().Any();

            if (gameWorld != null && points)
            {
                gameWorld.GetOrAddComponent<Airdrop.HandleComponent>();
            }
        }
    }
}
