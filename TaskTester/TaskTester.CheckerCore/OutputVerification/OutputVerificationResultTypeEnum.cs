using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    //[Obsolete]
    public enum OutputVerificationResultType
    {
        CorrectAnswer,
        PartiallyCorrectAnswer,
        WrongAnswer,
        CouldNotBind,
        CheckerCrashed
    }
}
