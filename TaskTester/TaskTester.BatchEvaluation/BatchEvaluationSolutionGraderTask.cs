using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using TaskTester.CheckerCore;
using TaskTester.CheckerCore.SolutionEvalutation;
using TaskTester.Tasking;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationSolutionGraderTask : BackgroundTask
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
            Dispatcher eventDispatcher,
            BatchEvaluationCompetitor competitor,
            BatchEvaluationProblem problem,
            IReadOnlyList<SolutionEvaluationTestResult> testResults
        ) : base(eventDispatcher)
        {
            mTestResults = testResults;
            mCompetitor = competitor;
            mProblem = problem;
        }

        public override void Start()
        {
            MarkAsStarted();
            ExecutingTask = Task.Run(action: this.Run);
        }

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

            var eventArgs = new SolutionGradedEventArgs {
                Competitor = mCompetitor,
                Problem = mProblem,
                Score = finalSCore
            };

            Notify(SolutionGraded, eventArgs);

            NotifyFinished();
        }
    }
}
