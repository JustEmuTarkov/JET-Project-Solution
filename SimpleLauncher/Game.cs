using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleLauncher
{
    class Game
    {
        private List<string> m_FilesToRemove = new List<string>() {
            Path.Combine(Environment.CurrentDirectory, "BattlEye"),
            Path.Combine(Environment.CurrentDirectory, "Logs"),
            Path.Combine(Environment.CurrentDirectory, "EscapeFromTarkov_BE.exe"),
            Path.Combine(Environment.CurrentDirectory, "Uninstall.exe"),
            Path.Combine(Environment.CurrentDirectory, "UnityCrashHandler64.exe")
        };
        private void RemoveUselessFiles()
        {
            foreach (var file in m_FilesToRemove)
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        Directory.Delete(file, true);
                    }

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {
                    Core.CMD.Write("Unable to delete: " + file,true,"[ERROR]");
                }
            }
        }
        private bool TestGameInstalledPatch()
        {
            bool HarmonyNotPresent = true;
            bool AssemblyNotPresent = true;
            bool NLogNotPresent = true;
            bool JETNotPresent = true;

            // searching for missing files (should have JET in name!!!)
            List<string> Files = Directory.GetFiles("EscapeFromTarkov_Data/Managed").ToList();
            foreach (var fileName in Files) 
            {
                if (fileName.ToLower().Contains("0Harmony"))
                    HarmonyNotPresent = false;
                if (fileName.ToLower().Contains("Assembly-CSharp"))
                    HarmonyNotPresent = false;
                if (fileName.ToLower().Contains("NLog.dll.nlog"))
                    HarmonyNotPresent = false;
                if (fileName.ToLower().Contains("JET"))
                    HarmonyNotPresent = false;
            }

            if (HarmonyNotPresent) 
                Core.CMD.Write("Missing file: 0Harmony.dll", true, "[ERROR]");
            if (AssemblyNotPresent) 
                Core.CMD.Write("Missing file: Assembly-CSharp.dll", true, "[ERROR]");
            if (NLogNotPresent) 
                Core.CMD.Write("Missing file: NLog.dll.nlog", true, "[ERROR]");
            if (JETNotPresent) 
                Core.CMD.Write("Missing file: JET Binary Patch (NLog.JET.dll or NLog.JET_Project.dll)", true, "[ERROR]");

            return !HarmonyNotPresent && !AssemblyNotPresent && !NLogNotPresent && !JETNotPresent;
        }
        internal string LaunchGame()
        {
            RemoveUselessFiles();

            if (!TestGameInstalledPatch())
                return "Missing Crucial Patch files!";

            if (!File.Exists("EscapeFromTarkov.exe"))
            {
                return "Missing Executable file";
            }

            try
            {
                var clientProcess = new ProcessStartInfo("EscapeFromTarkov.exe")
                {
                    Arguments = GenerateArguments(),
                    UseShellExecute = false,
                    WorkingDirectory = Environment.CurrentDirectory
                };
                Process.Start(clientProcess);
            }
            catch (Exception e)
            {
                return $"Unknown Exception: {e.Message} {e.StackTrace}";
            }

            return "OK";
        }
        internal static string GenerateArguments()
        {
            return "-bC5vLmcuaS5u={\"email\":\"" + Core.i_Main.ConnectedAccount.email + "\",\"password\":\"" + Core.i_Main.ConnectedAccount.password + "\",\"toggle\":true,\"timestamp\":0} -token=" + Core.i_Main.ConnectedAccount.id + " -config={\"BackendUrl\":\"" + Core.i_Main.ConnectedServer.backendUrl + "\",\"Version\":\"live\"}";
        }
    }
}
