﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT;
using HarmonyLib;
//using JET.Utility;
using JET.Utility.Patching;
using JET.Utility.Reflection.CodeWrapper;
using UnityEngine;

namespace SinglePlayerMod.Patches.ScavMode
{
    class ScavExfilFix : GenericPatch<ScavExfilFix>
    {
        public ScavExfilFix() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.LocalGameType.BaseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance).Single(IsTargetMethod);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Constants.ExfilPointManagerType, "EligiblePoints", new System.Type[] { typeof(Profile) }));
            var searchIndex = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed.
            if (searchIndex == -1)
            {
                JET.Utility.Logger.Patch.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }

            searchIndex -= 3;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();
            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Info"),
                new Code(OpCodes.Ldfld, Constants.ProfileInfoType, "Side"),
                new Code(OpCodes.Ldc_I4_4),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, Constants.LocalGameType.BaseType, "gparam_0"),
                new Code(OpCodes.Box, typeof(PlayerOwner)),
                new Code(OpCodes.Callvirt, typeof(PlayerOwner), "get_Player"),
                new Code(OpCodes.Callvirt, typeof(Player), "get_Position"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "ScavExfiltrationClaim", new System.Type[]{ typeof(Vector3), typeof(string) }),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "GetScavExfiltrationMask"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "ScavExfiltrationClaim", new System.Type[]{ typeof(int), typeof(string) }),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Call, brFalseLabel, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "EligiblePoints", new System.Type[]{ typeof(Profile) }),
                new CodeWithLabel(OpCodes.Stloc_0, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            return methodInfo.IsVirtual && methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(void) && methodInfo.GetMethodBody().LocalVariables.Count > 0;
        }
    }
}
