using System;

namespace JET.Utility
{
    public class Paths
    {
        /// <summary>
        /// Returns a Base Directory of Current Domain
        /// </summary>
        public static string GetGameDirectory => AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// Returns a path where Client Mods are located
        /// </summary>
        public static string CustomModsDirectory
        {
            get => System.IO.Path.Combine(Utility.Paths.GetGameDirectory, "ClientMods");
        }
    }
}
