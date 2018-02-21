using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public sealed class Problem
    {
        public IReadOnlyList<SolutionTest> Tests { get; }

        public Problem(IReadOnlyList<SolutionTest> tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));
            Tests = tests;
        }
    }
}
