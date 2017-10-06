using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.Tests
{
    [TestClass]
    public class CrashReportFinderTests
    {
        [TestMethod]
        public void TestCrashReportFinder()
        {
            CrashReportFinder finder = new CrashReportFinder(7748, null, 1);
            IReadOnlyList<CrashReport> finds = finder.Find();
            ;
        }
    }
}
 