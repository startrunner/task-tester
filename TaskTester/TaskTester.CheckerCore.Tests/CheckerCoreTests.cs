using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.ProcessRunning;
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
            ApplicationRunner runner = new ApplicationRunner();
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromSeconds(1),
                StringOrFile.FromText("wait 200 sum 1 5   wait 10000  exit 0")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Forced);
            Assert.IsTrue(result.StdOut.ToString().Contains("6"));
        }

        [TestMethod]
        public void TestAlmostSlow()
        {
            ApplicationRunner runner = new ApplicationRunner();
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromSeconds(0.5),
                StringOrFile.FromText("wait 200 sum 1 5   wait 200  exit 0")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            Assert.IsTrue(result.StdOut.ToString().Contains("6"));
            ;
        }

        [TestMethod]
        public void TestExit()
        {
            ApplicationRunner runner = new ApplicationRunner();
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromHours(1),
                StringOrFile.FromText("exit 3")
            );

            Assert.AreEqual(result.ExitCode, 3);
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
        }

        [TestMethod]
        public void TestCrash()
        {
            ApplicationRunner runner = new ApplicationRunner();
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromSeconds(1),
                StringOrFile.FromText("sum 1 2 crash")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Crashed);
            Assert.IsNotNull(result.CrashReport);
        }

        [TestMethod]
        public void TestOutput()
        {
            ApplicationRunner runner = new ApplicationRunner();
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromSeconds(0.1),
                StringOrFile.FromText("sum 1 2 sum 3 5 end")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            StringOrFile output = result.StdOut;
            Assert.IsTrue(output.Str.IndexOf('3') != -1 && output.Str.IndexOf('8') > output.Str.IndexOf('3'));
            ;
        }

        [TestMethod]
        public void TestContinuity()
        {
            ApplicationRunner runner = new ApplicationRunner( );
            ProcessRunResult result = runner.Run(
                dummyPath,
                TimeSpan.FromHours(1),
                StringOrFile.FromText("continuity 100000     end")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            StringOrFile output = result.StdOut;
            string[] lines = output.Str
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            int prev = int.Parse(lines.First().Split(' ')[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                int current = int.Parse(lines[i].Split(' ')[0]);
                Assert.IsTrue(current == prev + 1);
                prev = current;
            }
            ;
        }

        static CheckerCoreTests()
        {
            dummyPath = Path.GetTempFileName() + ".exe";
            StreamWriter writer = new StreamWriter(dummyPath);
            byte[] bytes = Resources.dummy;
            writer.BaseStream.Write(bytes, 0, bytes.Length);
            writer.Close();
        }
    }
}
