using System;
using TaskTester.CheckerCore.Common;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class TestResultViewModel
    {
        public TestResultTypeViewModel Type { get; set; }
        public double ScoreMultiplier { get; set; }
        public string CrashMessage { get; set; }
        public StringOrFile SolutionOutput { get; set; }
        public StringOrFile ExpectedOutput { get; set; }
        public TimeSpan ExecutionTime { get; set; }

        public string ExecutionTimeFormatted => $"{ExecutionTime.TotalSeconds:0.000}s";
    }
}