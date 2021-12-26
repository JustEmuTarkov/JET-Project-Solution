using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SinglePlayerMod.Patches.Other
{
	/// <summary>
	/// A mod that disables the Filters in InventoryLogic
	/// </summary>
    class NoFilters : GenericPatch<NoFilters>
	{
		public NoFilters() : base(prefix: nameof(PatchPrefix)) { }
		protected override MethodBase GetTargetMethod()
		{
			var type = Constants.TargetAssembly.GetTypes().Single(x => x.GetMethod("CheckItemFilter", BindingFlags.Public | BindingFlags.Static) != null
																  && x.GetMethod("CheckItemFilter", BindingFlags.Public | BindingFlags.Static).IsDefined(typeof(ExtensionAttribute), true));
			return type.GetMethod("smethod_0", BindingFlags.NonPublic | BindingFlags.Static);
		}

		public static bool PatchPrefix(ref bool __result)
		{
			__result = true;
			return false;
		}
	}
	// Method that is override:
	/*
private static bool smethod_0(ItemFilter[] filters, Item item, bool checkOnlyExclusive)
{
	if (filters == null)
	{
		return true;
	}
	if (filters.Length == 0)
	{
		return true;
	}
	IContainerCollection topLevelCollection;
	Item[] array = ((topLevelCollection = (item as IContainerCollection)) != null) ? topLevelCollection.GetAllItemsFromCollection(null).ToArray<Item>() : null;
	foreach (ItemFilter itemFilter in filters)
	{
		if (!(checkOnlyExclusive ? itemFilter.CheckItemExcludedFilter(item) : (itemFilter.CheckItemFilter(item) && itemFilter.CheckItemExcludedFilter(item))))
		{
			return false;
		}
		if (array != null && array.Length != 0)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (!itemFilter.CheckItemExcludedFilter(array[j]))
				{
					return false;
				}
			}
		}
	}
	return true;
}
     */
}
