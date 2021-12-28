﻿using UnityEngine;
using Comfort.Common;
using EFT;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using System;

namespace JET.Utility
{
    public static class ClientAccesor
    {
        #region Get MainApplication Variable
        public static ClientApplication GetClientApp()
        {
            return Singleton<ClientApplication>.Instance;
        }

        public static MainApplication GetMainApp()
        {
            return GetClientApp() as MainApplication;
        }
        #endregion 

        #region Get GameVersion String
        private static string _gameVersion = "";
        public static string GameVersion
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_gameVersion)) return _gameVersion;
                var list = Constants.Instance.TargetAssembly.GetTypes()
                    .Where(type =>
                        type.Name.StartsWith("Class") &&
                        type.GetField("string_0", BindingFlags.NonPublic | BindingFlags.Static) != null &&
                        type.GetMethods().Length == 4 &&
                        type.GetProperties().Length == 0 &&
                        type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Length == 0 &&
                        type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static).Length == 0)
                    .ToList();
                if (list.Count > 0)
                    _gameVersion = list[0].GetField("string_0", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null).ToString();
                return _gameVersion;
            }
        }
        #endregion

        #region PreloaderUI Instance

        public static EFT.UI.PreloaderUI PreloaderUI 
        {
            get 
            {
                return Singleton<EFT.UI.PreloaderUI>.Instance;
            }
        }

        #endregion

        #region Get BetaVersionText Variable
        static EFT.UI.LocalizedText localizedText;
        internal static EFT.UI.LocalizedText BetaVersionLabel {
            get
            {
                if (localizedText == null && PreloaderUI != null)
                {
                    if (typeof(EFT.UI.PreloaderUI).GetField("_alphaVersionLabel", BindingFlags.NonPublic | BindingFlags.Instance) == null)
                        return null;
                    localizedText = typeof(EFT.UI.PreloaderUI)
                    .GetField("_alphaVersionLabel", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(PreloaderUI) as EFT.UI.LocalizedText;
                }
                return localizedText;
            }
        }

        #endregion

        private static string CashedBackendUrl;
        public static string BackendUrl {
            get 
            {
                if(CashedBackendUrl == null)
                {
                    CashedBackendUrl = Constants.Instance.TargetAssemblyTypes
                        .Where(type => type.GetField("DEFAULT_BACKEND_URL", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) != null)
                        .FirstOrDefault()
                        .GetProperty("BackendUrl", BindingFlags.Static | BindingFlags.Public).GetValue(null) as string;
                }
                if (CashedBackendUrl == null)
                    Debug.LogError("CashedBackendUrl still is null");
                return CashedBackendUrl;
            }
        }

    }
}