using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public enum VerifierArgumentType
    {
        None,
        Stdin,
        Stdout,
        Stderr,
        Solution,
        FileStdin,
        FileStdout,
        FileStderr,
        FileSolution,
        ExitCode,
    }
}
