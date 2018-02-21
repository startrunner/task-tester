using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public sealed class SolutionEvaluationTestResult
    {
        public SolutionEvaluationTestResultType Type { get; internal set; }
        public double Score { get; internal set; }
        public StringOrFile ExpectedOutput { get; internal set; }
        public ProcessRunResult RunResult { get; internal set; }
        public string TestGroup { get; internal set; } = "";
    }
}
