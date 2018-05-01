using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public class FileSystemConsoleApplication : IConsoleApplication
    {
        public string ExecutablePath { get; }

        public FileSystemConsoleApplication(string executablePath)
        {
            if (executablePath == null)
                throw new ArgumentNullException(nameof(executablePath));
            ExecutablePath = executablePath;
        }

        public IConsoleProcess StartProcess(string processArguments) =>
            FileSystemConsoleProcess.Start(ExecutablePath, processArguments);
    }
}
