using JET.Utility.Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace JET
{
    internal class ModsLoader
    {
        internal static readonly List<JetMod> ModInstances = new List<JetMod>();
        private static readonly Dictionary<Type, ModSettings> AvailableMods = new Dictionary<Type, ModSettings>();

        /// <summary>
        /// Constructor that loads all mods and launches them
        /// </summary>
        internal ModsLoader()
        {
            PopulateAvailableMods();

            var noDependencies = AvailableMods.Where(x => !x.Value.DependsOn.Any() && !x.Value.SoftDependsOn.Any()).ToArray();
            var remaining = AvailableMods.Except(noDependencies).ToDictionary(x => x.Key, x => x.Value);

            // Load mods without dependencies first for less recursion (hopefully)
            foreach (var (type, settings) in noDependencies)
            {
                if (!LoadMod(settings, out var mod)) continue;
                ModInstances.Add(mod);
                Debug.Log($"Mod {type.FullName} loaded successfully");
            }

            // Remaining mods have required dependencies that don't exist
            remaining = LoadModsWithDependencies(remaining, false);
            remaining = LoadModsWithDependencies(remaining, true);

            foreach (var (type, settings) in remaining)
            {
                var missing = settings.DependsOn.Where(x => ModInstances.All(y => y.GetType() != x));
                var missingList = missing.Select(x => x.FullName);
                Debug.LogError($"Mod {type.FullName} is missing required dependencies and will not be loaded. Missing: {string.Join(", ", missingList)}");
            }

            #region Maoci's initial idea of mods loading

            //foreach (var file in mods)
            //{
            //    var launchingCommands = File.ReadLines(file).ToArray();
            //    var assemblyPathing = launchingCommands[1].Split('.').ToList();
            //    //Debug.Log($"{launchingCommands[0]} {launchingCommands[1]}");
            //    if (assemblyPathing.Count < 2)
            //    {
            //        Debug.LogError($"Failed with count {assemblyPathing.Count}, string be: {launchingCommands[1]}");
            //        continue;
            //    }
            //    try
            //    {
            //        var loadedAssembly = Assembly.LoadFile(Path.Combine(GetGameDirectory, "ClientMods", launchingCommands[0]));

            //        var classHandle = loadedAssembly.GetExportedTypes().Single(type => type.Name == assemblyPathing[assemblyPathing.Count - 2]);

            //        classHandle.GetMethods(BindingFlags.Public | BindingFlags.Static).Single(method =>
            //        {
            //            if (method.Name == assemblyPathing[assemblyPathing.Count - 1])
            //                Debug.Log(method.Name + " " + method.GetParameters().Length);
            //            return method.Name == assemblyPathing[assemblyPathing.Count - 1] && method.GetParameters().Length == 0;
            //        })
            //            .Invoke(classHandle, new object[] { });

            //        LoadedAssemblyMods.Add(loadedAssembly);
            //    }
            //    catch (Exception errorOccured)
            //    {
            //        Debug.Log($"Error Loading: {file}");
            //        Debug.Log($"Error: {errorOccured.Message}");
            //        Debug.Log(errorOccured.StackTrace);
            //        Debug.Log("-  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -");
            //        continue;
            //    }
            //    Debug.Log($"Loaded: {launchingCommands[0]} [{launchingCommands[1]}]");
            //}

            #endregion
        }

        /// <summary>
        /// Method to load DLL from CustomMods directory with file name supplied to it and returns the types of it
        /// </summary>
        /// <param name="filename">file name that you want to include</param>
        /// <returns>Type[] as GetTypes() from the loaded assembly</returns>
        private static Type[] CustomLoadedAssemblyTypes(string filename)
        {
            var fullPath = Path.Combine(Utility.Paths.CustomModsDirectory, filename);
            // Read the file before loading so the dll file doesn't stay locked.
            var bytes = File.ReadAllBytes(fullPath);
            var assembly = Assembly.Load(bytes);
            return assembly.GetTypes();
        }

        /// <summary>
        /// Loads Assemblies from the CustomMods directory and adds them to the AvailableMods List
        /// </summary>
        private static void PopulateAvailableMods()
        {
            var mods = Directory.GetFiles(Utility.Paths.CustomModsDirectory, "*.dll").ToList();

            if (mods.Count == 0)
            {
                Debug.Log("There are no mods to load.");
                return;
            };

            foreach (var file in mods)
            {
                Type[] loadedTypes;
                try
                {
                    loadedTypes = CustomLoadedAssemblyTypes(file);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load {Path.GetFileName(file)}. {e}");
                    continue;
                }

                loadedTypes = loadedTypes.Where(x => x.IsClass && x.BaseType == typeof(JetMod)).ToArray();
                if (loadedTypes.Length > 1)
                {
                    Debug.LogError($"Failed to load {Path.GetFileName(file)}. You may only have one class that inherits from {nameof(JetMod)}.");
                    continue;
                }

                var type = loadedTypes.FirstOrDefault();
                // DLL file is not a mod and is likely a dependency. Keep it loaded and continue.
                if (type == default)
                    continue;

                AvailableMods.Add(type, new ModSettings(type));
            }
        }

        /// <summary>
        /// Method that search for dependencies and allow or disallow to load the mod
        /// </summary>
        /// <param name="mods">List of mods to load</param>
        /// <param name="ignoreSoftDependencies">Whether or not to ignore soft dependencies</param>
        /// <returns>A list of mods that weren't loaded</returns>
        private static Dictionary<Type, ModSettings> LoadModsWithDependencies(IReadOnlyDictionary<Type, ModSettings> mods, bool ignoreSoftDependencies)
        {
            var remaining = new Dictionary<Type, ModSettings>(mods.ToDictionary(x => x.Key, x => x.Value));

            while (remaining.Count > 0)
            {
                var modsLoaded = 0;
                foreach (var (type, settings) in mods)
                {
                    var allDependencies =
                        settings.DependsOn.Concat(ignoreSoftDependencies ? new Type[] { } : settings.SoftDependsOn);
                    var isMissingDependencies = !allDependencies.All(x => ModInstances.Any(y => y.GetType() == x));

                    if(isMissingDependencies) continue;

                    if (!LoadMod(settings, out var mod)) continue;

                    ModInstances.Add(mod);
                    remaining.Remove(type);
                    modsLoaded++;
                    Debug.Log($"Mod {type.FullName} loaded successfully");
                }

                if (modsLoaded == 0)
                    break;
            }

            return remaining;
        }

        /// <summary>
        /// Method to call Initialize method inside mod instance and launch a mod
        /// </summary>
        /// <param name="settings">author, name etc.</param>
        /// <param name="mod">an instance of mod which holds Initializator method</param>
        /// <returns></returns>
        private static bool LoadMod(ModSettings settings, out JetMod mod)
        {
            mod = null;
            if (settings.ModType.GetConstructors().All(x => x.GetParameters().Length != 0))
            {
                Debug.LogError($"Mod {settings.ModType.FullName} does not contain a constructor that takes 0 arguments. Please add one.");
                return false;
            }

            if (settings.CompatibleWith.Length > 0 && !settings.CompatibleWith.Contains(Application.version))
            {
                Debug.LogError($"Mod {settings.ModType.FullName} is not compatible with game version {Application.version}.");
                return false;
            }

            try
            {
                var instance = Activator.CreateInstance(settings.ModType) as JetMod;
                var dependInstances = ModInstances
                    .Where(x => settings.DependsOn.Contains(x.GetType()) || settings.DependsOn.Contains(x.GetType()))
                    .ToDictionary(x => x.GetType(), x => x);

                try
                {
                    instance?.Initialize(dependInstances, Application.version);
                    if (instance == null)
                    {
                        Debug.LogError($"Failed to load mod {settings.ModType.FullName}. Instance is null.");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"An error occurred while loading mod {settings.ModType.FullName}: {e}");
                    // Don't return as the mod could still be (mostly) working
                }

                mod = instance;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load mod {settings.ModType.FullName}: {e}");
                return false;
            }
        }
    }
}
