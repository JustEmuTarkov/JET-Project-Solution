using HarmonyLib;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BundleLoader
{
    public static class Shared
    {
        #region Dictionaries
        public static readonly Dictionary<string, string> CachedBundles = new Dictionary<string, string>();
        public static readonly Dictionary<string, List<string>> ModdedAssets = new Dictionary<string, List<string>>();
        public static readonly Dictionary<string, string> ModdedBundlePaths = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> ManifestCache = new Dictionary<string, string>();
        #endregion

        #region Properties
        public static Type LoaderType
        {
            get
            {
                if (_loaderType == null)
                    _loaderType = Constants.Instance.TargetAssembly.GetTypes().Single(x => x.IsClass && x.GetProperty("SameNameAsset") != null);
                return _loaderType;
            }
        }
        public static Type BundleLockType
        {
            get
            {
                if (_bundleLockType == null)
                    _bundleLockType = Constants.Instance.TargetAssembly.GetTypes().Single(x =>
                        x.IsInterface &&
                        x.GetProperty("IsLocked") != null &&
                        x.GetMethod("Unlock") != null);
                return _bundleLockType;
            }
        }

        public static Type NodeType
        {
            get
            {
                if (_nodeType == null)
                {
                    var nodeInterfaceType = Constants.Instance.TargetAssembly.GetTypes()
                        .First(x => x.IsInterface && x.GetProperty("SameNameAsset") != null);

                    _nodeType = Constants.Instance.TargetAssembly.GetTypes()
                        .Single(x =>
                            x.IsClass && x.GetMethod("GetNode") != null && string.IsNullOrWhiteSpace(x.Namespace))
                        .MakeGenericType(nodeInterfaceType);
                }

                return _nodeType;
            }
        }
        public static PropertyInfo LoadState
        {
            get
            {
                if (_loadState == null)
                    _loadState = LoaderType.GetProperty("LoadState");
                return _loadState;
            }
        }
        public static PropertyInfo LoadStateProperty
        {
            get
            {
                if (_loadStateProperty == null)
                    _loadStateProperty = LoadState.PropertyType.GetProperty("Value");
                return _loadStateProperty;
            }
        }
        public static FieldInfo BundleLockField
        {
            get
            {
                if (_bundleLockField == null)
                    _bundleLockField = LoaderType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.FieldType == BundleLockType);
                return _bundleLockField;
            }
        }
        public static FieldInfo TaskField
        {
            get
            {
                if (_taskField == null)
                    _taskField = LoaderType.GetField("task_0", BindingFlags.Instance | BindingFlags.NonPublic); ;
                return _taskField;
            }
        }
        public static FieldInfo BundleField
        {
            get
            {
                if (_bundleField == null)
                    _bundleField = LoaderType.GetField("assetBundle_0", BindingFlags.Instance | BindingFlags.NonPublic);
                return _bundleField;
            }
        }
        public static ConstructorInfo BundleLockConstructor
        {
            get
            {
                if (_bundleLockConstructor == null)
                    _bundleLockConstructor = Constants.Instance.TargetAssembly.GetTypes()
                        .First(x => x.IsClass && x.GetProperty("MaxConcurrentOperations") != null).GetConstructors().First();
                return _bundleLockConstructor;
            }
        }
        #endregion

        #region Backing Fields
        private static Type _loaderType;
        private static Type _bundleLockType;
        private static Type _nodeType;
        private static PropertyInfo _loadState;
        private static PropertyInfo _loadStateProperty;
        private static FieldInfo _bundleLockField;
        private static FieldInfo _taskField;
        private static FieldInfo _bundleField;
        private static ConstructorInfo _bundleLockConstructor;
        #endregion

        #region Constants
        public const string LOCAL_BUNDLES_PATH = @"Bundles\Local";
        public const string CACHE_BUNDLES_PATH = @"Bundles\Cache";
        #endregion

        #region Types
        internal class BundleLockWrapper
        {
            readonly object _instance;

            internal bool IsLocked
            {
                get => (bool)AccessTools.Property(_instance.GetType(), "IsLocked").GetValue(_instance);
                set => AccessTools.Property(_instance.GetType(), "IsLocked").SetValue(_instance, value);
            }
            internal BundleLockWrapper(object instance)
            {
                _instance = instance;
            }

            internal void Lock() =>
                AccessTools.Method(_instance.GetType(), "Lock").Invoke(_instance, new object[] { });

            internal void Unlock() =>
                AccessTools.Method(_instance.GetType(), "Unlock").Invoke(_instance, new object[] { });

        }

        //This is meant to mimic the struct that is used to convert a JSON file to a Dictionary<string, BundleDetails> in EasyAssets.method_0
        internal struct BundleDetailStruct
        {
            public string FileName;
            public uint Crc;
            public string[] Dependencies;
        }
        #endregion
    }
}
