using System;

namespace JET.Utility
{
    internal class Paths
    {
        internal static string GetGameDirectory => AppDomain.CurrentDomain.BaseDirectory;
        internal static string CustomModsDirectory
        {
            get => System.IO.Path.Combine(Utility.Paths.GetGameDirectory, "ClientMods");
        }
    }
}
