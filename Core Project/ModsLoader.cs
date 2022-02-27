using JET.Utility.Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JET
{
    class ModsLoader
    {
        internal static readonly List<JetMod> ModInstances = new List<JetMod>();
        private static Dictionary<Type, ModSettings> AvailableMods = new Dictionary<Type, ModSettings>();
        private static IEnumerable<KeyValuePair<Type, ModSettings>> NoDependencies;
        private static KeyValuePair<Type, ModSettings>[] Remaining;

        /// <summary>
        /// Constructor that loads all mods and launches them
        /// </summary>
        internal ModsLoader()
        {
            if (!InitialLoadingOfFiles())
            {
                Debug.Log($"Mods count is equal to 0. Skipping loading of the custom mods");
            }

            NoDependencies = AvailableMods.Where(x => !x.Value.DependsOn.Any() && !x.Value.SoftDependsOn.Any());
            Remaining = AvailableMods.Where(x => !NoDependencies.Contains(x)).ToArray();

            // Load mods without dependencies first for less recursion (hopefully)
            foreach (var (type, settings) in NoDependencies)
            {
                if (!LoadMod(settings, out var mod)) continue;
                ModInstances.Add(mod);
                Debug.Log($"Mod {type.FullName} loaded successfully");
            }

            for (int iter = 0; iter < 2; iter++)
            {
                LoadMods(iter);
            }

            // Remaining mods have required dependencies that don't exist
            foreach (var (type, settings) in Remaining)
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
        /// Loads the Assemblies(DLL's) from CustomMods directory and add them to the list of AvailableMods List
        /// </summary>
        /// <returns>Always "true"</returns>
        private static bool InitialLoadingOfFiles()
        {
            var mods = Directory.GetFiles(Utility.Paths.CustomModsDirectory, "*.dll").ToList();

            if (mods.Count == 0) return false;

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
            return true;
        }

        /// <summary>
        /// Method that dearch for dependencies and allow or disallow to load the mod
        /// </summary>
        /// <param name="iteration">0 or 1 where 0 is Load with chekcs of all dependencies, 1 is </param>
        /// <returns></returns>
        private static bool LoadMods(int iteration = 0)
        {
            // Iteration 0 -> Load mods only if dependencies and soft dependencies are loaded
            // Iteration 1 -> Load mods if dependencies are loaded, ignoring soft dependencies
            while (Remaining.Length > 0)
            {
                var modsLoaded = 0;
                var newRemaining = Remaining.ToList();
                foreach (var (type, settings) in Remaining)
                {
                    if (iteration == 0)
                    {
                        if (!settings.DependsOn.Select(x => ModInstances.Any(y => y.GetType() == x)).All(x => x) ||
                        !settings.SoftDependsOn.Select(x => ModInstances.Any(y => y.GetType() == x)).All(x => x))
                            continue;
                    }
                    else
                    {
                        if (!settings.DependsOn.Select(x => ModInstances.Any(y => y.GetType() == x)).All(x => x))
                            continue;
                    }

                    if (!LoadMod(settings, out var mod)) continue;

                    ModInstances.Add(mod);
                    newRemaining.RemoveFirst(x => x.Key == type);
                    modsLoaded++;
                    Debug.Log($"Mod {type.FullName} loaded successfully");
                }

                Remaining = newRemaining.ToArray();
                if (modsLoaded == 0)
                    break;
            }
            return true;
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
