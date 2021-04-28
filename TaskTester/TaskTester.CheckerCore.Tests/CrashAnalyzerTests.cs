using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TaskTester.CheckerCore.CrashReporting;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.Tests
{
    [TestFixture]
    public class CrashReportFinderTests
    {
        [Test]
        public void TestCrashReportFinder()
        {
            IReadOnlyList<ICrashReport> finds = CrashReportFinder.Instance.FindCrashReports(7748, null, 1).ToList();
            ;
        }
    }
}
 