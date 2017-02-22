using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.Tests
{
    [TestClass]
    public class CrashReportFinderTests
    {
        [TestMethod]
        public void TestCrashReportFinder()
        {
            CrashReportFinder finder = new CrashReportFinder(7748);
            var finds = finder.Find();
            ;
        }
    }
}
 