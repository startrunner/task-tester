using System;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class ProcessRunArguments
    {
        public IConsoleApplication Application { get; }
        public StringOrFile StandardInput { get; }
        public string ProcessArguments { get; }
        public bool AllowCrashReports { get; } = true;
        public TimeSpan MaxRuntime { get; }

        public ProcessRunArguments(IConsoleApplication application, StringOrFile stdIn, TimeSpan maxRuntime, string processArguments, bool allowCrashReports)
        {
            Application = application;
            StandardInput = stdIn;
            ProcessArguments = processArguments??string.Empty;
            AllowCrashReports = allowCrashReports;
            MaxRuntime = maxRuntime;
        }
    }
}
