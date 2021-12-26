using EFT;
using HarmonyLib;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotData = GInterface18; // find ChooseProfile and get ginterface off that

namespace SinglePlayerMod.Patches.Raid
{
    class RemoveUsedBotProfile : GenericPatch<RemoveUsedBotProfile>
    {
        private static Type targetInterface;
        private static Type targetType;
        private static AccessTools.FieldRef<object, List<Profile>> profilesField;

        public RemoveUsedBotProfile() : base(prefix: nameof(PatchPrefix))
        {
            // compile-time check
            _ = nameof(BotData.ChooseProfile);

            targetInterface = Constants.TargetAssembly.GetTypes().Single(IsTargetInterface);
            targetType = Constants.TargetAssembly.GetTypes().Single(IsTargetType);
            profilesField = AccessTools.FieldRefAccess<List<Profile>>(targetType, "list_0");
        }

        private static bool IsTargetInterface(Type type)
        {
            if (!type.IsInterface || type.GetProperty("StartProfilesLoaded") == null || type.GetMethod("CreateProfile") == null)
            {
                return false;
            }

            return true;
        }

        private bool IsTargetType(Type type)
        {
            if (!targetInterface.IsAssignableFrom(type) || !targetInterface.IsAssignableFrom(type.BaseType))
            {
                return false;
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return targetType.GetMethod("GetNewProfile", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static bool PatchPrefix(ref Profile __result, object __instance, BotData data)
        {
            var profiles = profilesField(__instance);

            if (profiles.Count > 0)
            {
                // second parameter makes client remove used profiles
                __result = data.ChooseProfile(profiles, true);
            }
            else
            {
                __result = null;
            }

            return false;
        }
    }
}
