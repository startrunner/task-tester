using System;
using System.Collections.Generic;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public interface IProcessRunResult
    {
        ProcessExitType ExitType { get; }
        int ExitCode { get; }
        StringOrFile StdOut { get; }
        StringOrFile StdErr { get; }
        double MemoryUsed { get; }
        ICrashReport CrashReport { get; }
    }

    public class ProcessRunResultMutable : IProcessRunResult
    {
        public ProcessExitType ExitType { get; set; }
        public int ExitCode { get; set; }
        public StringOrFile StdOut { get; set; }
        public StringOrFile StdErr { get; set; }
        public double MemoryUsed { get; set; }
        public ICrashReport CrashReport { get; set; }
    }
}