using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.ProcessRunning;
using TaskTester.CheckerCore.SolutionEvalutation;
using TaskTester.CheckerCore.Tests;

namespace TaskTester.CheckerCore.Tests
{
    [TestFixture]
    public class CheckerCoreTests
    {
        static string DummyPath = Path.GetFullPath("dummy.exe".GetLocalFilePath());

        [Test]
        public void TestSlow()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromSeconds(1),
                StringOrFile.FromText("wait 200 sum 1 5   wait 10000  exit 0")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Timeout);
            Assert.IsTrue(result.Output.ToString().Contains("6"));
        }

        [Test]
        public void TestAlmostSlow()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromSeconds(0.5),
                StringOrFile.FromText("wait 200 sum 1 5   wait 200  exit 0")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            Assert.IsTrue(result.Output.ToString().Contains("6"));
            ;
        }

        [Test]
        public void TestExit()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromHours(1),
                StringOrFile.FromText("exit 3")
            );

            Assert.AreEqual(result.ExitCode, 3);
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
        }

        [Test]
        public void TestCrash()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromSeconds(1),
                StringOrFile.FromText("sum 1 2 crash")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Crashed);
            Assert.IsNotNull(result.CrashReport);
        }

        [Test]
        public void TestOutput()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromSeconds(0.1),
                StringOrFile.FromText("sum 1 2 sum 3 5 end")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            StringOrFile output = result.Output;
            Assert.IsTrue(output.StringValue.IndexOf('3') != -1 && output.StringValue.IndexOf('8') > output.StringValue.IndexOf('3'));
            ;
        }

        [Test]
        public void TestContinuity()
        {
            ProcessRunResult result = ConsoleApplicationRunner.Instance.Run(
                new FileSystemConsoleApplication(DummyPath, CheckerCore.CrashReporting.CrashReportFinder.Instance),
                TimeSpan.FromHours(1),
                StringOrFile.FromText("continuity 100000     end")
            );
            Assert.AreEqual(result.ExitType, ProcessExitType.Graceful);
            StringOrFile output = result.Output;
            string[] lines = output.StringValue
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
    }
}
