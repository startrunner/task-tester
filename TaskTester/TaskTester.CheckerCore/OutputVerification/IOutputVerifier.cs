using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    interface IOutputVerifier
    {
        bool UsesStdIn { get; }
        bool UsesStdout { get; }
        bool UsesStdErr { get; }
        bool UsesSolution { get; }

        OutputVerificationResult Verify(IProcessInterface processInterface);
    }
}
