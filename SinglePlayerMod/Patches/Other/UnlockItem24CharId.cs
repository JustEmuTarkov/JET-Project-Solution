using HarmonyLib;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace SinglePlayerMod.Patches.Other
{
    /// <summary>
    /// This patch unlocks the 24 character ID's of items restriction for trader selling
    /// </summary>
    class UnlockItem24CharId : GenericPatch<UnlockItem24CharId>
    {
        public UnlockItem24CharId() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            foreach (var MyType in Constants.Instance.TargetAssembly.GetTypes())
            {

                if (MyType.FullName.Split('+').Length == 3 && MyType.FullName.StartsWith("Class")) // make sure its 3 classes deep
                {
                    if (!(new Regex(@"Class[0-9]{3}\+Class[0-9]{3}\+Class[0-9]{3}").Match(MyType.FullName)).Success)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(MyType.Namespace)) // make sure it doesnt have namespace
                    {
                        foreach (var m in MyType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            var paramInfo = m.GetParameters();
                            if (!m.Name.Contains("method_"))
                            {
                                continue;
                            }
                            // make sure return type is class<thing, thing, thing> there is 1 param named item and its type is TradingItemReference
                            if (m.ReturnType.Name.EndsWith("3") && paramInfo.Length == 1)
                            {
                                if (paramInfo[0].ParameterType.Name == "TradingItemReference")
                                {
                                    //Debug.LogError($"{m.Name} {m.ReturnType.Name} {MyType.FullName}");
                                    return m;
                                }
                            }
                        }
                    }
                }

            }
            return null;
        }

        static IEnumerable<CodeInstruction> PatchTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var codes = new List<CodeInstruction>(codeInstructions);
            // that should be the fastest way cause its at index 3 and we need to remov e3 instructions from there
            for (var i = 0; i < 3; i++)
                codes.RemoveAt(3);
            /* more advanced code if the first one will fail after some updates from bsg ...
            List<CodeInstruction> searchCode = new List<CodeInstruction> {
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4_S),
                new CodeInstruction(OpCodes.Callvirt)
            };
            FoundItemToDeleteReturning:
            if(searchCode.Count > 0)
                for (var i = 0; i < codes.Count; i++) {
                    if (codes[i].opcode == searchCode[0].opcode) {
                        searchCode.RemoveAt(0);
                        codes.RemoveAt(i);
                        goto FoundItemToDeleteReturning;
                    }
                }

            if (searchCode.Count > 0){
                Debug.LogError("Patch Failed!! too much codes to search should be 0...");
                PatchLogger.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }
            */
            if (codes.Count == 9) return codes.AsEnumerable();
            Debug.LogError($"Patch Failed!! strange number of opcodes {codes.Count} [originalCode count is: {codeInstructions.ToList().Count}]");
            JET.Utility.Logger.Patch.LogTranspileSearchError(MethodBase.GetCurrentMethod());
            return codeInstructions;
        }
    }
}
