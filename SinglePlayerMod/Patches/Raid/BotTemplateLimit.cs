using System;
using System.Collections.Generic;
using JET.Utility.Patching;
using System.Reflection;
using EFT;
using JET.Utility;
using UnityEngine;
using WaveInfo = GClass1250; // search for: Difficulty and chppse gclass with lower number whic hcontains Role and Limit variables
using BotsPresets = GClass598; // Method: GetNewProfile (higher GClass number)
using System.Linq;
// Method: GetNewProfile (higher GClass number)

namespace SinglePlayerMod.Patches.Raid
{
    class BotTemplateLimit : GenericPatch<BotTemplateLimit>
    {
        public BotTemplateLimit() : base(postfix: nameof(PatchPostfix))
        {
            // compile-time checks
            _ = nameof(BotsPresets.CreateProfile);
            _ = nameof(WaveInfo.Difficulty);
        }

        protected override MethodBase GetTargetMethod()
        {
            var sortedList = Constants.Instance.TargetAssembly.GetTypes().Where(_class => _class.Name.StartsWith("GClass"));
            foreach (var type in sortedList)
            {
                //if (type.Name.StartsWith("GClass"))
                //{
                    var BoolCheck = type.GetMethod("GetNewProfile", BindingFlags.NonPublic | BindingFlags.Instance) == null;
                    if (BoolCheck) continue;
                    // its proper gclass now lets check if our targeted method exists there
                    var TargetedMethod = type.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (TargetedMethod != null)
                        return TargetedMethod;
                //}
            }
            return null;
        }

        public static void PatchPostfix(List<WaveInfo> __result/*, List<WaveInfo> wavesProfiles*/, List<WaveInfo> delayed)
        {
            /*
                In short this method sums Limits by grouping wavesPropfiles collection by Role and Difficulty
                then in each group sets Limit to 30, the remainder is stored in "delayed" collection.
                So we change Limit of each group.
                Clear delayed waves, we don't need them if we have enough loaded profiles and in method_2 it creates a lot of garbage.
            */

            delayed?.Clear();

            foreach (WaveInfo wave in __result)
            {
                wave.Limit = Request(wave.Role);
            }
        }

        private static int Request(WildSpawnType role)
        {
            var json = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/settings/bot/limit/" + role.ToString());

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JET]: Received bot " + role.ToString() + " limit data is NULL, using fallback");
                return 30;
            }
            Debug.LogError("[JET]: Successfully received bot " + role.ToString() + " limit data");
            return Convert.ToInt32(json);
        }
    }
}
