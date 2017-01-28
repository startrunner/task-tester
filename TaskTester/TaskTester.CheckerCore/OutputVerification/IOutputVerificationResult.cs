using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerificationResult
    {
        OutputVerificationType Type { get; }
        double Score { get; }
    }

    public class OutputVerificationResultMutable:IOutputVerificationResult
    {
        public OutputVerificationType Type { get; set; }
        public double Score { get; set; }
    }
}
