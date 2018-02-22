using System;
using Newtonsoft.Json;
using TaskTester.CheckerCore.Common;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class TestResultViewModel
    {
        
        public TestResultTypeViewModel Type { get; set; }
        
        public double ScoreMultiplier { get; set; }
        
        public string CrashMessage { get; set; }
        
        [JsonIgnore]
        public StringOrFile SolutionOutput { get; set; }
        
        [JsonIgnore]
        public StringOrFile ExpectedOutput { get; set; }
        
        public TimeSpan ExecutionTime { get; set; }

        public string ExecutionTimeFormatted => $"{ExecutionTime.TotalSeconds:0.000}s";
    }
}