using System;
using TaskTester.BatchEvaluation;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public class BatchTestTestResultViewModel
    {
        public BatchTestTestResultTypeViewModel Type { get; }
        public double Score { get; }
        public string TestGroup { get; }
        public bool HasTestGroup => !string.IsNullOrEmpty(TestGroup);
        public string CrashMessage { get; } = string.Empty;

        public BatchTestTestResultViewModel(CompetitorEvaluationTask.TestEvaluatedEventArgs e)
        {
            Type = TranslateType(e.TestResult.Type);
            Score = e.TestResult.Score;
            TestGroup = e.TestResult.TestGroup;
            CrashMessage = e.TestResult.RunResult?.CrashReport?.ExceptionMessage ?? "";
        }

        private static BatchTestTestResultTypeViewModel TranslateType(SolutionEvaluationTestResultType type)
        {
            switch (type)
            {
                case SolutionEvaluationTestResultType.CheckerCouldNotBind:
                    return BatchTestTestResultTypeViewModel.CouldNotBind;

                case SolutionEvaluationTestResultType.CheckerCrashed:
                    return BatchTestTestResultTypeViewModel.CheckerCrashed;

                case SolutionEvaluationTestResultType.CorrectAnswer:
                    return BatchTestTestResultTypeViewModel.CorrectAnswer;

                case SolutionEvaluationTestResultType.WrongAnswer:
                    return BatchTestTestResultTypeViewModel.WrongAnswer;

                case SolutionEvaluationTestResultType.MemoryLimitExceeded:
                    return BatchTestTestResultTypeViewModel.MemoryLimitExceeded;

                case SolutionEvaluationTestResultType.PartiallyCorrectAnswer:
                    return BatchTestTestResultTypeViewModel.PartiallyCorrectAnswer;

                case SolutionEvaluationTestResultType.ProgramCrashed:
                    return BatchTestTestResultTypeViewModel.ProgramCrashed;

                case SolutionEvaluationTestResultType.TimeLimitExceeded:
                    return BatchTestTestResultTypeViewModel.TimeLimitExceeded;

                default:
                    throw new NotImplementedException();

            }
        }
    }
}