using System;

namespace JET.Utility
{
    internal class Paths
    {
        /// <summary>
        /// Returns a Base Directory of Current Domain
        /// </summary>
        internal static string GetGameDirectory => AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// Returns a path where Client Mods are located
        /// </summary>
        internal static string CustomModsDirectory
        {
            get => System.IO.Path.Combine(Utility.Paths.GetGameDirectory, "ClientMods");
        }
    }
}
