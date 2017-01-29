using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IOutputVerificationResult
    {
        OutputVerificationResultType Type { get; }
        double Score { get; }
        ICrashReport CrashReport { get; set; }
    }

    public class OutputVerificationResultMutable:IOutputVerificationResult
    {
        public OutputVerificationResultType Type { get; set; }
        public ICrashReport CrashReport { get; set; }
        public double Score { get; set; }
    }
}
