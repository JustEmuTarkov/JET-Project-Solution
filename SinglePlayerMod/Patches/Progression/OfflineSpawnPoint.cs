//using JET.Utility.Patching;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;

//using ISpawnPoints = GInterface244;

//namespace SinglePlayerMod.Patches.Progression
//{
//    class OfflineSpawnPoint : GenericPatch<OfflineSpawnPoint>
//    {
//        public OfflineSpawnPoint() : base(prefix: nameof(PatchPrefix)) { }

//        protected override MethodBase GetTargetMethod()
//        {
//            var targetType = Constants.Instance.TargetAssembly.GetTypes().First(IsTargetType);
//            return targetType.GetMethods(Constants.Instance.DefaultBindingFlags).First(m => m.Name.Contains("SelectSpawnPoint"));
//        }

//        private static bool IsTargetType(Type type)
//        {
//            var methods = type.GetMethods(Constants.Instance.DefaultBindingFlags);

//            if (!methods.Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1))
//                return false;

//            return !type.IsInterface;
//        }

//        /// <summary>
//        /// Patches ingame spawn point selection function
//        /// </summary>
//        /// <param name="__result"></param>
//        /// <param name="___ginterface240_0"> make sure to properly name this variable cause its used by harmony to get it on runtime</param>
//        /// <param name="category"></param>
//        /// <param name="side"></param>
//        /// <param name="infiltration"></param>
//        /// <returns></returns>
//        public static bool PatchPrefix(ref EFT.Game.Spawning.ISpawnPoint __result, ISpawnPoints ___ginterface244_0, EFT.Game.Spawning.ESpawnCategory category, EFT.EPlayerSide side, string infiltration)
//        {
//            var spawnPoints = ___ginterface244_0.ToList();

//            var unfilteredSpawnPoints = spawnPoints.ToList();
//            var infils = spawnPoints.Select(sp => sp.Infiltration).Distinct();
//            Debug.LogError($"PatchPrefix SelectSpawnPoint Infiltrations: {spawnPoints.Count} | {String.Join(", ", infils)}");

//            spawnPoints = spawnPoints.Where(sp => sp != null && sp.Infiltration != null && (String.IsNullOrEmpty(infiltration) || sp.Infiltration.Equals(infiltration))).ToList();
//            if (spawnPoints.Count == 0)
//            {
//                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
//                return false;
//            }

//            spawnPoints = spawnPoints.Where(sp => sp.Categories.Contain(category)).ToList();
//            if (spawnPoints.Count == 0)
//            {
//                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
//                return false;
//            }

//            spawnPoints = spawnPoints.Where(sp => sp.Sides.Contain(side)).ToList();
//            if (spawnPoints.Count == 0)
//            {
//                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
//                return false;
//            }

//            __result = spawnPoints.RandomElement();
//            return false;
//        }

//        private static EFT.Game.Spawning.ISpawnPoint GetFallBackSpawnPoint(List<EFT.Game.Spawning.ISpawnPoint> spawnPoints, EFT.Game.Spawning.ESpawnCategory category, EFT.EPlayerSide side, string infiltration)
//        {
//            var spawn = spawnPoints.Where(sp => sp.Categories.Contain(EFT.Game.Spawning.ESpawnCategory.Player)).RandomElement();
//            Debug.LogError($"PatchPrefix SelectSpawnPoint [Id: {spawn.Id}]: Couldn't find any spawn points for:  {category}  |  {side}  |  {infiltration}");
//            return spawn;
//        }
//    }
//}
