using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IVerifierResultBinder
    {
        bool TryBind(ProcessRunResult checkerRun, out OutputVerificationResult result);
    }
}