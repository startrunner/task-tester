using System;
using System.Collections.Generic;
using System.Threading;
using TaskTester.CheckerCore.Tasking;
using TaskTester.DataExtraction;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationTask : TaskTesterJob
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
            Action<Delegate, object[]> eventDispatcher,
            CancellationToken cancellationToken,
            string rootDirectory,
            string directoryPathCriteria,
            IReadOnlyList<string> commandLineTemplates,
            IReadOnlyList<BatchEvaluationProblem> problems
        ) : base(eventDispatcher, cancellationToken)
        {
            //mEventDispatcher = eventDispatcher;
            mRootDirectory = rootDirectory;
            mDirectoryPathCriteria = directoryPathCriteria;
            mCommandLineTemplates = commandLineTemplates;
            mProblems = problems;
        }

        public override void Start() => Start(Run);

        private void Run()
        {
            mCancellationToken.ThrowIfCancellationRequested();
            var discoveryTask = new CompetitionDataExtractionTask(
                eventDispatcher: null,
                cancellationToken: mCancellationToken,
                rootDirectory: mRootDirectory,
                directoryPathCriteria: mDirectoryPathCriteria
            );
            discoveryTask.CompetitorInfoExtracted += HandleCompetitorDiscovered;
            discoveryTask.Start();
            discoveryTask.ExecutingTask.GetAwaiter().GetResult();

            foreach (BatchEvaluationCompetitor competitor in mDiscoveredCompetitors)
            {
                mCancellationToken.ThrowIfCancellationRequested();
                RunCompetitor(competitor);
            }

            NotifyFinished();
        }

        private void RunCompetitor(BatchEvaluationCompetitor competitor)
        {
            mCancellationToken.ThrowIfCancellationRequested();
            var competitorTask = new CompetitorEvaluationTask(
                eventDispatcher: null,
                cancellationToken: mCancellationToken,
                competitor: competitor,
                problems: mProblems,
                commandLineTemplates: mCommandLineTemplates
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
