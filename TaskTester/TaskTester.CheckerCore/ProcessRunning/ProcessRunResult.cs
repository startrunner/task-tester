using System.Collections.Generic;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ProcessRunResult
    {
        public ProcessExitType ExitType { get; internal set; }
        public int ExitCode { get; internal set; }
        public StringOrFile StdOut { get; internal set; }
        public StringOrFile StdErr { get; internal set; }
        public double MemoryUsed { get; internal set; }
        public CrashReport CrashReport { get; internal set; }
    }
}