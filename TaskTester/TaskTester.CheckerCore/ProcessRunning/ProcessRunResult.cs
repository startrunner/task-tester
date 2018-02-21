using System;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ProcessRunResult 
    {
        public ProcessExitType ExitType { get; }
        public CrashReport CrashReport { get; }
        public StringOrFile Output { get; }
        public StringOrFile StandardError { get; }
        public double MemoryUsed { get; }
        public int ExitCode { get; }
        public TimeSpan ExecutionTime { get; }

        public ProcessRunResult(ProcessExitType exitType, CrashReport crashReport, StringOrFile stdOut, StringOrFile stdErr, double memoryUsed, int exitCode, TimeSpan executionTime)
        {
            ExitType = exitType;
            CrashReport = crashReport;
            Output = stdOut;
            StandardError = stdErr;
            MemoryUsed = memoryUsed;
            ExitCode = exitCode;
            ExecutionTime = executionTime;
        }


    }
}