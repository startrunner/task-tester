using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IProcessInterface
    {
        int ExitCode { get; }
        StringOrFile StandardInput { get; }
        StringOrFile StandardOutput { get; }
        StringOrFile StandardError { get; }
    }

    public class ProcessInterfaceMutable : IProcessInterface
    {
        public int ExitCode { get; set; }
        public StringOrFile StandardError { get; set; }
        public StringOrFile StandardInput { get; set; }
        public StringOrFile StandardOutput { get; set; }
    }
}
