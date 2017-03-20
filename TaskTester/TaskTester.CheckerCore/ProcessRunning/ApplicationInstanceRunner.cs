using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class ApplicationInstanceRunner
    {
        object thisLock = new object();
        int processID;
        Process process;
        bool used = false;//Object is single-use
        StringBuilder stdErrBuilder = new StringBuilder();
        StringBuilder stdOutBuilder = new StringBuilder();

        public string ExecutablePath { get; private set; }
        public StringOrFile StdIn { get; set; }
        public bool AllowCrashReports { get; set; } = true;

        public TimeSpan MaxRuntime { get; set; }
        public string ProcessArguments { get; internal set; }

        public ApplicationInstanceRunner(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        public async Task<IProcessRunResult> GoAsync()
        {
            if(used)
            {
                throw new InvalidOperationException($"Object of type {nameof(ApplicationInstanceRunner)} are single-use.");
            }
            used = true;
            StartProcess();
            AttachProcessEvents();

            bool timelyExit = await WaitForTimelyExit();
            ICrashReport report = await GetCrashReportIfNecessary();
            bool crashed = report != null;

            string stdErr, stdOut;

            lock (thisLock)
            {
                stdErr = stdErrBuilder.ToString();
                stdOut = stdOutBuilder.ToString();
            }

            var rt = new ProcessRunResultMutable()
            {
                ExitCode = process.ExitCode,
                MemoryUsed = 100.0,
                StdErr = StringOrFile.FromText(stdErr),
                StdOut = StringOrFile.FromText(stdOut),
                CrashReport = report,
                ExitType = DeduceExitType(crashed, timelyExit),
                ExecutionTime = process.ExitTime - process.StartTime
            };

            return rt;
        }

        private void StartProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(ExecutablePath) {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false,
                Arguments=ProcessArguments
            };

            process = ProcessParent.Instance.StartProcess(startInfo);
            processID = process.Id;
            Debug.WriteLine($"Started process {process.Id}");
        }

        void AttachProcessEvents()
        {
            process.OutputDataReceived += ((sender, e) =>
            {
                lock (thisLock)
                {
                    stdOutBuilder.AppendLine(e.Data);
                }
            });
            process.ErrorDataReceived += ((sender, e) =>
            {
                lock (thisLock)
                {
                    stdErrBuilder.AppendLine(e.Data);
                }
            });
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private async Task EnterInputAndWaitForExitAsync()
        {
            await process.StandardInput.WriteLineAsync(StdIn.Str);
            try
            {
                process.StandardInput.Close();
            }
            catch { }
            process.WaitForExit();
        }

        private async Task<bool> WaitForTimelyExit()
        {
            bool timelyExit = true;

            await
            Task.WhenAny(
            Task.Delay(MaxRuntime),
            Task.Run(EnterInputAndWaitForExitAsync))
            .ContinueWith(x =>
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    timelyExit = false;
                }
                else timelyExit = true;
            });

            return timelyExit;
        }

        private async Task<ICrashReport> GetCrashReportIfNecessary()
        {
            if (!AllowCrashReports) return null;

            ICrashReport report = null;
            if (process.ExitCode != 0)
            {
                //something fishy with this exit code, check for crashes
                var crashReportFinder = new CrashReportFinder(processID) {
                    ExecutablePath = ExecutablePath,
                    MaxReportCount = 1
                };
                report = (await crashReportFinder.FindAsync()).FirstOrDefault();
            }
            return report;
        }

        private ProcessExitType DeduceExitType(bool crashed, bool timelyExit)
        {
            if (timelyExit && !crashed) return ProcessExitType.Graceful;
            else if (crashed) return ProcessExitType.Crashed;
            else if (!timelyExit) return ProcessExitType.Forced;
            else return ProcessExitType.Undetermined;
        }
    }
}
