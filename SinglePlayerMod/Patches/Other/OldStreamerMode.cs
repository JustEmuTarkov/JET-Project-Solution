using HarmonyLib;
using JET.Utility.Patching;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SinglePlayerMod.Patches.Other
{
    /// <summary>
    /// This patch enables the method <PreloaderUI>().Instance.SetStreamMode(bool). WHich disables rtt and loss packets from FPS counter also disabling "Alpha text"
    /// </summary>
    class OldStreamerMode : GenericPatch<OldStreamerMode>
    {
        public OldStreamerMode() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.TargetAssembly.GetTypes().Single(x => x.GetMethod("SetStreamMode") != null).GetMethod("SetStreamMode");
        }

        static IEnumerable<CodeInstruction> PatchTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Ldc_I4_0);
            int Index = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    codes[i].opcode = OpCodes.Ldarg_1;
                    Index++;
                }
            }

            // Patch failed
            if (Index < 3)
            {
                JET.Utility.Logger.Patch.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }
            return codes.AsEnumerable();
        }
    }
}
