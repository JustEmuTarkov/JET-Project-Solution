using System.Linq;
using System.Threading.Tasks;
using JET.Utility.Patching;
using System.Reflection;
using EFT;
using UnityEngine;
using JET.Utility;
using Comfort.Common;
using System.Threading;

//using WaveInfo = GClass984; // not used // search for: Difficulty and chppse gclass with lower number whic hcontains Role and Limit variables
using BotsPresets = GClass553; // Search for GetNewProfile
using BotData = GInterface15; // Search for PrepareToLoadBackend
using PoolManager = GClass1487; // Search for LoadBundlesAndCreatePools
using JobPriority = GClass2549; // Search for General


namespace SinglePlayerMod.Patches.Raid
{
    class LoadBotTemplatesFromServer : GenericPatch<LoadBotTemplatesFromServer>
    {
        private static MethodInfo _getNewProfileMethod;

        public LoadBotTemplatesFromServer() : base(prefix: nameof(PatchPrefix))
        {
            _ = nameof(BotData.PrepareToLoadBackend);
            _ = nameof(BotsPresets.GetNewProfile);
            _ = nameof(PoolManager.LoadBundlesAndCreatePools);
            _ = nameof(JobPriority.General);

            _getNewProfileMethod = typeof(BotsPresets)
                .GetMethod(nameof(BotsPresets.GetNewProfile), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotsPresets).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single(x => IsTargetMethod(x));
        }

        private bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 2
                && parameters[0].Name == "data"
                && parameters[1].Name == "cancellationToken");
        }

        public static bool PatchPrefix(ref Task<Profile> __result, BotsPresets __instance, BotData data)
        {
            /*
                in short when client wants new bot and GetNewProfile() return null (if not more available templates or they don't satisfied by Role and Difficulty condition)
                then client gets new piece of WaveInfo collection (with Limit = 30 by default) and make request to server
                but use only first value in response (this creates a lot of garbage and cause freezes)
                after patch we request only 1 template from server

                along with other patches this one causes to call data.PrepareToLoadBackend(1) gets the result with required role and difficulty:
                new[] { new WaveInfo() { Limit = 1, Role = role, Difficulty = difficulty } }
                then perform request to server and get only first value of resulting single element collection
            */
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var taskAwaiter = (Task<Profile>)null;
            var profile = (Profile)_getNewProfileMethod.Invoke(__instance, parameters: new object[] { data });

            if (profile == null)
            {
                // load from server
                Debug.Log("Loading bot profile from server");
                var source = data.PrepareToLoadBackend(1).ToList();
                taskAwaiter = ClientAccesor.GetClientApp().GetClientBackEndSession().LoadBots(source).ContinueWith(GetFirstResult, taskScheduler);
            }
            else
            {
                // return cached profile
                Debug.Log("Loading bot profile from cache");
                taskAwaiter = Task.FromResult(profile);
            }

            // load bundles for bot profile
            var continuation = new Continuation(taskScheduler);
            __result = taskAwaiter.ContinueWith(continuation.LoadBundles, taskScheduler).Unwrap();
            return false;
        }

        private static Profile GetFirstResult(Task<Profile[]> task)
        {
            if (task.IsCompleted && task.Result.Any())
            {
                var result = task.Result[0];
                UnityEngine.Debug.LogError($"Loading bot profile from server. role: {result.Info.Settings.Role} side: {result.Side}");
                return result;
            }

            return null;
        }

        private struct Continuation
        {
            Profile Profile;
            TaskScheduler TaskScheduler { get; }

            public Continuation(TaskScheduler taskScheduler)
            {
                Profile = null;
                TaskScheduler = taskScheduler;
            }

            public Task<Profile> LoadBundles(Task<Profile> task)
            {
                Profile = task.Result;

                var loadTask = Singleton<PoolManager>.Instance
                    .LoadBundlesAndCreatePools(PoolManager.PoolsCategory.Raid,
                                               PoolManager.AssemblyType.Local,
                                               Profile.GetAllPrefabPaths(false).ToArray(),
                                               JobPriority.General,
                                               null,
                                               default(CancellationToken));

                return loadTask.ContinueWith(GetProfile, TaskScheduler);
            }

            private Profile GetProfile(Task task)
            {
                return Profile;
            }
        }
    }
}