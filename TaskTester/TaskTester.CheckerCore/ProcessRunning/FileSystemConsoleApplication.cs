using System;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class FileSystemConsoleApplication : IConsoleApplication
    {
        public ICrashReportFinder CrashReportFinder { get; }
        public string ExecutablePath { get; }

        public FileSystemConsoleApplication(string executablePath, ICrashReportFinder crashReportFinder)
        {
            if (executablePath == null)
                throw new ArgumentNullException(nameof(executablePath));

            CrashReportFinder = crashReportFinder;
            ExecutablePath = executablePath;
        }

        public IConsoleProcess StartProcess(string processArguments) =>
            FileSystemConsoleProcess.Start(ExecutablePath, processArguments, CrashReportFinder);
    }
}
