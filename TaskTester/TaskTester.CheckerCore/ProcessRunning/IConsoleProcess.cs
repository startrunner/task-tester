using System;

namespace TaskTester.CheckerCore.ProcessRunning
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
        bool TryFindCrashReport(out ICrashReport report);
    }
}
