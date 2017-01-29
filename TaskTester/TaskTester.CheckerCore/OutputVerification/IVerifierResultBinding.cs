using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IVerifierResultBinding
    {
        bool TryBind(IProcessRunResult checkerRun, out IOutputVerificationResult result);
    }
}