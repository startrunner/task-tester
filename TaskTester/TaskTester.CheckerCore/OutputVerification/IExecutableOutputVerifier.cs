using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.OutputVerification
{
    public interface IExecutableOutputVerifier : IOutputVerifier
    {
        string ExecutablePath { get; }
        IReadOnlyList<VerifierArgumentType> Arguments { get; }
        IReadOnlyList<IVerifierResultBinder> Bindings { get; }
        VerifierArgumentType Stdin { get; }
    }

    public class ExecutableOutputVerifierMutable : IExecutableOutputVerifier
    {
        public string ExecutablePath { get; set; }
        public IReadOnlyList<VerifierArgumentType> Arguments { get; set; } = new VerifierArgumentType[0];
        public IReadOnlyList<IVerifierResultBinder> Bindings { get; set; } = new IVerifierResultBinder[0];
        public VerifierArgumentType Stdin { get; set; } = VerifierArgumentType.None;

        private StringOrFile GetVerifierArgument(OutputVerificationInfo info, VerifierArgumentType thing)
        {
            switch(thing)
            {
                case VerifierArgumentType.ExitCode:
                    return StringOrFile.FromText(info.ExitCode.ToString());
                case VerifierArgumentType.FileSolution:
                    return StringOrFile.FromText(info.SolFile.FilePath);
                case VerifierArgumentType.FileStderr:
                    return StringOrFile.FromText(info.StandardError.FilePath);
                case VerifierArgumentType.FileStdin:
                    return StringOrFile.FromText(info.StandardInput.FilePath);
                case VerifierArgumentType.FileStdout:
                    return StringOrFile.FromText(info.StandardOutput.FilePath);
                case VerifierArgumentType.Solution:
                    return info.SolFile;
                case VerifierArgumentType.Stderr:
                    return info.StandardError;
                case VerifierArgumentType.Stdin:
                    return info.StandardInput;
                case VerifierArgumentType.Stdout:
                    return info.StandardOutput;
                default:
                case VerifierArgumentType.None:
                    return StringOrFile.FromText(string.Empty);
            }
        }

        public async Task<OutputVerificationResult> VerifyAsync(OutputVerificationInfo info)
        {
            StringBuilder argBuilder = new StringBuilder();
            foreach(VerifierArgumentType arg in Arguments)
            {
                argBuilder.Append($" \"{GetVerifierArgument(info, arg)}\"");
            }
            ApplicationRunner runner = new ApplicationRunner();

            ProcessRunResult checkerRun = await runner.RunAsync(
                ExecutablePath,
                TimeSpan.FromSeconds(60),
                GetVerifierArgument(info, Stdin),
                argBuilder.ToString().TrimEnd()
            );
            ;
            if (checkerRun.ExitType == ProcessExitType.Crashed)
            {
                return new OutputVerificationResult (
                    OutputVerificationResultType.CheckerCrashed,
                    checkerRun.CrashReport,
                    0
                );
            }

            OutputVerificationResult result = null;

            foreach(IVerifierResultBinder binding in Bindings)
            {
                if (binding.TryBind(checkerRun, out result)) break;
            }

            if (result == null)
            {
                result = new OutputVerificationResult (
                    OutputVerificationResultType.CouldNotBind,
                    null,
                    0
                );
            }

            return result;
        }

        public OutputVerificationResult Verify(OutputVerificationInfo info) => VerifyAsync(info).GetAwaiter().GetResult();
    }
}
