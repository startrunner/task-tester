using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification.ResultBinders
{
    public class StdOutContainsBinder : IVerifierResultBinder
    {
        public OutputVerificationResult ResultOnBind { get; private set; }
        public string SearchText { get; private set; }

        public bool TryBind(ProcessRunResult checkerRun, out OutputVerificationResult result)
        {
            if(checkerRun.Output.StringValue.Contains(SearchText))
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

        public StdOutContainsBinder(string searchText, OutputVerificationResult result)
        {
            this.SearchText = searchText;
            this.ResultOnBind = result;
        }
    }
}
