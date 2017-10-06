using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerifier
    {
        Task<OutputVerificationResult> VerifyAsync(OutputVerificationInfo info);
        OutputVerificationResult Verify(OutputVerificationInfo info);
    }
}
