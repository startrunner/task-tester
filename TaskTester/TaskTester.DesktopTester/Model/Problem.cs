using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResiltBindings;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.DesktopTester.Model
{
    partial class Problem
    {
        public delegate void OneTestCompletedEventHandler(Problem sender, IExecutionResult result);
        public event OneTestCompletedEventHandler OneTestCompleted;

        public delegate void AllTestsCompletedEventHandler(Problem sender, List<IExecutionResult> results);
        public event AllTestsCompletedEventHandler AllTestsCompleted;

        public bool IsInProgress { get; private set; } = false;
        public FileInfo ExecutableFile { get; set; }
        public List<FileInfo> InputFiles { get; set; }
        public List<FileInfo> SolutionFiles { get; set; }
        public TimeSpan? TimeLimit { get; set; } = TimeSpan.FromSeconds(1);
        public Checker Checker { get; set; } = new Checker();

        public bool IsValidForExecution
        {
            get
            {
                if (IsInProgress) return false;
                else if (ExecutableFile == null) return false;
                else if (InputFiles == null || InputFiles.Count == 0) return false;
                else if (SolutionFiles == null || SolutionFiles.Count == 0) return false;
                else if (InputFiles.Count != SolutionFiles.Count) return false;
                else if (TimeLimit == null) return false;
                else return true;
            }
        }

        public async Task<IExecutionResult> ExecuteTestAsync(FileInfo executable, FileInfo inputFile, FileInfo solutionFile, TimeSpan timeLimit)
        {
            await Task.Yield();

            ApplicationRunner runner = new ApplicationRunner(executable.FullName) {
                MaxRuntime = timeLimit,
                StdIn = StringOrFile.FromFile(inputFile.FullName),
            };
            IProcessRunResult runResult = await runner.RunAsync();

            ExecutionResultMutable rt = new ExecutionResultMutable() {
                ExecutionTime = runResult.ExecutionTime,
                ExpectedAnswer = File.ReadAllText(solutionFile.FullName),
                IdentifierIndex = 0,
                SolutionAnswer = runResult.StdOut.Str,
                CrashReport = runResult.CrashReport,
            };

            if (runResult.ExitType== ProcessExitType.Crashed)
            {
                rt.Type = TestResultType.ProgramCrashed;
            }
            else if (runResult.ExitType == ProcessExitType.Forced)
            {
                rt.Type = TestResultType.Timeout;
            }
            else if (runResult.ExitType== ProcessExitType.Graceful)
            {
                IOutputVerifier verifier = GetVerifier();

                var result = verifier.Verify(new OutputVerificationInfoMutable() {
                    ExitCode = runResult.ExitCode,
                    SolFile = StringOrFile.FromFile(solutionFile.FullName),
                    StandardError = runResult.StdOut,
                    StandardInput = StringOrFile.FromFile(inputFile.FullName),
                    StandardOutput = runResult.StdOut,
                });

                switch(result.Type)
                {
                    case OutputVerificationResultType.CheckerCrashed:
                        rt.Score = 0;
                        rt.Type = TestResultType.CheckerCrashed;
                        rt.CrashReport = result.CrashReport;
                        break;
                    case OutputVerificationResultType.CorrectAnswer:
                        rt.Type = TestResultType.CorrectAnswer;
                        rt.Score = result.Score;
                        break;
                    case OutputVerificationResultType.WrongAnswer:
                        rt.Type = TestResultType.WrongAnswer;
                        break;
                    case OutputVerificationResultType.PartiallyCorrectAnswer:
                        rt.Type = TestResultType.PartiallyCorrectAnswer;
                        rt.Score = result.Score;
                        break;
                    case OutputVerificationResultType.CouldNotBind:
                        rt.Type = TestResultType.CouldNotBind;
                        break;
                }
            }
            else if (runResult.ExitType == ProcessExitType.Undetermined) { throw new InvalidOperationException(); }

            return rt;

            throw new NotImplementedException();
            //return await BinaryExecutor.ExecuteTestAsync(executable, input, sol, timeLimit);
        }

        private IOutputVerifier GetVerifier()
        {
            if(!string.IsNullOrEmpty(Checker.ExecutablePath) && File.Exists(Checker.ExecutablePath))
            {
                IOutputVerifier checker = new ExecutableOutputVerifierMutable {
                    ExecutablePath = Checker.ExecutablePath,
                    Arguments = Checker.Args.Select((arg) =>
                    {
                        switch (arg.Type)
                        {
                            case ArgType.SolFile:
                                return VerifierArgumentType.FileSolution;
                            case ArgType.StdinFile:
                                return VerifierArgumentType.FileStdin;
                            case ArgType.StdOutFile:
                                return VerifierArgumentType.FileStdout;
                            default:
                            case ArgType.None:
                                return VerifierArgumentType.None;
                        }
                    }).ToArray(),
                    Bindings = Checker.Bindings.Select(x =>
                    new StdOutContainsBinding(x.SearchString, new OutputVerificationResultMutable {
                        Score = x.Score,
                        Type = x.Type
                    })).ToArray(),
                    Stdin = VerifierArgumentType.None
                };
                return checker;
            }

            return new DefaultOutputVerifier();
        }

        public async Task<IEnumerable<IExecutionResult>> RunTestsAsync()
        {
            List<IExecutionResult> results = new List<IExecutionResult>();
            Dispatcher uid = Dispatcher.CurrentDispatcher;

            if (!IsValidForExecution) throw new InvalidOperationException("You cannot run tests on an invalid problem.");
            IsInProgress = true;

            await Task.Run(async () =>
            {

                var ins = InputFiles.OrderBy(f => f.Name).ToArray();
                var sols = SolutionFiles.OrderBy(x => x.Name).ToList();

                for (int i = 0; i < InputFiles.Count; i++)
                {
                    var result = await ExecuteTestAsync(ExecutableFile, ins[i], sols[i], TimeLimit.GetValueOrDefault());
                    uid.Invoke(() =>
                    {
                         OneTestCompleted?.Invoke(this, result);
                         results.Add(result);
                     });
                }
            });

            IsInProgress = false;
            AllTestsCompleted?.Invoke(this, results);

            return null;

        }
    }
}
