using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TestLib;

namespace TaskTester.Model
{
    partial class Problem
    {
        public delegate void OneTestCompletedEventHandler(Problem sender, ExecutionResult result);
        public event OneTestCompletedEventHandler OneTestCompleted;

        public delegate void AllTestsCompletedEventHandler(Problem sender, List<ExecutionResult> results);
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

        public static async Task<ExecutionResult> ExecuteTestAsync(FileInfo executable, FileInfo inputFile, FileInfo solutionFile, TimeSpan timeLimit)
        {
            await Task.Yield();
            var input = inputFile.OpenText().ReadToEnd();
            var sol = solutionFile.OpenText().ReadToEnd();
            return new ExecutionResult() {
                ExecutionTime = TimeSpan.FromSeconds(1),
                ExpectedAnswer = "1",
                SolutionAnswer = "1",
                IdentifierIndex = 1,
                Type = TestResultType.CorrectAnswer
            };
            //return await BinaryExecutor.ExecuteTestAsync(executable, input, sol, timeLimit);
        }

        public async Task<IEnumerable<ExecutionResult>> RunTestsAsync()
        {
            List<ExecutionResult> results = new List<ExecutionResult>();
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
                         result.IdentifierIndex = i;
                     });
                }
            });

            IsInProgress = false;
            AllTestsCompleted?.Invoke(this, results);

            return null;

        }
    }
}
