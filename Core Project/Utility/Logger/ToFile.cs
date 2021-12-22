using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Timers;

namespace JET.Utility.Logger
{
    public static class ToFile
    {
        private static string CurrentFileName
        {
            get
            {
                if (_currentLogFile == null)
                    _currentLogFile = Path.Combine(Application.dataPath, "../Logs/JET Debug/", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".log");
                if (!Directory.Exists(Path.GetDirectoryName(_currentLogFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_currentLogFile));
                return _currentLogFile;
            }
        }
        private static string _currentLogFile = null;
        private static List<string> LogCache = new List<string>();
        private static Timer WriteTimer = new Timer(5000); // 5 seconds

        static ToFile()
        {
            WriteTimer.AutoReset = true;
            WriteTimer.Elapsed += WriteTimer_Elapsed;
            WriteTimer.Start();
            MonoBehaviour.JET_Instance.ApplicationQuitEvent += Application_quitting;
        }

        private static void Application_quitting() => SaveLogFile();

        private static void WriteTimer_Elapsed(object sender, ElapsedEventArgs e) => SaveLogFile();

        private static void SaveLogFile()
        {
            lock (LogCache)
            {
                if (LogCache.Count > 0)
                    File.AppendAllLines(CurrentFileName, LogCache);
                LogCache.Clear();
            }
        }

        public static void Log(string message)
        {
            lock (LogCache)
            {
                LogCache.Add(message);
            }
        }
    }
}
