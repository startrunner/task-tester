using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class DefaultOutputVerifier : IOutputVerifier
    {
        public bool UsesSolution => true;
        public bool UsesStdout => true;
        public bool UsesStdErr => false;
        public bool UsesStdIn => false;


        public OutputVerificationResult Verify(IProcessInterface processInterface)
        {
            //TODO: Implement checking
            throw new NotImplementedException();
        }
    }
}
