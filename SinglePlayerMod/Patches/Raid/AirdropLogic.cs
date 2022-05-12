using Comfort.Common;
using EFT;
using JET.Utility;
using JET.Utility.Patching;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    class AirdropLogic : GenericPatch<AirdropLogic>
    {
        // move the request and airdrop config somewhere where it will be more reliable not in patch...
        private static Request m_request;
        public static Request request { 
            get 
            {
                if (m_request == null)
                {
                    m_request = new Request(null, ClientAccesor.BackendUrl);
                }
                return m_request;
            } 
        }
        private static Airdrop.Config m_airdropConfig;
        public static Airdrop.Config AirdropConfig { 
            get 
            {
                if (m_airdropConfig == null) 
                {
                    var json = request.GetJson("/singleplayer/airdrop/config");
                    m_airdropConfig = JsonConvert.DeserializeObject<Airdrop.Config>(json);
                }
                return m_airdropConfig;
            }
        }
        public static int height = 0;

        public AirdropLogic() : base(prefix: nameof(PrefixPatch)) { }

        protected override MethodBase GetTargetMethod()
        {
            Type airdropLogicClass = Constants.Instance.TargetAssemblyTypes
                .Where(type => 
                        type.GetField("airdropSynchronizableObject_0", BindingFlags.NonPublic | BindingFlags.Instance) != null && 
                        type.GetMethod("method_17", BindingFlags.NonPublic | BindingFlags.Instance) != null).First();
            return airdropLogicClass.GetMethod("method_17", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static bool PrefixPatch(Vector3 point, ref float distance, RaycastHit raycastHit, LayerMask mask)
        {
            distance = UnityEngine.Random.Range(AirdropConfig.airdropMinOpenHeight, AirdropConfig.airdropMaxOpenHeight);
            return true;
        }
    }
}
