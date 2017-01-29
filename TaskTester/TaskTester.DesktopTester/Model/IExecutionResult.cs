using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.DesktopTester.Model
{
    internal interface IExecutionResult
    {
        TimeSpan? ExecutionTime { get; }
        string ExpectedAnswer { get; }
        int IdentifierIndex { get; }
        string SolutionAnswer { get; }
        TestResultType Type { get; }
    }

    class ExecutionResultMutable:IExecutionResult
    {
        public TimeSpan? ExecutionTime { get; internal set; }
        public string ExpectedAnswer { get; internal set; }
        public int IdentifierIndex { get; internal set; }
        public string SolutionAnswer { get; internal set; }
        public TestResultType Type { get; internal set; }
    }
}
