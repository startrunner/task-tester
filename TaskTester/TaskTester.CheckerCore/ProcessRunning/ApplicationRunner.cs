using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ApplicationRunner : IDisposable
    {
        public Task<ProcessRunResult> RunAsync(
            string executablePath,
            TimeSpan maxRuntime,
            StringOrFile stdIn,
            string processArguments = null,
            bool allowCrashReports = true
        )
        {
            ApplicationRunArguments args = new ApplicationRunArguments(
                executablePath,
                stdIn,
                maxRuntime,
                processArguments,
                allowCrashReports
            );

            return RunAsync(args);
        }


        private async Task<ProcessRunResult> RunAsync(ApplicationRunArguments args)
        {
            ApplicationRunnerContext context = await StartProcessAndCreateContextAsync(args);

            bool timelyExit = await TryWaitForTimelyExitAsync(context.process, args);
            CrashReport report = await GetCrashReportIfNecessary(context.process, context.processID, args);
            bool crashed = report != null;

            string stdErr, stdOut;
            lock (context.outLock) { stdOut = context.GetStandardOutput(); }
            lock (context.errLock) { stdErr = context.GetStandardError(); }

            Process process = context.process;

            return new ProcessRunResult(
                DeduceExitType(crashed, timelyExit),
                report,
                StringOrFile.FromText(stdOut),
                StringOrFile.FromText(stdErr),
                100,//memory used
                process.ExitCode,
                process.ExitTime - process.StartTime
            );
        }


        public ProcessRunResult Run(
            string executablePath,
            TimeSpan maxRuntime,
            StringOrFile stdIn,
            string processArguments = null,
            bool allowCrashReports = true
        )
            => RunAsync(executablePath, maxRuntime, stdIn, processArguments, allowCrashReports).Result;


        private async Task<ApplicationRunnerContext> StartProcessAndCreateContextAsync(ApplicationRunArguments args)
        {
            Process process = await StartProcessAsync(args);

            ApplicationRunnerContext context = new ApplicationRunnerContext(process);

            process.OutputDataReceived += (sender, e) => context.AppendStandardOutput(e.Data);
            process.ErrorDataReceived += (sender, e) => context.AppendStandardError(e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return context;
        }

        private async Task<Process> StartProcessAsync(ApplicationRunArguments args)
        {
            await Task.Yield();
            ProcessStartInfo startInfo = new ProcessStartInfo(args.ExecutablePath) {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false,
                Arguments = args.ProcessArguments
            };

            return ProcessParent.Instance.StartProcess(startInfo);
        }

        private async Task EnterInputAndWaitForExitAsync(Process contextPorcess, ApplicationRunArguments args)
        {
            await contextPorcess.StandardInput.WriteLineAsync(args.StandardInput.Str);

            try { contextPorcess.StandardInput.Close(); }
            catch { }

            contextPorcess.WaitForExit();
        }

        private async Task<bool> TryWaitForTimelyExitAsync(Process contextProcess, ApplicationRunArguments args)
        {
            bool timelyExit = true;

            await
            Task.WhenAny(
            Task.Delay(args.MaxRuntime),
            Task.Run(() => EnterInputAndWaitForExitAsync(contextProcess, args)))
            .ContinueWith(x =>
            {
                if (!contextProcess.HasExited)
                {
                    contextProcess.Kill();
                    timelyExit = false;
                }
                else timelyExit = true;
            });

            return timelyExit;
        }

        private async Task<CrashReport> GetCrashReportIfNecessary(Process process, int processID, ApplicationRunArguments args)
        {
            if (!args.AllowCrashReports) { return null; }

            CrashReport report = null;
            if (process.ExitCode != 0)
            {
                //something fishy with this exit code, check for crashes
                var crashReportFinder = new CrashReportFinder(processID, args.ExecutablePath, 1);
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

        void IDisposable.Dispose() { }
    }
}
