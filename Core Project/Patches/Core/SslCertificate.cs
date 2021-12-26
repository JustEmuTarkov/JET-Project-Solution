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
			return Constants.TargetAssemblyTypes
				.Single(x => x.BaseType == Constants.UnityCertificateHandlerType)
				.GetMethod(_ValidateCertificate, Constants.NonPublicInstanceDeclaredOnlyFlag);
		}

		static bool PatchPrefix(ref bool __result)
		{
			__result = true;

			return false;
		}
	}
}
