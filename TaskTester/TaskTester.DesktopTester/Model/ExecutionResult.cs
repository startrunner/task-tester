using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.Model
{
    class ExecutionResult
    {
        public TimeSpan? ExecutionTime { get; internal set; }
        public string ExpectedAnswer { get; internal set; }
        public int IdentifierIndex { get; internal set; }
        public string SolutionAnswer { get; internal set; }
        public TestResultType Type { get; internal set; }
    }
}
