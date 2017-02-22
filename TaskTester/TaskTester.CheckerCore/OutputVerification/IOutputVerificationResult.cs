using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerificationResult
    {
        OutputVerificationResultType Type { get; }
        double Score { get; }
        ICrashReport CrashReport { get; set; }
    }

    public class OutputVerificationResultMutable:IOutputVerificationResult
    {
        public OutputVerificationResultType Type { get; set; }
        public ICrashReport CrashReport { get; set; }
        public double Score { get; set; }
    }
}
