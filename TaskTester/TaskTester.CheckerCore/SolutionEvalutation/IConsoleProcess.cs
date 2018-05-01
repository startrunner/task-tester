using System;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public interface IConsoleProcess
    {
        void WriteStandardInput(string input);
        string GetStandardOutput();
        string GetStandardError();
        void EnsureKilled();
        TimeSpan ExecutionTime { get; }
        int ExitCode { get; }
        bool HasExited { get; }
        void CloseStandardInput();
        void WaitForExit(int timeLimit = int.MaxValue);
        bool HasCrashed(out ICrashReport report);
    }
}
