using System.Collections.Generic;

namespace TaskTester.CheckerCore
{
    public class ProcessRunResult
    {
        public int ExitCode { get; internal set; }
        public string StdOut { get; internal set; }
        public string StdErr { get; internal set; }
        public bool TimelyExit { get; internal set; }
        public bool GracefulExit => CrashReport == null;
        public double MemoryUsed { get; internal set; }
        public CrashReport CrashReport { get; internal set; }
    }
}