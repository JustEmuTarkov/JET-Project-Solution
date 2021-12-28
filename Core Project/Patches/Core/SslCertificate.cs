using JET.Utility.Patching;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Core
{
	/// <summary>
	/// Overrides SSL certificate method response to always return true.
	/// </summary>
    class SslCertificate : GenericPatch<SslCertificate>
	{
		private string _ValidateCertificate = "ValidateCertificate";
		public SslCertificate() : base(prefix: nameof(PatchPrefix)) { }

		protected override MethodBase GetTargetMethod()
		{
			return Constants.Instance.TargetAssemblyTypes
				.Single(x => x.BaseType == Constants.Instance.UnityCertificateHandlerType)
				.GetMethod(_ValidateCertificate, Constants.Instance.NonPublicInstanceDeclaredOnlyFlag);
		}

		static bool PatchPrefix(ref bool __result)
		{
			__result = true;

			return false;
		}
	}
}
