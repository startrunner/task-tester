using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public interface IConsoleApplication
    {
        [Obsolete]
        string ExecutablePath { get; }

        IConsoleProcess StartProcess(string processArguments);
    }
}
