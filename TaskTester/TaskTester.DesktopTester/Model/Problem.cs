using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
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

        public static async Task<IExecutionResult> ExecuteTestAsync(FileInfo executable, FileInfo inputFile, FileInfo solutionFile, TimeSpan timeLimit)
        {
            await Task.Yield();

            ApplicationRunner runner = new ApplicationRunner(executable.FullName) {
                MaxRuntime = timeLimit,
                StdIn = StringOrFile.FromFile(inputFile.FullName),
            };
            IProcessRunResult runResult = await runner.RunAsync();

            if (runResult.ExitType== ProcessExitType.Crashed)
            {
                return new ExecutionResultMutable() {
                    ExecutionTime = runResult.ExecutionTime,
                    ExpectedAnswer = File.ReadAllText(solutionFile.FullName),
                    IdentifierIndex = 0,
                    SolutionAnswer = runResult.StdOut.Str + '\n' + runResult.CrashReport.ExceptionMessage,
                    Type = TestResultType.ProgramCrashed
                };
            }
            else if (runResult.ExitType == ProcessExitType.Forced)
            {
                return new ExecutionResultMutable() {
                    ExecutionTime = runResult.ExecutionTime,
                    ExpectedAnswer = File.ReadAllText(solutionFile.FullName),
                    IdentifierIndex = 0,
                    SolutionAnswer = runResult.StdOut.Str,
                    Type = TestResultType.Timeout
                };
            }
            else if (runResult.ExitType== ProcessExitType.Graceful)
            {
                IOutputVerifier verifier = new DefaultOutputVerifier();

                var result = verifier.Verify(new ProcessVerificationInfoMutable() {
                    ExitCode = runResult.ExitCode,
                    SolFile = StringOrFile.FromFile(solutionFile.FullName),
                    StandardError = runResult.StdOut,
                    StandardInput = StringOrFile.FromFile(inputFile.FullName),
                    StandardOutput = runResult.StdOut,
                });

                if (result.Type == OutputVerificationType.CorrectAnswer)
                {
                    return new ExecutionResultMutable() {
                        ExecutionTime = runResult.ExecutionTime,
                        ExpectedAnswer = File.ReadAllText(solutionFile.FullName),
                        SolutionAnswer = runResult.StdOut.Str,
                        IdentifierIndex = 0,
                        Type = TestResultType.CorrectAnswer
                    };
                }
                else
                {
                    return new ExecutionResultMutable() {
                        ExecutionTime = runResult.ExecutionTime,
                        ExpectedAnswer = File.ReadAllText(solutionFile.FullName),
                        SolutionAnswer = runResult.StdOut.Str,
                        IdentifierIndex = 0,
                        Type = TestResultType.WrongAnswer
                    };
                }
            }
            else if (runResult.ExitType == ProcessExitType.Undetermined) { throw new InvalidOperationException(); }

            throw new NotImplementedException();
            //return await BinaryExecutor.ExecuteTestAsync(executable, input, sol, timeLimit);
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
