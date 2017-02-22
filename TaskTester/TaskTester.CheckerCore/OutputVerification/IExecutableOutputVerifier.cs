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
        IReadOnlyList<IVerifierResultBinding> Bindings { get; }
        VerifierArgumentType Stdin { get; }
    }

    public class ExecutableOutputVerifierMutable : IExecutableOutputVerifier
    {
        public string ExecutablePath { get; set; }
        public IReadOnlyList<VerifierArgumentType> Arguments { get; set; } = new VerifierArgumentType[0];
        public IReadOnlyList<IVerifierResultBinding> Bindings { get; set; } = new IVerifierResultBinding[0];
        public VerifierArgumentType Stdin { get; set; } = VerifierArgumentType.None;

        private StringOrFile GetThing(IOutputVerificationInfo info, VerifierArgumentType thing)
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

        public async Task<IOutputVerificationResult> VerifyAsync(IOutputVerificationInfo info)
        {
            StringBuilder argBuilder = new StringBuilder();
            foreach(var arg in Arguments)
            {
                argBuilder.Append($" \"{GetThing(info, arg)}\"");
            }
            ApplicationRunner runner = new ApplicationRunner(ExecutablePath) {
                MaxRuntime = TimeSpan.FromSeconds(60),
                StdIn = GetThing(info, Stdin),
                ProcessArguments = argBuilder.ToString().TrimEnd()
            };

            IProcessRunResult checkerRun = await runner.RunAsync();
            ;
            if (checkerRun.ExitType == ProcessExitType.Crashed)
            {
                return new OutputVerificationResultMutable {
                    Score = 0,
                    Type = OutputVerificationResultType.CheckerCrashed,
                    CrashReport = checkerRun.CrashReport
                };
            }

            IOutputVerificationResult result = null;

            foreach(var binding in Bindings)
            {
                if (binding.TryBind(checkerRun, out result)) break;
            }

            if (result == null)
            {
                result = new OutputVerificationResultMutable {
                    Score = 0,
                    Type = OutputVerificationResultType.CouldNotBind
                };
            }

            return result;
        }

        public IOutputVerificationResult Verify(IOutputVerificationInfo info) => VerifyAsync(info).GetAwaiter().GetResult();
    }
}
