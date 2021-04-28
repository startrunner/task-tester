using System;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ConsoleApplicationRunner
    {
        public static readonly ConsoleApplicationRunner Instance = new ConsoleApplicationRunner();

        private ConsoleApplicationRunner() { }

        public ProcessRunResult Run(
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

            return Run(args);
        }

        private ProcessRunResult Run(ProcessRunArguments args)
        {
            IConsoleProcess process = args.Application.StartProcess(args.ProcessArguments);

            bool timelyExit = TryWaitForTimelyExitAsync(process, args).GetAwaiter().GetResult();
            bool crashed = process.TryFindCrashReport(out ICrashReport report);

            string stdOut = process.GetStandardOutput();
            string stdErr = process.GetStandardError();

            return new ProcessRunResult(
                DeduceExitType(crashed, timelyExit),
                report,
                StringOrFile.FromText(stdOut),
                StringOrFile.FromText(stdErr),
                100,
                process.ExitCode,
                process.ExecutionTime
            );
        }

        private async Task EnterInputAndWaitForExitAsync(IConsoleProcess process, ProcessRunArguments args)
        {
            await Task.Yield();
            process.WriteStandardInput(args.StandardInput.StringValue);
            process.WriteStandardInput("\n");
            process.CloseStandardInput();

            process.WaitForExit((int)args.MaxRuntime.TotalMilliseconds + 300);
        }

        private async Task<bool> TryWaitForTimelyExitAsync(IConsoleProcess process, ProcessRunArguments args)
        {
            bool timelyExit = true;

            await Task.WhenAny(
                Task.Delay(args.MaxRuntime),
                EnterInputAndWaitForExitAsync(process, args)
            );

            if (!process.HasExited)
            {
                await Task.Delay(100);
                process.EnsureKilled();
                timelyExit = false;
            }
            else { timelyExit = true; }

            return timelyExit;
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
