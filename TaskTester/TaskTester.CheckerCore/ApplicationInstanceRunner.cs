using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore
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
            process = StartProcess();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var startTime = DateTime.Now;

            bool timelyExit = true;

            await
                Task.WhenAny(
                Task.Delay(MaxRuntime),
                Task.Run(EnterInputAndWaitForExitAsync))
                .ContinueWith(x =>
                {
                    if (!process.HasExited)
                    {
                        //process hasn't exited in time:
                        //A: it's too slow
                        //B: it has crashed and is saving an error report
                        if (!process.WaitForExit(1))
                        {
                            //give it some time in case it's saving an error report
                            process.Kill();
                        }
                        timelyExit = false;
                    }
                    else timelyExit = true;
                });

            CrashReport report=null;

            if (process.ExitCode != 0)
            {
                //something fishy here, check for crashes
                var crashReportFinder = new CrashReportFinder(processID) {
                    ExecutablePath = ExecutablePath,
                    MaxReportCount = 1
                };
                report = (await crashReportFinder.FindAsync()).FirstOrDefault();
            }

            return new ProcessRunResult() {
                ExitCode = process.ExitCode,
                TimelyExit = timelyExit,
                MemoryUsed = 100.0,
                StdErr = stdErrBuilder.ToString(),
                StdOut = stdOutBuilder.ToString(),
                CrashReport = report
            };

            throw new NotImplementedException();
        }

        private Process StartProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(ExecutablePath) {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false
            };

            var process = ProcessParent.Instance.StartProcess(startInfo);
            processID = process.Id;
            Debug.WriteLine($"Started process {process.Id}");
            return process;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            stdErrBuilder.Append(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            stdOutBuilder.AppendLine(e.Data);
        }

        public async Task EnterInputAndWaitForExitAsync()
        {
            await process.StandardInput.WriteLineAsync(StdIn);
            process.WaitForExit();
        }
    }
}
