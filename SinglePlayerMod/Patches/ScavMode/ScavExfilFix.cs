using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT;
using HarmonyLib;
using JET.Utility.Patching;
using JET.Utility.Reflection.CodeWrapper;
using UnityEngine;

namespace SinglePlayerMod.Patches.ScavMode
{
    class ScavExfilFix : GenericPatch<ScavExfilFix>
    {

        public ScavExfilFix() : base(transpiler: nameof(PatchTranspile)) {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.Instance.ExfilPointManagerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance)
                .Single(methodInfo => 
                    methodInfo.IsVirtual && methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(void) && methodInfo.GetMethodBody().LocalVariables.Count > 0);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(Constants.Instance.ExfilPointManagerType, "EligiblePoints", new Type[] { typeof(Profile) }));
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
                Debug.LogError(string.Format("Patch {0} failed: Could not find reference code.", MethodBase.GetCurrentMethod()));
                return instructions;
            }

            searchIndex -= 3;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();
            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Info"),
                new Code(OpCodes.Ldfld, Constants.Instance.ProfileInfoType, "Side"),
                new Code(OpCodes.Ldc_I4_4),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Call, Constants.Instance.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, Constants.Instance.LocalGameType.BaseType, "gparam_0"),
                new Code(OpCodes.Box, typeof(PlayerOwner)),
                new Code(OpCodes.Callvirt, typeof(PlayerOwner), "get_Player"),
                new Code(OpCodes.Callvirt, typeof(Player), "get_Position"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Call, Constants.Instance.ProfileType, "get_FenceInfo"),
                new Code(OpCodes.Call, Constants.Instance.FenceTraderInfoType, "get_AvailableExitsCount"),
                new Code(OpCodes.Callvirt, Constants.Instance.ExfilPointManagerType, "ScavExfiltrationClaim", new Type[]{ typeof(Vector3), typeof(string), typeof(int) }),
                new Code(OpCodes.Call, Constants.Instance.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Call, Constants.Instance.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.Instance.ExfilPointManagerType, "GetScavExfiltrationMask"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.Instance.ExfilPointManagerType, "ScavExfiltrationClaim", new Type[]{ typeof(int), typeof(string) }),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Call, brFalseLabel, Constants.Instance.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.Instance.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Callvirt, Constants.Instance.ExfilPointManagerType, "EligiblePoints", new Type[]{ typeof(Profile) }),
                new CodeWithLabel(OpCodes.Stloc_2, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }

    }
}
