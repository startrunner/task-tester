using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskTester.CheckerCore.Tests.Properties;

namespace TaskTester.CheckerCore.Tests
{
    [TestClass]
    public class CheckerCoreTests
    {

        static string dummyPath = null;

        [TestMethod]
        public void TestSlow()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromSeconds(1),
                StdIn = "wait 200 sum 1 5   wait 10000  exit 0"
            };
            var result = runner.Run();
            Assert.IsFalse(result.TimelyExit);
            Assert.IsTrue(result.StdOut.Contains("6"));
        }

        [TestMethod]
        public void TestAlmostSlow()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromSeconds(0.5),
                StdIn = "wait 200 sum 1 5   wait 200  exit 0"
            };
            var result = runner.Run();
            Assert.IsTrue(result.TimelyExit);
            Assert.IsTrue(result.StdOut.Contains("6"));
            ;
        }

        [TestMethod]
        public void TestExit()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromHours(1),
                StdIn = "exit 3"
            };
            var result = runner.Run();

            Assert.AreEqual(result.ExitCode, 3);
            Assert.IsTrue(result.TimelyExit);
            Assert.IsTrue(result.GracefulExit);
        }

        [TestMethod]
        public void TestCrash()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromHours(1),
                StdIn = "sum 1 2 crash"
            };
            var result = runner.Run();
            Assert.IsFalse(result.GracefulExit);
            Assert.IsTrue(result.TimelyExit);
            ;
        }

        [TestMethod]
        public void TestOutput()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromSeconds(0.1),
                StdIn = "sum 1 2 sum 3 5 end"
            };
            var result = runner.Run();
            Assert.IsTrue(result.GracefulExit);
            Assert.IsTrue(result.TimelyExit);
            var output = result.StdOut;
            Assert.IsTrue(output.IndexOf('3') != -1 && output.IndexOf('8') > output.IndexOf('3'));
            ;
        }

        static CheckerCoreTests()
        {
            dummyPath = Path.GetTempFileName()+".exe";
            StreamWriter writer = new StreamWriter(dummyPath);
            var bytes = Resources.dummy;
            writer.BaseStream.Write(bytes, 0, bytes.Length);
            writer.Close();
        }
    }
}
