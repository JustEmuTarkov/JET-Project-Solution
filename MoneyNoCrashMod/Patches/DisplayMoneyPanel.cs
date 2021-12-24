using JET.Utility.Patching;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MoneyNoCrashMod.Patches
{
    class DisplayMoneyPanel : GenericPatch<DisplayMoneyPanel>
	{
		public static PropertyInfo __property;

		private string _MethodName = "RunValidation"; // method to patch
		private string _FieldName = "Succeed"; // variable to override

		public DisplayMoneyPanel() : base(prefix: nameof(PatchPrefix)) { }

		protected override MethodBase GetTargetMethod()
		{
			return typeof(EFT.UI.DisplayMoneyPanel).GetMethod(_MethodName, BindingFlags.Public | BindingFlags.Instance);
		}

        private static bool PatchPrefix(
            IEnumerable<EFT.InventoryLogic.Item> inventoryItems,
            EFT.UI.DisplayMoneyPanel __instance,
            CustomTextMeshProUGUI ___roubles,
            CustomTextMeshProUGUI ___euros,
            CustomTextMeshProUGUI ___dollars)
        {
            __instance.ShowGameObject();
            var items = inventoryItems.SelectMany(new Func<Item, IEnumerable<Item>>(GetAllItems));
            var OnlyMoney = items.Where(new Func<EFT.InventoryLogic.Item, bool>(IsMoney));
            long CNT_ROUBLES = 0;
            long CNT_DOLLARS = 0;
            long CNT_EUROS = 0;
            foreach (var money in OnlyMoney)
            {
                switch (money.TemplateId)
                {
                    case "5449016a4bdc2d6f028b456f": // Roubles
                        CNT_ROUBLES += money.StackObjectsCount;
                        break;
                    case "5696686a4bdc2da3298b456a": // Dollars
                        CNT_DOLLARS += money.StackObjectsCount;
                        break;
                    case "569668774bdc2da2298b4568": // EUROS
                        CNT_EUROS += money.StackObjectsCount;
                        break;
                }
            }
            NumberFormatInfo provider = new NumberFormatInfo
            {
                NumberGroupSeparator = " "
            };
            ___roubles.text = CNT_ROUBLES.ToString("N0", provider);
            ___dollars.text = CNT_DOLLARS.ToString("N0", provider);
            ___euros.text = CNT_EUROS.ToString("N0", provider);
            return false;
        }

        internal static IEnumerable<Item> GetAllItems(Item item)
        {
            return item.GetAllItems(false);
        }
        internal static bool IsMoney(EFT.InventoryLogic.Item item)
        {
            return item.TemplateId == "5449016a4bdc2d6f028b456f" ||
                item.TemplateId == "5696686a4bdc2da3298b456a" ||
                item.TemplateId == "569668774bdc2da2298b4568";
        }

        /*
          // for transpiler patching
        internal static Dictionary<ECurrencyType, long> GetMoneySums_Patched(IEnumerable<Item> items) {
            Dictionary<ECurrencyType, long> MoneyDict = new Dictionary<ECurrencyType, long>() {
                { ECurrencyType.EUR, 0 },
                { ECurrencyType.RUB, 0 },
                { ECurrencyType.USD, 0 }
            };
            foreach (var money in items)
            {
                switch (money.TemplateId)
                {
                    case "5449016a4bdc2d6f028b456f": // Roubles
                        MoneyDict[ECurrencyType.RUB] += money.StackObjectsCount;
                        break;
                    case "5696686a4bdc2da3298b456a": // Dollars
                        MoneyDict[ECurrencyType.USD] += money.StackObjectsCount;
                        break;
                    case "569668774bdc2da2298b4568": // EUROS
                        MoneyDict[ECurrencyType.EUR] += money.StackObjectsCount;
                        break;
                }
            }
            return MoneyDict;
        }
         */

    }
}
