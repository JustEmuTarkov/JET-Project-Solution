using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JET.Patches.Core
{
	/// <summary>
	/// Patches the RunValidation method to not execute any of its code and set variable Succeed to true
	/// </summary>
    class BattlEye : GenericPatch<BattlEye>
	{
		public static PropertyInfo __property;

		private string _MethodName = "RunValidation"; // method to patch
		private string _FieldName = "Succeed"; // variable to override

		public BattlEye() : base(prefix: nameof(PatchPrefix)) { }

		protected override MethodBase GetTargetMethod()
		{
			System.Type __type = Constants.TargetAssemblyTypes.Single(x => x.GetMethod(_MethodName, Constants.PublicInstanceFlag) != null);

			__property = __type.GetProperty(_FieldName, Constants.PublicInstanceFlag);

			return __type.GetMethod(_MethodName, Constants.PublicInstanceFlag);
		}

		private static bool PatchPrefix(ref Task __result, object __instance)
		{
			__property.SetValue(__instance, true);
			__result = Task.CompletedTask;
			return false;
		}
	}
}
