﻿using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SinglePlayerMod.Patches.MatchMaker
{
    class BringBackInsuranceScreen : GenericPatch<BringBackInsuranceScreen>
    {
        public BringBackInsuranceScreen() : base(prefix: nameof(PrefixPatch), postfix: nameof(PostfixPatch)) { }

        // don't forget 'ref'
        static void PrefixPatch(ref bool local)
        {
            local = false;
        }

        static void PostfixPatch(ref bool ___bool_0)
        {
            ___bool_0 = true;
        }

        protected override MethodBase GetTargetMethod()
        {
            // find method 
            // private void method_53(bool local, GStruct73 weatherSettings, GStruct177 botsSettings, GStruct74 wavesSettings)
            return Constants.MenuControllerType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(IsTargetMethod);    // controller contains 2 methods with same signature. Usually target method is first of them.
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();

            if (parameters.Length != 4
            || parameters[0].ParameterType != typeof(bool)
            || parameters[0].Name != "local"
            || parameters[1].Name != "weatherSettings"
            || parameters[2].Name != "botsSettings"
            || parameters[3].Name != "wavesSettings")
            {
                return false;
            }

            return true;
        }
    }
}