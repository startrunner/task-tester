using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerifier
    {
        Task<IOutputVerificationResult> VerifyAsync(IOutputVerificationInfo info);
        IOutputVerificationResult Verify(IOutputVerificationInfo info);
    }
}
