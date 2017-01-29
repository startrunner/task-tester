using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification.ResiltBindings
{
    public class StdOutContainsBinding : IVerifierResultBinding
    {
        public IOutputVerificationResult ResultOnBind { get; private set; }
        public string SearchText { get; private set; }

        public bool TryBind(IProcessRunResult checkerRun, out IOutputVerificationResult result)
        {
            if(checkerRun.StdOut.Str.Contains(SearchText))
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

        public StdOutContainsBinding(string searchText, IOutputVerificationResult result)
        {
            this.SearchText = searchText;
            this.ResultOnBind = result;
        }
    }
}
