using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT.Interactive;
using HarmonyLib;
//using HarmonyLib;
using JET.Utility.Patching;
using JET.Utility.Reflection.CodeWrapper;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    class BotStationaryWeaponFix : GenericPatch<BotStationaryWeaponFix>
    {
        private static readonly string kMethodName = "CheckWantTakeStationary";

        public BotStationaryWeaponFix() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod() => Constants.TargetAssembly
            .GetTypes().Single(x => x.GetMethod(kMethodName) != null).GetMethod(kMethodName);

        static IEnumerable<CodeInstruction> PatchTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StationaryWeapon), "get_OperatorPosition"));
            int searchIndex = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed
            if (searchIndex == -1)
            {
                JET.Utility.Logger.Patch.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }

            // Look ahead and search for a bgt instruction (should be within the next 10 lines) and get its operand. We want the same label to jump to
            // for our code below.
            var jumpToLabel = default(Label);
            var labelFound = false;

            for (var i = searchIndex; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Bgt_S)
                {
                    jumpToLabel = (Label)codes[i].operand;
                    break;
                }

                labelFound = true;
            }

            if (!labelFound)
            {
                Debug.LogError("[BotStationaryWeaponPatch] Label not found.");
                return instructions;
            }

            // This is start of the instruction that we are interested in.
            searchIndex -= 2;

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>() {
                new Code(OpCodes.Ldloc_3),
                new Code(OpCodes.Ldfld, typeof(StationaryWeaponLink), "Weapon"),
                new Code(OpCodes.Ldnull),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brtrue_S, jumpToLabel)
            });

            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }
    }
}
