using UnityEngine;

namespace BundleLoader.Utilities
{
    public class PathGeneration
    {
        public static string CustomAssetsPath = $"{Application.dataPath}/StreamingAssets/Windows/CustomAssets";

        public static string GenerateDownloadedFileFolderPath(string modName)
        {
            return $"{CustomAssetsPath}/{modName}";
        }
        public static string GenerateDownloadedFilePath(string modName, string bundleName)
        {
            return $"{GenerateDownloadedFileFolderPath(modName)}/{bundleName}";
        }
    }
}
