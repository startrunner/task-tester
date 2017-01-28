using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    interface IOutputVerifier
    {
        OutputVerificationResult Verify(StringOrFile input, StringOrFile output, StringOrFile solution);
    }
}
