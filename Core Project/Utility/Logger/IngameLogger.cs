using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JET.Utility.Logger
{
    class IngameLogger
    {
        private static void FindInternalLoggerVariables() {
            if (loggerClass == null)
            {
                var list = Constants.TargetAssemblyTypes
                    .Where(type =>
                        type.Name.StartsWith("GClass") &&
                        type.GetField("IsLogsEnabled", BindingFlags.Public | BindingFlags.Static) != null &&
                        type.GetField("UnityDebugLogsEnabled", BindingFlags.Public | BindingFlags.Static) != null
                    ).ToList();

                if (list.Count > 0)
                {
                    loggerClass = list[0];
                    isLogsEnabled = loggerClass.GetField("IsLogsEnabled", BindingFlags.Public | BindingFlags.Static);
                    unityDebugLogsEnabled = loggerClass.GetField("IsLogsEnabled", BindingFlags.Public | BindingFlags.Static);
                }
            }
        }

        internal static void CheckAndSet() {
            Full();
            None();
            Normal();
        }

        internal static void Full() {
            if (!Validator.isFullLoggerEnabled) return;
            // if logger is enabled enable all features
            FindInternalLoggerVariables();
            if (isLogsEnabled != null)
                isLogsEnabled.SetValue(null, true);
            if (unityDebugLogsEnabled != null)
                unityDebugLogsEnabled.SetValue(null, true);
            Debug.unityLogger.logEnabled = true;
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
        }

        internal static void None() {
            if (!Validator.isFullLoggerDisabled) return;
            // if logger is enabled enable all features
            FindInternalLoggerVariables();
            if (isLogsEnabled != null)
                isLogsEnabled.SetValue(null, false);
            if (unityDebugLogsEnabled != null)
                unityDebugLogsEnabled.SetValue(null, false);
            Debug.unityLogger.logEnabled = false;
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        }

        internal static void Normal() {
            if (!Validator.isNormalLoggerEnabled) return;
            // if logger is enabled enable all features
            FindInternalLoggerVariables();
            if (isLogsEnabled != null)
                isLogsEnabled.SetValue(null, true);
            if (unityDebugLogsEnabled != null)
                unityDebugLogsEnabled.SetValue(null, true);
            Debug.unityLogger.logEnabled = true;
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        }

        private static Type loggerClass;
        private static FieldInfo isLogsEnabled;
        private static FieldInfo unityDebugLogsEnabled;

    }
}
