using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerifier
    { 
        OutputVerificationResult Verify(OutputVerificationInfo info);
    }
}
