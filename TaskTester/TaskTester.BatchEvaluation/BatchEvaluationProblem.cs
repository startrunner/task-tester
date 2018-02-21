using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationProblem
    {
        public string Identifier { get; }

        public Problem Problem { get; }

        public BatchEvaluationProblem(string identifier, Problem problem)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            Identifier = identifier;
            Problem = problem;
        }
    }
}
