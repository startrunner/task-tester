using System.Collections.Generic;
using System.Diagnostics;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public interface ICrashReportFinder
    {
        IEnumerable<ICrashReport> FindCrashReports(Process process, int maxReportCount = int.MaxValue);
    }
}
