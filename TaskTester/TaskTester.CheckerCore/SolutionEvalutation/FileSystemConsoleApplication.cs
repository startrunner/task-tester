using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
