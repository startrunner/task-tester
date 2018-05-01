using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.ProcessRunning;
using TaskTester.Tasking;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public class SolutionEvaluationTask : BackgroundTask
    {
        readonly IConsoleApplication mApplication;
        readonly IReadOnlyList<SolutionTest> mTests;
        readonly List<SolutionEvaluationTestResult> mFinishedTests = new List<SolutionEvaluationTestResult>();

        public IReadOnlyList<SolutionEvaluationTestResult> FinishedTests => mFinishedTests;
        public int TotalTestCount => mTests.Count;

        public event EventHandler<SolutionEvaluationTestResult> TestEvaluated;

        public SolutionEvaluationTask(
            Dispatcher eventDispatcher,
            CancellationToken cancellationToken,
            IConsoleApplication mApplication,
            IReadOnlyList<SolutionTest> mTests
        ) : base(eventDispatcher, cancellationToken)
        {
            if (mApplication == null)
                throw new ArgumentNullException(nameof(mApplication));
            if (mTests == null)
                throw new ArgumentNullException(nameof(mTests));

            this.mApplication = mApplication;
            this.mTests = mTests;
        }

        public override void Start() => Start(Run);

        private void Run()
        {
            mCancellationToken.ThrowIfCancellationRequested();

            SolutionEvaluationTask that = this;
            for (int testIndex = 0, testCount = that.mTests.Count; testIndex < testCount; testIndex++)
            {
                that.RunTestAndNotify(testIndex);
                mCancellationToken.ThrowIfCancellationRequested();
            }
        }

        private void RunTestAndNotify(int testIndex)
        {
            SolutionEvaluationTestResult result = RunTest(testIndex);
            mFinishedTests.Add(result);
            Notify(TestEvaluated, result);
        }

        private SolutionEvaluationTestResult RunTest(int testIndex)
        {
            SolutionTest test = mTests[testIndex];

            ProcessRunResult runResult = ConsoleApplicationRunner.Instance.Run(
                application: mApplication,
                stdIn: test.Input,
                maxRuntime: test.TimeLimit,
                processArguments: test.ProcessArguments,
                allowCrashReports: true
            );

            if (TryTranslateUngracefulExit(runResult.ExitType, out SolutionEvaluationTestResultType testResultType))
            {
                return new SolutionEvaluationTestResult {
                    ExpectedOutput = test.ExpectedOutput,
                    RunResult = runResult,
                    Score = 0,
                    Type = testResultType,
                    TestGroup = test.TestGroup
                };
            }

            if (runResult.ExitType != ProcessExitType.Graceful) throw new NotImplementedException();

            OutputVerificationResult verificationResult = test.OutputVerifier.Verify(new OutputVerificationInfo(
                exitCode: runResult.ExitCode,
                standardInput: test.Input,
                standardError: runResult.StandardError,
                standardOutput: runResult.Output,
                expectedOutput: test.ExpectedOutput
            ));

            return CreateTestResult(test, runResult, verificationResult);
        }

        private SolutionEvaluationTestResult CreateTestResult(SolutionTest test, ProcessRunResult runResult, OutputVerificationResult verificationResult)
        {
            return new SolutionEvaluationTestResult() {
                ExpectedOutput = test.ExpectedOutput,
                RunResult = runResult,
                Score = verificationResult.ScoreMultiplier * test.MaxScore,
                Type = TranslateVerificationResultType(verificationResult.Type),
                TestGroup = test.TestGroup
            };

        }
        private SolutionEvaluationTestResultType TranslateVerificationResultType(OutputVerificationResultType type)
        {
            switch (type)
            {
                case OutputVerificationResultType.CheckerCrashed:
                    return SolutionEvaluationTestResultType.CheckerCrashed;
                case OutputVerificationResultType.CorrectAnswer:
                    return SolutionEvaluationTestResultType.CorrectAnswer;
                case OutputVerificationResultType.CouldNotBind:
                    return SolutionEvaluationTestResultType.CheckerCouldNotBind;
                case OutputVerificationResultType.PartiallyCorrectAnswer:
                    return SolutionEvaluationTestResultType.PartiallyCorrectAnswer;
                case OutputVerificationResultType.WrongAnswer:
                    return SolutionEvaluationTestResultType.WrongAnswer;
                default:
                    throw new NotImplementedException();
            }
        }



        private bool TryTranslateUngracefulExit(ProcessExitType processExitType, out SolutionEvaluationTestResultType type)
        {
            switch (processExitType)
            {
                case ProcessExitType.Crashed:
                    type = SolutionEvaluationTestResultType.ProgramCrashed;
                    return true;
                case ProcessExitType.Timeout:
                    type = SolutionEvaluationTestResultType.TimeLimitExceeded;
                    return true;
                default:
                    type = default(SolutionEvaluationTestResultType);
                    return false;
            }
        }
    }
}
