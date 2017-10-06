using System;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class OutputVerificationInfo 
    {
        public OutputVerificationInfo(int exitCode, StringOrFile standardError, StringOrFile standardInput, StringOrFile standardOutput, StringOrFile solFile)
        {
            ExitCode = exitCode;
            StandardError = standardError;
            StandardInput = standardInput;
            StandardOutput = standardOutput;
            SolFile = solFile;
        }

        public int ExitCode { get;  }

        public StringOrFile StandardError { get;  }
        public StringOrFile StandardInput { get;  }
        public StringOrFile StandardOutput { get;  }
        public StringOrFile SolFile { get;  }
    }
}
