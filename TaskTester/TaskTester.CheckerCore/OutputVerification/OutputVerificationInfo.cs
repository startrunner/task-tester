using System;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class OutputVerificationInfo 
    {
        public OutputVerificationInfo(
            int exitCode, 
            StringOrFile standardError,
            StringOrFile standardInput, 
            StringOrFile standardOutput,
            StringOrFile expectedOutput)
        {
            ExitCode = exitCode;
            StandardError = standardError;
            StandardInput = standardInput;
            StandardOutput = standardOutput;
            ExpectedOutput = expectedOutput;
        }

        public int ExitCode { get;  }

        public StringOrFile StandardError { get;  }
        public StringOrFile StandardInput { get;  }
        public StringOrFile StandardOutput { get;  }
        public StringOrFile ExpectedOutput { get;  }
    }
}
