using JET.Utility.Patching;
using JET.Utility.Logger;
using NLog;
using System;
using System.Linq;
using System.Reflection;

namespace JET.Patches.Logger
{
    /// <summary>
    /// overrides the logger that will save the logs to custom made file
    /// </summary>
    class LoggerOverride : GenericPatch<LoggerOverride>
    {
        public LoggerOverride() : base(prefix: nameof(LoggerPrefix)) { }
        static bool LoggerPrefix(string nlogFormat, string unityFormat, LogLevel logLevel, params object[] args)
        {
            try
            {
                ToFile.Log($"[{logLevel.Name}] {string.Format(nlogFormat, args)}");
            }
            catch (FormatException)
            {
                ToFile.Log($"[{logLevel.Name}] {nlogFormat}");
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.Instance.TargetAssemblyTypes
                .First(x => x.IsClass && x.GetProperty("UnityDebugLogsEnabled") != null)
                .GetMethods()
                .First(x => x.GetParameters().Length == 4 && x.Name == "Log");
        }
    }
}
