using System;
using System.IO;
using System.Linq;
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
            Assert.AreEqual(result.ExitType, ProcessExitType.Forced);
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
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
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
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
        }

        [TestMethod]
        public void TestCrash()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromSeconds(1),
                StdIn = "sum 1 2 crash"
            };
            var result = runner.Run();
            Assert.AreEqual(result.ExitType, ProcessExitType.Crashed);
            Assert.IsNotNull(result.CrashReport);
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
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            var output = result.StdOut;
            Assert.IsTrue(output.IndexOf('3') != -1 && output.IndexOf('8') > output.IndexOf('3'));
            ;
        }

        [TestMethod]
        public void TestContinuity()
        {
            ApplicationRunner runner = new ApplicationRunner(dummyPath) {
                MaxRuntime = TimeSpan.FromHours(1),
                StdIn = "continuity 100000     end"
            };
            var result = runner.Run();
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            var output = result.StdOut;
            string[] lines = output
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>x.Trim())
                .Where(x=>!string.IsNullOrWhiteSpace(x))
                .ToArray();

            int prev = int.Parse(lines.First().Split(' ')[0]);
            for(int i=1;i<lines.Length;i++)
            {
                int current = int.Parse(lines[i].Split(' ')[0]);
                Assert.IsTrue(current == prev + 1);
                prev = current;
            }
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
