using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
