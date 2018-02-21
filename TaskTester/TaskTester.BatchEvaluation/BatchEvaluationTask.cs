using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore;
using TaskTester.CheckerCore.SolutionEvalutation;
using TaskTester.DataExtraction;
using TaskTester.Tasking;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationTask : BackgroundTask
    {
        readonly string mRootDirectory;
        readonly string mDirectoryPathCriteria;
        readonly IReadOnlyList<string> mCommandLineTemplates;
        readonly IReadOnlyList<BatchEvaluationProblem> mProblems;
        readonly List<BatchEvaluationCompetitor> mDiscoveredCompetitors = new List<BatchEvaluationCompetitor>();

        public event EventHandler<CompetitorInfoExtractedEventArgs> CompetitorDiscovered;
        public event EventHandler<CommandLineRunningTask.CommandRanEventArgs> CommandLineExecuted;
        public event EventHandler<CompetitorEvaluationTask.TestEvaluatedEventArgs> TestEvaluated;
        public event EventHandler<BatchEvaluationSolutionGraderTask.SolutionGradedEventArgs> SolutionGraded;

        public BatchEvaluationTask(
            Dispatcher eventDispatcher,
            string rootDirectory,
            string directoryPathCriteria,
            IReadOnlyList<string> commandLineTemplates,
            IReadOnlyList<BatchEvaluationProblem> problems
        ) : base(eventDispatcher)
        {
            mEventDispatcher = eventDispatcher;
            mRootDirectory = rootDirectory;
            mDirectoryPathCriteria = directoryPathCriteria;
            mCommandLineTemplates = commandLineTemplates;
            mProblems = problems;
        }

        public override void Start()
        {
            MarkAsStarted();
            this.ExecutingTask = Task.Run(action: Run);
        }

        private void Run()
        {
            var discoveryTask = new CompetitionDataExtractionTask(
                null,
                mRootDirectory,
                mDirectoryPathCriteria
            );
            discoveryTask.CompetitorInfoExtracted += HandleCompetitorDiscovered;
            discoveryTask.Start();
            discoveryTask.ExecutingTask.GetAwaiter().GetResult();

            foreach (BatchEvaluationCompetitor competitor in mDiscoveredCompetitors)
            {
                RunCompetitor(competitor);
            }

            NotifyFinished();
        }

        private void RunCompetitor(BatchEvaluationCompetitor competitor)
        {
            var competitorTask = new CompetitorEvaluationTask(
                null,
                competitor,
                mProblems,
                mCommandLineTemplates
            );
            competitorTask.CommandRan += HandleCommandLineRan;
            competitorTask.TestEvaluated += CompetitorTask_TestEvaluated;
            competitorTask.SolutionGraded += CompetitorTask_SolutionGraded;
            competitorTask.Start();
            competitorTask.ExecutingTask.Wait();
        }

        private void CompetitorTask_SolutionGraded(object sender, BatchEvaluationSolutionGraderTask.SolutionGradedEventArgs e) =>
            Notify(SolutionGraded, e);

        private void CompetitorTask_TestEvaluated(object sender, CompetitorEvaluationTask.TestEvaluatedEventArgs e) =>
            Notify(TestEvaluated, e);

        private void HandleCompetitorDiscovered(object sender, CompetitorInfoExtractedEventArgs e)
        {
            while (mDiscoveredCompetitors.Count <= e.Index) mDiscoveredCompetitors.Add(null);
            mDiscoveredCompetitors[e.Index] = new BatchEvaluationCompetitor(e.Index, e.Directory);
            Notify(CompetitorDiscovered, e);
        }

        private void HandleCommandLineRan(object sender, CommandLineRunningTask.CommandRanEventArgs e)
        {
            Notify(CommandLineExecuted, e);
        }
    }
}
