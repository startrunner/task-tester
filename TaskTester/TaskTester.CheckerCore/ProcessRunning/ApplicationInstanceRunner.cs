using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class ApplicationInstanceRunner
    {
        int processID;
        Process process;
        StringBuilder stdErrBuilder =
            new StringBuilder(), stdOutBuilder = new StringBuilder();
        public string ExecutablePath { get; private set; }
        public string StdIn { get; set; }

        public TimeSpan MaxRuntime { get; set; }

        public ApplicationInstanceRunner(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        public async Task<ProcessRunResult> GoAsync()
        {
            StartProcess();
            AttachProcessEvents();

            bool timelyExit = await WaitForTimelyExit();
            CrashReport report = await GetCrashReportIfNecessary();
            bool crashed = report != null;

            return new ProcessRunResult() {
                ExitCode = process.ExitCode,
                MemoryUsed = 100.0,
                StdErr = stdErrBuilder.ToString(),
                StdOut = stdOutBuilder.ToString(),
                CrashReport = report,
                ExitType = DeduceExitType(crashed, timelyExit)
            };
        }

        private void StartProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(ExecutablePath) {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false
            };

            process = ProcessParent.Instance.StartProcess(startInfo);
            processID = process.Id;
            Debug.WriteLine($"Started process {process.Id}");
        }

        void AttachProcessEvents()
        {
            process.OutputDataReceived += ((sender, e) => stdOutBuilder.Append(e.Data));
            process.ErrorDataReceived += ((sender, e) => stdErrBuilder.Append(e.Data));
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private async Task EnterInputAndWaitForExitAsync()
        {
            await process.StandardInput.WriteLineAsync(StdIn);
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
                    /*//process hasn't exited in time:
                    //A: it's too slow
                    //B: it has crashed and is saving an error report
                    if (!process.WaitForExit(1))
                    {
                        //give it some time in case it's saving an error report
                        process.Kill();
                    }*/

                    //EDIT: Error report is saved immediately after the crash, before the WER dialog;
                    //No need to wait.
                    process.Kill();
                    timelyExit = false;
                }
                else timelyExit = true;
            });

            return timelyExit;
        }

        private async Task<CrashReport> GetCrashReportIfNecessary()
        {
            CrashReport report = null;
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
