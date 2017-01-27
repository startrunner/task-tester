using System.Collections.Generic;

namespace TaskTester.CheckerCore
{
    public class ProcessRunResult
    {
        public ProcessExitType ExitType { get; internal set; }
        public int ExitCode { get; internal set; }
        public string StdOut { get; internal set; }
        public string StdErr { get; internal set; }
        public double MemoryUsed { get; internal set; }
        public CrashReport CrashReport { get; internal set; }
    }
}