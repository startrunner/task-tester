using System;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public interface IConsoleApplication
    {
        [Obsolete]
        string ExecutablePath { get; }

        IConsoleProcess StartProcess(string processArguments);
    }
}
