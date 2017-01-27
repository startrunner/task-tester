using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore
{
    public enum ProcessExitType
    {
        Graceful, Forced, Crashed,

        UnknownImpossibleForbiddenWhatTheHell
    }
}
