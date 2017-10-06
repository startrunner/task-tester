using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.OutputVerification
{

    public class OutputVerificationResult
    {
        public OutputVerificationResult(OutputVerificationResultType type, CrashReport crashReport, double score)
        {
            Type = type;
            CrashReport = crashReport;
            Score = score;
        }

        public OutputVerificationResultType Type { get;  }
        public CrashReport CrashReport { get;  }
        public double Score { get;  }
    }
}
