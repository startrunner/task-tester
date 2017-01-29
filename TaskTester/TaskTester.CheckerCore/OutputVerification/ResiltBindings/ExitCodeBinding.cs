using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification.ResiltBindings
{
    public class ExitCodeBinding : IVerifierResultBinding
    {
        public int DesiredExitCode { get; private set; }
        public IOutputVerificationResult ResultOnBind { get; private set; }

        public bool TryBind(IProcessRunResult checkerRun, out IOutputVerificationResult result)
        {
            if(checkerRun.ExitCode==DesiredExitCode)
            {
                result = ResultOnBind;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public ExitCodeBinding(int desiredExitCode, IOutputVerificationResult resultOnBind)
        {
            this.DesiredExitCode = desiredExitCode;
            this.ResultOnBind = resultOnBind;
        }
    }
}
