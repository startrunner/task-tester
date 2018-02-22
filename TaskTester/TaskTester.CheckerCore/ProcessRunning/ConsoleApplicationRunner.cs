using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.CrashReporting;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ConsoleApplicationRunner
    {
        public static readonly ConsoleApplicationRunner Instance = new ConsoleApplicationRunner();

        private ConsoleApplicationRunner() { }

        public Task<ProcessRunResult> RunAsync(
            IConsoleApplication application,
            TimeSpan maxRuntime,
            StringOrFile stdIn,
            string processArguments = null,
            bool allowCrashReports = true
        )
        {
            ProcessRunArguments args = new ProcessRunArguments(
                application,
                stdIn,
                maxRuntime,
                processArguments,
                allowCrashReports
            );

            return RunAsync(args);
        }


        private async Task<ProcessRunResult> RunAsync(ProcessRunArguments args)
        {
            ApplicationRunnerContext context = await StartProcessAndCreateContextAsync(args);

            bool timelyExit = await TryWaitForTimelyExitAsync(context.process, args);
            CrashReport report = await GetCrashReportIfNecessary(context.process, context.processID, args);
            bool crashed = report != null;

            string stdOut = context.GetStandardOutput();
            string stdErr = context.GetStandardError();

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
            IConsoleApplication consoleApplication,
            TimeSpan maxRuntime,
            StringOrFile stdIn,
            string processArguments = null,
            bool allowCrashReports = true
        ) =>
            RunAsync(consoleApplication, maxRuntime, stdIn, processArguments, allowCrashReports).Result;



        private async Task<ApplicationRunnerContext> StartProcessAndCreateContextAsync(ProcessRunArguments args)
        {
            Process process = await StartProcessAsync(args);

            ApplicationRunnerContext context = new ApplicationRunnerContext(process);

            process.OutputDataReceived += (sender, e) => context.AppendStandardOutput(e.Data);
            process.ErrorDataReceived += (sender, e) => context.AppendStandardError(e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return context;
        }

        private async Task<Process> StartProcessAsync(ProcessRunArguments args)
        {
            await Task.Yield();
            ProcessStartInfo startInfo = new ProcessStartInfo(args.Application.ExecutablePath) {
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

        private async Task EnterInputAndWaitForExitAsync(Process process, ProcessRunArguments args)
        {
            await process.StandardInput.WriteLineAsync(args.StandardInput.StringValue);

            try { process.StandardInput.Close(); }
            catch { }

            process.WaitForExit();
        }

        private async Task<bool> TryWaitForTimelyExitAsync(Process process, ProcessRunArguments args)
        {
            bool timelyExit = true;

            await
            Task.WhenAny(
                Task.Delay(args.MaxRuntime),
                Task.Run(() => EnterInputAndWaitForExitAsync(process, args))
            )
            .ContinueWith(x =>
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    timelyExit = false;
                }
                else { timelyExit = true; }
            });

            return timelyExit;
        }

        private async Task<CrashReport> GetCrashReportIfNecessary(Process process, int processID, ProcessRunArguments args)
        {
            if (!args.AllowCrashReports) { return null; }

            CrashReport report = null;
            if (process.ExitCode != 0)
            {
                //something fishy with this exit code, check for crashes
                var crashReportFinder = new CrashReportFinder(processID, args.Application.ExecutablePath, 1);
                report = (await crashReportFinder.FindAsync()).FirstOrDefault();
            }
            return report;
        }

        private ProcessExitType DeduceExitType(bool crashed, bool timelyExit)
        {
            if (timelyExit && !crashed)
            { return ProcessExitType.Graceful; }

            else if (crashed)
            { return ProcessExitType.Crashed; }

            else if (!timelyExit)
            { return ProcessExitType.Timeout; }

            else
            { return ProcessExitType.Undetermined; }
        }
    }
}
