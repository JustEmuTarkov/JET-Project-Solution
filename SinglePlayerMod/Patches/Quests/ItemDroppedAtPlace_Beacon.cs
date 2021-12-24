using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace SinglePlayerMod.Patches.Quests
{
    class ItemDroppedAtPlace_Beacon : GenericPatch<ItemDroppedAtPlace_Beacon>
    {
        public ItemDroppedAtPlace_Beacon() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod() => typeof(EFT.Player)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Single(IsTargetMethod);

        private bool IsTargetMethod(MethodInfo method)
        {
            if (!method.IsVirtual)
            {
                return false;
            }

            var parameters = method.GetParameters();

            if (parameters.Length != 2
            || parameters[0].ParameterType != typeof(EFT.InventoryLogic.Item)
            || parameters[0].Name != "item"
            || parameters[1].ParameterType != typeof(string)
            || parameters[1].Name != "zone")
            {
                return false;
            }

            return true;
        }

        public static bool PatchPrefix(EFT.Player __instance, EFT.InventoryLogic.Item item, string zone)
        {
            __instance.Profile.ItemDroppedAtPlace(item.TemplateId, zone);

            return false;
        }
    }
}
