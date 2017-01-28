using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerifier
    {
        bool UsesStdIn { get; }
        bool UsesStdout { get; }
        bool UsesStdErr { get; }
        bool UsesSolution { get; }

        IOutputVerificationResult Verify(IProcessVerificationInfo info);
    }
}
