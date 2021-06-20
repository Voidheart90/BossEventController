using System;
using System.Diagnostics;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Common
{
    [Flags]
    internal enum LogLevel
    {
        None = 0,
        Fatal = 1,
        Error = 2,
        Info = 4,
        Debug = 8,
        Message = 16,
        Warning = 32,
        All = None | Fatal | Error | Info | Debug | Message | Warning
    }

    public static class LogEx
    {
        public static ManualLogSource LogSource;
        public static ConfigFile Config;

        public static void Init(ManualLogSource logSource, ConfigFile configFile)
        {
            LogSource = logSource;
            Config = configFile;
        }

        private static bool TestLogLevel(LogLevel level)
        {
            var boxedValue = Config["Debug", "LogLevel"].BoxedValue;
            if (boxedValue != null)
                return ((LogLevel)boxedValue).HasFlag(level);

            return false;
        }

        public static void LogDebug(object data)
        {
            if (TestLogLevel(LogLevel.Debug))
                LogSource.LogDebug(data);
        }

        public static void LogError(object data)
        {
            if (TestLogLevel(LogLevel.Error))
                LogSource.LogError(data);
        }

        public static void LogFatal(object data)
        {
            if (TestLogLevel(LogLevel.Fatal))
                LogSource.LogFatal(data);
        }

        public static void LogInfo(object data)
        {
            if (TestLogLevel(LogLevel.Info))
                LogSource.LogInfo(data);
        }

        public static void LogWarning(object data)
        {
            if (TestLogLevel(LogLevel.Warning))
                LogSource.LogWarning(data);
        }

        [Conditional("DEBUG")]
        public static void LogMessage(object data)
        {
            if (TestLogLevel(LogLevel.Message))
                LogSource.LogMessage(data);
        }
    }
}