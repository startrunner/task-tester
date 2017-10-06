using System;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class ApplicationRunArguments
    {
        public string ExecutablePath { get; }
        public StringOrFile StandardInput { get; }
        public string ProcessArguments { get; }
        public bool AllowCrashReports { get; } = true;

        public TimeSpan MaxRuntime { get; }

        public ApplicationRunArguments(string executablePath, StringOrFile stdIn, TimeSpan maxRuntime, string processArguments, bool allowCrashReports)
        {
            ExecutablePath = executablePath;
            StandardInput = stdIn;
            ProcessArguments = processArguments??string.Empty;
            AllowCrashReports = allowCrashReports;
            MaxRuntime = maxRuntime;
        }
    }
}
