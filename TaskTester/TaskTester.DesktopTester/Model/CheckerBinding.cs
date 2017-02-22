using TaskTester.CheckerCore.OutputVerification;

namespace TaskTester.DesktopTester.Model
{
    class CheckerBinding
    {
        public string SearchString { get; set; } = "";
        public double Score { get; set; }
        public OutputVerificationResultType Type { get; set; }
    }
}
