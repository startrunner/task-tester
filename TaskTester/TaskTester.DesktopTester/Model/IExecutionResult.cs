﻿using System;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.DesktopTester.Model
{
    internal interface IExecutionResult
    {
        TimeSpan? ExecutionTime { get; }
        string ExpectedAnswer { get; }
        int IdentifierIndex { get; }
        string SolutionAnswer { get; }
        TestResultType Type { get; }
        CrashReport CrashReport { get; }
        double Score { get; }
    }

    class ExecutionResultMutable : IExecutionResult
    {
        public CrashReport CrashReport { get; set; }
        public TimeSpan? ExecutionTime { get; set; }
        public string ExpectedAnswer { get; set; }
        public int IdentifierIndex { get; set; }
        public string SolutionAnswer { get; set; }
        public TestResultType Type { get; set; }
        public double Score { get; set; } = 0;
    }
}
