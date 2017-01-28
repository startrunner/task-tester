using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public interface IProcessRunResult
    {
        ProcessExitType ExitType { get; }
        ICrashReport CrashReport { get; }
        StringOrFile StdOut { get; }
        StringOrFile StdErr { get; }
        double MemoryUsed { get; }
        int ExitCode { get; }
    }

    public class ProcessRunResultMutable : IProcessRunResult
    {
        public ProcessExitType ExitType { get; set; }
        public ICrashReport CrashReport { get; set; }
        public StringOrFile StdOut { get; set; }
        public StringOrFile StdErr { get; set; }
        public double MemoryUsed { get; set; }
        public int ExitCode { get; set; }
    }
}