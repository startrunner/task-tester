﻿using System;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IProcessVerificationInfo
    {
        int ExitCode { get; }
        StringOrFile StandardInput { get; }
        StringOrFile StandardOutput { get; }
        StringOrFile StandardError { get; }
        StringOrFile SolFile { get; }
    }

    public class ProcessVerificationInfoMutable : IProcessVerificationInfo
    {
        public int ExitCode { get; set; }

        public StringOrFile StandardError { get; set; }
        public StringOrFile StandardInput { get; set; }
        public StringOrFile StandardOutput { get; set; }
        public StringOrFile SolFile { get; set; }
    }
}