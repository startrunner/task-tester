using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerifier
    {
        Task<IOutputVerificationResult> VerifyAsync(IOutputVerificationInfo info);
        IOutputVerificationResult Verify(IOutputVerificationInfo info);
    }
}
