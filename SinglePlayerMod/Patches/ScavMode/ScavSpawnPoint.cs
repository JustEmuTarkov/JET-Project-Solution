using HarmonyLib;
using JET.Utility.Patching;
using JET.Utility.Reflection.CodeWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SinglePlayerMod.Patches.ScavMode
{
    class ScavSpawnPoint : GenericPatch<ScavSpawnPoint>
    {
        public ScavSpawnPoint() : base(prefix: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.ExfilPointManagerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance)
                .Single(methodInfo => methodInfo.GetParameters().Length == 3 && methodInfo.ReturnType == typeof(void));

        }

        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(Constants.ExfilPointManagerType, "RemoveProfileIdFromPoints"));
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

            searchIndex += 1;

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_ScavExfiltrationPoints")
            });

            codes.RemoveRange(searchIndex, 23);
            codes.InsertRange(searchIndex, newCodes);
            return codes.AsEnumerable();
        }
    }
}
