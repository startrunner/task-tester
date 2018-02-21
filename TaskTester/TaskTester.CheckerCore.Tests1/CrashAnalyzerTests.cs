using NUnit.Framework;
using System.Collections.Generic;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.Tests
{
    [TestFixture]
    public class CrashReportFinderTests
    {
        [Test]
        public void TestCrashReportFinder()
        {
            CrashReportFinder finder = new CrashReportFinder(7748, null, 1);
            IReadOnlyList<CrashReport> finds = finder.Find();
            ;
        }
    }
}
 