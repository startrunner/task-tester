using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationSolutionGraderTask : CheckerCore.Tasking.TaskTesterJob
    {
        public class SolutionGradedEventArgs
        {
            public BatchEvaluationCompetitor Competitor { get; internal set; }
            public BatchEvaluationProblem Problem { get; internal set; }
            public double Score { get; internal set; }
        }

        readonly IReadOnlyList<SolutionEvaluationTestResult> mTestResults;
        readonly BatchEvaluationCompetitor mCompetitor;
        readonly BatchEvaluationProblem mProblem;

        public event EventHandler<SolutionGradedEventArgs> SolutionGraded;

        public BatchEvaluationSolutionGraderTask(
            Action<Delegate, object[]> eventDispatcher,
            CancellationToken cancellationToken,
            BatchEvaluationCompetitor competitor,
            BatchEvaluationProblem problem,
            IReadOnlyList<SolutionEvaluationTestResult> testResults
        ) : base(eventDispatcher, cancellationToken)
        {
            mTestResults = testResults;
            mCompetitor = competitor;
            mProblem = problem;
        }

        public override void Start() => Start(Run);

        private void Run()
        {
            double finalSCore = 0;
            IReadOnlyList<SolutionEvaluationTestResult> results = mTestResults;
            string[] groups = results.Select(x => x.TestGroup).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();

            var totalTestCount = new Dictionary<string, int>();
            var nonZeroTestCount = new Dictionary<string, int>();
            var scoreSum = new Dictionary<string, double>();

            foreach (string group in groups)
            {
                totalTestCount.Add(group, 0);
                nonZeroTestCount.Add(group, 0);
                scoreSum.Add(group, 0);
            }

            foreach (SolutionEvaluationTestResult result in results)
            {
                if (!string.IsNullOrEmpty(result.TestGroup))
                {
                    totalTestCount[result.TestGroup]++;
                    if (result.Score != 0)
                    {
                        nonZeroTestCount[result.TestGroup]++;
                        scoreSum[result.TestGroup] += result.Score;
                    }
                }
                else
                {
                    finalSCore += result.Score;
                }
            }

            foreach (string group in groups)
            {
                if (totalTestCount[group] == nonZeroTestCount[group]) finalSCore += scoreSum[group];
            }

            var eventArgs = new SolutionGradedEventArgs
            {
                Competitor = mCompetitor,
                Problem = mProblem,
                Score = finalSCore
            };

            Notify(SolutionGraded, eventArgs);

            NotifyFinished();
        }
    }
}
