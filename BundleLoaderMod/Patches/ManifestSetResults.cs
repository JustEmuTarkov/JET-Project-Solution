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
        // if that works then load it by using postfix (after the call of main method)
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
        private static Dictionary<string, BundleDetails> m_CustomDetails = new Dictionary<string, BundleDetails>();
        static void GenerateCustomBundleDetails(string bundleName, string modName, string FolderPath, string[] Dependencies) {
            var bundleDetails = new BundleDetails()
            {
                Crc = Shared.GenerateFileCrc(Path.Combine(FolderPath, bundleName)),
                FileName = Path.Combine(FolderPath, bundleName),
                Dependencies = Dependencies,
            };

            m_CustomDetails.Add($"CustomAssets/{modName}/{bundleName}", bundleDetails);
            //BuildPipeline.GetCRCForAssetBundle(path, out crc);

        }
        static void LoadBundlesFromServer() 
        {
            // make sure main folder for custom bundles exists !!!
            if (!Directory.Exists(PathGeneration.CustomAssetsPath))
            {
                Directory.CreateDirectory(PathGeneration.CustomAssetsPath);
            }
            //CheckCustomAssetsPath();
            var text = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/bundles");
            var serverBundles = JsonConvert.DeserializeObject<Shared.Bundle[]>(text);

            // loop through list of all bundles
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
                    if (isLocalFile)
                    {
                        File.Copy(bundle.path, customBundleFilePath);
                        Debug.LogError($"LOCAL FILE COPY: {bundle.path} => {customBundleFilePath}");
                    }
                    else
                    {
                        // Download bundle and put it in the cache folder
                        var url = ClientAccesor.BackendUrl + "/files/bundle/" + bundle.key;

                        Debug.Log("Downloading bundle from " + url);

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