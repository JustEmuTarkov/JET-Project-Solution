using EFT;
using HarmonyLib;
using JET.Utility.Patching;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid
{
    /// <summary>
    /// Fixes the problem of the tinnitus sound effect being played on the player if any AI on the map get the contusion effect applied to them
    /// The patch adds an extra condition to the check before playing the sound effect, making sure the sound is only played if contusion occurred on the player
    /// </summary>
    class TinnitusFix : GenericPatch<TinnitusFix>
    {
        public TinnitusFix() : base(postfix: nameof(PatchTranspiler)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnHealthEffectAdded", BindingFlags.Instance | BindingFlags.Public);
        }
        /// <summary>
        /// Edits the Opcodes of actual method gathered by GetTargetMethod
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="instructions"></param>
        /// <returns></returns>
        private static IEnumerable<CodeInstruction> PatchTranspiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BetterAudio), "StartTinnitusEffect"));

            // Locate the reference instruction from which we can locate all the other relevant instructions
            var searchIndex = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            if (searchIndex == -1)
            {
                Debug.LogError($"Patch {nameof(TinnitusFix)} failed: Could not find reference code.");
                return instructions;
            }

            // The next instruction after our reference point should be a 'br' with the condition exit label
            if (codes[searchIndex + 1].opcode != OpCodes.Br)
            {
                Debug.LogError($"Patch {nameof(TinnitusFix)} failed: Could not locate 'br' instruction");
                return instructions;
            }

            // We grab the target label that we can use to exit the condition if it's not satisfied
            var skipLabel = (Label)codes[searchIndex + 1].operand;

            // Locate the index at which our instructions should be inserted
            var insertIndex = -1;
            for (var i = searchIndex; i > searchIndex - 10; i--)
            {
                if (codes[i].opcode == OpCodes.Brtrue)
                {
                    insertIndex = i + 1;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                Debug.LogError($"Patch {nameof(TinnitusFix)} failed: Could not find instruction insert location.");
            }

            // Add a new condition that checks if your player is the one who has the contusion effect applied
            var newCodes = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), "get_IsYourPlayer")),
                new CodeInstruction(OpCodes.Brfalse, skipLabel)
            };

            // edits the method called: public virtual void OnHealthEffectAdded(GInterface149 effect)
            // Original code line:
            // else if (@class.effect is GInterfaceXXX && !(this.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem is GClassXXXX))
            // Updated code line:
            // else if (@class.effect is GInterfaceXXX && !(this.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem is GClassXXXX) && this.IsYourPlayer)


                codes.InsertRange(insertIndex, newCodes);

            return codes;
        }
    }
}