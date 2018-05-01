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
            IReadOnlyList<CrashReport> finds = CrashReportFinder.Instance.FindAll(7748, null, 1);
            ;
        }
    }
}
 