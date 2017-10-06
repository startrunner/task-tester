using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification.ResultBinders
{
    public class ExitCodeBinder : IVerifierResultBinder
    {
        public int DesiredExitCode { get; private set; }
        public OutputVerificationResult ResultOnBind { get; private set; }

        public bool TryBind(ProcessRunResult checkerRun, out OutputVerificationResult result)
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

        public ExitCodeBinder(int desiredExitCode, OutputVerificationResult resultOnBind)
        {
            this.DesiredExitCode = desiredExitCode;
            this.ResultOnBind = resultOnBind;
        }
    }
}
