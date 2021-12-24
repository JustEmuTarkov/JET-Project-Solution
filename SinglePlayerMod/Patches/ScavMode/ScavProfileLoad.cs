using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFT;
using HarmonyLib;
using JET.Utility;
using JET.Utility.Patching;
using JET.Utility.Reflection.CodeWrapper;
using UnityEngine;

namespace SinglePlayerMod.Patches.ScavMode
{
    class ScavProfileLoad : GenericPatch<ScavProfileLoad>
    {
        public ScavProfileLoad() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            /* SHORTEN VERSION - Sometimes isnt working for unknown reason... (maybe only for me...) */
            /*
            var classType = PatcherConstants.MainApplicationType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(type =>
                type.Name.StartsWith("Struct") &&
                type.GetField("entryPoint") != null &&
                type.GetField("timeAndWeather") != null &&
                type.GetField("location") != null &&
                type.GetField("mainApplication_0") != null &&
                type.GetField("timeHasComeScreenController") != null).FirstOrDefault();

            var returned = classType.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (returned != null)
            {
                Debug.Log("[ScavProfileLoadPatch] Method Found: " + returned.Name + " " + classType.Name);
                return returned;
            } else {
                Debug.Log("[ScavProfileLoadPatch] Method Not Found in class " + classType.Name);
                return null;
            }
            */
            foreach (var type in Constants.MainApplicationType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (type.Name.StartsWith("Struct"))
                {
                    if (type.GetField("entryPoint") != null &&
                        type.GetField("timeAndWeather") != null &&
                        type.GetField("location") != null &&
                        type.GetField("mainApplication_0") != null &&
                        type.GetField("timeHasComeScreenController") != null)
                    {
                        var TargetedMethod = type.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        if (TargetedMethod != null)
                        {
                            Debug.Log("[ScavProfileLoadPatch] Method Found: " + TargetedMethod.Name + " " + type.Name);
                            return TargetedMethod;
                        }

                    }
                }
            }
            return null;

            //return typeof(MainApplication).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            //    .FirstOrDefault(IsTargetMethod);
        }
        //private static bool IsTargetMethod(MethodInfo methodInfo)
        //{
        //    var parameters = methodInfo.GetParameters();

        //    if (parameters.Length != 4
        //    || parameters[0].Name != "location"
        //    || parameters[1].Name != "timeAndWeather"
        //    || parameters[2].Name != "entryPoint"
        //    || parameters[3].Name != "timeHasComeScreenController"
        //    || parameters[2].ParameterType != typeof(string)
        //    || methodInfo.ReturnType != typeof(void))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where backend.Session.getProfile() is called.
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Constants.SessionInterfaceType, "get_Profile"));
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

            // Move back by 4. This is the start of this method call.
            // Note that we don't actually want to replace the code at searchIndex (which is a Ldloc0) since there is a branch
            // instruction prior to this instruction that leads to it and we can reuse a Ldloc0 instruction here.
            searchIndex -= 4;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();
            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, typeof(ClientApplication), "_backEnd"),
                new Code(OpCodes.Callvirt, Constants.BackendInterfaceType, "get_Session"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, typeof(MainApplication), "esideType_0"),
                new Code(OpCodes.Ldc_I4_0),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Callvirt, Constants.SessionInterfaceType, "get_Profile"),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Callvirt, brFalseLabel, Constants.SessionInterfaceType, "get_ProfileOfPet"),
                new CodeWithLabel(OpCodes.Stfld, brLabel, typeof(MainApplication).GetNestedTypes(BindingFlags.NonPublic).Single(IsTargetNestedType), "profile")
            });

            codes.RemoveRange(searchIndex + 1, 5);
            codes.InsertRange(searchIndex + 1, newCodes);

            return codes.AsEnumerable();
        }

        private static bool IsTargetNestedType(System.Type nestedType)
        {
            return nestedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Count(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string)) > 0 &&
                nestedType.GetField("savageProfile") != null;
        }

    }
}