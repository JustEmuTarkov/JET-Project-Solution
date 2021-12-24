using JET.Utility.Patching;
using JET.Utility.Reflection;
using System.Linq;
using System.Reflection;

namespace SinglePlayerMod.Patches.ScavMode
{
    class ScavSpawnPoint : GenericPatch<ScavSpawnPoint>
    {
        public ScavSpawnPoint() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.TargetAssembly.GetTypes()
                .FirstOrDefault(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(y => y.Name).Contains("SelectFarthestFromOtherPlayers"))
                .GetNestedTypes(BindingFlags.NonPublic).FirstOrDefault(x => x.GetField("infiltrationZone") != null)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(EFT.Game.Spawning.ISpawnPoint)/*typeof(BotZone.SpawnAreaSettings)*/);
        }

        /*
// Class1808.Class1810
// Token: 0x06009F97 RID: 40855 RVA: 0x001028C2 File Offset: 0x00100AC2
internal bool method_2(ISpawnPoint spawnPoint)
{
	return spawnPoint.smethod_1(this.side);
}
         */

        static bool PatchPrefix(ref bool __result, object __instance)
        {
            var playerSide = (EFT.EPlayerSide)PrivateValueAccessor.GetPrivateFieldValue(__instance.GetType(), "side", __instance);

            if (playerSide == EFT.EPlayerSide.Savage)
            {
                __result = true;

                return false;
            }

            return true;
        }
    }
}
