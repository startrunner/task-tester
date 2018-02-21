using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.OutputVerification
{

    public class OutputVerificationResult
    {
        public OutputVerificationResult(OutputVerificationResultType type, double scoreMultiplier)
        {
            Type = type;
            ScoreMultiplier = scoreMultiplier;
        }

        public OutputVerificationResultType Type { get;  }
        public double ScoreMultiplier { get;  }
    }
}
