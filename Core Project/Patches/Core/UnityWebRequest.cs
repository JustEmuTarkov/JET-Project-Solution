using JET.Utility.Patching;
using System.Reflection;
using UnityEngine.Networking;

namespace JET.Patches.Core
{
    /// <summary>
    /// Overrides Unity web request to allow any SSL Certificate
    /// </summary>
    class UnityWebRequest : GenericPatch<UnityWebRequest>
    {
        private static readonly CertificateHandler _certificateHandler = new FakeCertificateHandler();

        public UnityWebRequest() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.UnityUnityWebRequestType.GetMethod(nameof(UnityWebRequestTexture.GetTexture), new[] { typeof(string) });
        }

        static void PatchPostfix(UnityEngine.Networking.UnityWebRequest __result)
        {
            __result.certificateHandler = _certificateHandler;
            __result.disposeCertificateHandlerOnDispose = false;
            __result.timeout = 1000;
        }

        internal class FakeCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }
    }
}
