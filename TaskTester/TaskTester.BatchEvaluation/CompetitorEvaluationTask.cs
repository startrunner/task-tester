﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore;
using TaskTester.CheckerCore.SolutionEvalutation;
using TaskTester.Tasking;

namespace TaskTester.BatchEvaluation
{
    public sealed class CompetitorEvaluationTask : BackgroundTask
    {
        #region EventArgs
        public class TestEvaluatedEventArgs
        {
            public BatchEvaluationCompetitor Competitor { get; internal set; }
            public BatchEvaluationProblem Problem { get; internal set; }
            public SolutionEvaluationTestResult TestResult { get; internal set; }
        }
        #endregion

        readonly BatchEvaluationCompetitor mCompetitor;
        readonly IReadOnlyList<BatchEvaluationProblem> mProblems;
        readonly IReadOnlyList<string> mCommandLineTemplates;

        public event EventHandler<CommandLineRunningTask.CommandRanEventArgs> CommandRan;
        public event EventHandler<TestEvaluatedEventArgs> TestEvaluated;
        public event EventHandler<BatchEvaluationSolutionGraderTask.SolutionGradedEventArgs> SolutionGraded;

        public CompetitorEvaluationTask(
            Dispatcher eventDispatcher,
            BatchEvaluationCompetitor competitor,
            IReadOnlyList<BatchEvaluationProblem> problems,
            IReadOnlyList<string> commandLineTemplates
        ) : base(eventDispatcher)
        {
            mCompetitor = competitor;
            mProblems = problems;
            mCommandLineTemplates = commandLineTemplates;
        }

        public override void Start() =>
            this.ExecutingTask = Task.Run(action: Run);

        private void Run()
        {
            var commandLineTask = new CommandLineRunningTask(
                null,
                mCommandLineTemplates,
                mProblems,
                mCompetitor
            );
            commandLineTask.Start();
            commandLineTask.CommandRan += CommandLineTask_CommandRan;
            commandLineTask.ExecutingTask.Wait();

            foreach (BatchEvaluationProblem problem in mProblems)
            {
                RunProblem(problem);
            }

            NotifyFinished();
        }

        private void RunProblem(BatchEvaluationProblem problem)
        {
            IReadOnlyList<SolutionEvaluationTestResult> testResults = new SolutionEvaluationTestResult[0];

            if (TryGetConsoleApplication(problem, out IConsoleApplication application))
            {
                var solutionTask = new SolutionEvaluationTask(
                    null,
                    application,
                    problem.Problem.Tests
                );
                solutionTask.TestEvaluated += (x, e) => Notify(
                    TestEvaluated,
                    new TestEvaluatedEventArgs {
                        TestResult = e,
                        Competitor = mCompetitor,
                        Problem = problem
                    }
                );
                solutionTask.Start();
                solutionTask.ExecutingTask.Wait();
                testResults = solutionTask.FinishedTests;
            }

            GradeProblem(
                problem,
                testResults
            );
        }

        private void GradeProblem(BatchEvaluationProblem problem, IReadOnlyList<SolutionEvaluationTestResult> testResults)
        {
            var graderTask = new BatchEvaluationSolutionGraderTask(
                null,
                mCompetitor,
                problem,
                testResults
            );

            graderTask.SolutionGraded += (x, e) => Notify(SolutionGraded, e);
            graderTask.Start();
            graderTask.ExecutingTask.Wait();
        }

        bool TryGetConsoleApplication(BatchEvaluationProblem problem, out IConsoleApplication application)
        {
            string path = Path.Combine(
                mCompetitor.Directory,
                problem.Identifier + ".exe"
            );

            if(!File.Exists(path))
            {
                application = null;
                return false;
            }

            application = new FileSystemConsoleApplication(path);
            return true;
        }


        private void CommandLineTask_CommandRan(object sender, CommandLineRunningTask.CommandRanEventArgs e) =>
            Notify(CommandRan, e);
    }
}