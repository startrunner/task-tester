using System;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ProcessRunResult 
    {
        public ProcessExitType ExitType { get; }
        public CrashReport CrashReport { get; }
        public StringOrFile StdOut { get; }
        public StringOrFile StdErr { get; }
        public double MemoryUsed { get; }
        public int ExitCode { get; }
        public TimeSpan ExecutionTime { get; }

        public ProcessRunResult(ProcessExitType exitType, CrashReport crashReport, StringOrFile stdOut, StringOrFile stdErr, double memoryUsed, int exitCode, TimeSpan executionTime)
        {
            ExitType = exitType;
            CrashReport = crashReport;
            StdOut = stdOut;
            StdErr = stdErr;
            MemoryUsed = memoryUsed;
            ExitCode = exitCode;
            ExecutionTime = executionTime;
        }


    }
}