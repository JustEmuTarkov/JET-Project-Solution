using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BundleLoader
{
    public static class Shared
    {
        public static bool EnableLogger = true;
        public static void LOGERROR(object data) { if(EnableLogger) Debug.LogError(data); }

        public static uint GenerateFileCrc(string fileAbsolutePath)
        {
            var fileInfo = new FileInfo(fileAbsolutePath);

            return (uint)fileInfo.Length;
        }
        public static bool isLocalServerFile(string path)
        {
            return Regex.IsMatch(path, @"^[A-Z]:/");
        }

        public class Bundle
        {
            public string key { get; set; }
            public string path { get; set; }
            public string[] dependencyKeys { get; set; }
        }
    }
}
