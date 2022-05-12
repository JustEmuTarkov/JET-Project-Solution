using BundleLoader.Utilities;
using HarmonyLib;
using JET.Utility;
using JET.Utility.Patching;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace BundleLoader.Patches
{
    class ManifestSetResults : GenericPatch<ManifestSetResults>
    {
        public ManifestSetResults() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            //CompatibilityAssetBundleManifest.SetResults(results)
            return typeof(CompatibilityAssetBundleManifest).GetMethod("SetResults");
        }
        /// <summary>
        /// This patch simply loads data from server (custom bundles) and then adds new bundles to the existing ones after that code continues as normal
        /// </summary>
        /// <param name="__instance"></param>
        private static void PatchPostfix(object __instance/*Dictionary<string, BundleDetails> results, ref Dictionary<string, BundleDetails> __m_Details*/)
        {
            LoadBundlesFromServer();
            _trav = new Traverse(__instance);
            Dictionary<string, BundleDetails> m_Details = _trav.Field<Dictionary<string, BundleDetails>>("m_Details").Value;
            foreach (var customBundle in m_CustomDetails) 
            {
                m_Details.Add(customBundle.Key, customBundle.Value);
            }
            Debug.LogError($"Loaded {m_CustomDetails.Count} custom bundles");
        }
        // this is place where custom bundles will be created from function GenerateCustomBundleDetails()
        private static Dictionary<string, BundleDetails> m_CustomDetails = new Dictionary<string, BundleDetails>();
        /// <summary>
        /// Creates Custom Bundle data that will be injected later on into the original list
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="modName"></param>
        /// <param name="FolderPath"></param>
        /// <param name="Dependencies"></param>
        static void GenerateCustomBundleDetails(string bundleName, string modName, string FolderPath, string[] Dependencies) {
            var bundleDetails = new BundleDetails()
            {
                Crc = Shared.GenerateFileCrc(Path.Combine(FolderPath, bundleName)),
                FileName = Path.Combine(FolderPath, bundleName),
                Dependencies = Dependencies,
            };

            m_CustomDetails.Add($"CustomAssets/{modName}/{bundleName}", bundleDetails);
        }
        /// <summary>
        /// This function Requests server about bundles they are using then it checks if files are from local server 
        /// if so then it will simply copy bundles from server directory
        /// if not then it will ask server to give the files aka Download them from server
        /// after that all bundles should be located in 
        /// "EscapeFromTarkov_Data/StreamingAssets/Windows/CustomAssets/Mod_Name_with_Version/*.bundle"
        /// </summary>
        static void LoadBundlesFromServer() 
        {
            // make sure main folder for custom bundles exists !!!
            if (!Directory.Exists(PathGeneration.CustomAssetsPath))
            {
                Directory.CreateDirectory(PathGeneration.CustomAssetsPath);
            }
            // request server manifest
            var text = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/bundles");
            var serverBundles = JsonConvert.DeserializeObject<Shared.Bundle[]>(text);

            // loop through manifest of all custom bundles
            foreach (var bundle in serverBundles) 
            {
                bool isLocalFile = Shared.isLocalServerFile(bundle.path);
                string bundleName = bundle.key.Split('/').Last();
                string modName = bundle.key.Split('/').First();
                string customBundlesFolderPath = PathGeneration.GenerateDownloadedFileFolderPath(modName).Replace('\\', '/');
                string customBundleFilePath = PathGeneration.GenerateDownloadedFilePath(modName, bundleName).Replace('\\', '/');
                // make sure directory exists for a custom bundle
                if (!Directory.Exists(customBundlesFolderPath)) 
                {
                    Directory.CreateDirectory(customBundlesFolderPath);
                }
                // download only if file is not existing in a first place
                if (!File.Exists(customBundleFilePath))
                {
                    // if local server just copy file
                    if (isLocalFile)
                    {
                        File.Copy(bundle.path, customBundleFilePath);
                        Debug.LogError($"LOCAL FILE COPY: {bundle.path} => {customBundleFilePath}");
                    }
                    // download using WebClient otherwise
                    else
                    {
                        var url = ClientAccesor.BackendUrl + "/files/bundle/" + bundle.key;
                        _client.DownloadFile(url, customBundleFilePath);
                        Debug.LogError($"DOWNLOAD FILE: {url} => {customBundleFilePath}");
                    }
                }
                GenerateCustomBundleDetails(bundleName, modName, customBundlesFolderPath, bundle.dependencyKeys);
            }
        }

        private static readonly WebClient _client = new WebClient { CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore) };
        private static Traverse _trav;
    }
}