using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore;
using TaskTester.Tasking;

namespace TaskTester.BatchEvaluation
{
    public sealed class CommandLineRunningTask : BackgroundTask
    {
        #region EventArgs
        public class StartedEventArgs : EventArgs
        {
            public int TotalCommandLines { get; internal set; }
        }
        public class CommandRanEventArgs : EventArgs
        {
            public BatchEvaluationCompetitor Competitor { get; internal set; }
            public string CommandLine { get; internal set; }
        }
        #endregion

        readonly IReadOnlyList<string> mCommandLineTemplates;
        readonly IReadOnlyList<BatchEvaluationProblem> mProblems;
        readonly BatchEvaluationCompetitor mCompetitor;

        public CommandLineRunningTask(
            Dispatcher eventDispatcher,
            IReadOnlyList<string> commandLineTemplates,
            IReadOnlyList<BatchEvaluationProblem> problems,
            BatchEvaluationCompetitor competitor
        ) : base(eventDispatcher)
        {
            mCommandLineTemplates = commandLineTemplates;
            mProblems = problems;
            mCompetitor = competitor;
        }

        public event EventHandler<StartedEventArgs> Started;
        public event EventHandler<CommandRanEventArgs> CommandRan;

        public override void Start()
        {
            MarkAsStarted();
            this.ExecutingTask =
                Task.Run(action: Run).ContinueWith(x => NotifyFinished());
        }

        private void Run()
        {
            NotifyStarted();
            List<string> commandLines = ExpandCommandLines();
            BatchEvaluationCompetitor competitor = mCompetitor;

            var terminalProcess = Process.Start(new ProcessStartInfo("cmd.exe") {
                WorkingDirectory = competitor.Directory,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            });

            foreach (string line in commandLines)
            {
                //terminalProcess.StandardInput.WriteLine(line);
                Process cmdProc = new Process() {
                    StartInfo = new ProcessStartInfo("cmd.exe") {
                        Arguments = $"/c {line}",
                        WorkingDirectory = competitor.Directory,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };
                cmdProc.OutputDataReceived += ((s, e) => { });
                cmdProc.ErrorDataReceived += ((s, e) => { });
                cmdProc.Start();
                cmdProc.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
                if (!cmdProc.HasExited) cmdProc.Kill();

                Notify(CommandRan, new CommandRanEventArgs {
                    CommandLine = line,
                    Competitor = competitor
                });
            }
            terminalProcess.StandardInput.Close();
            try
            {
                terminalProcess.Kill();
            }
            catch { }
        }

        private List<string> ExpandCommandLines()
        {
            var commandLines = new List<string>();

            foreach (BatchEvaluationProblem problem in mProblems)
            {
                foreach (string lineTemplate in mCommandLineTemplates)
                {
                    commandLines.Add(
                        string.Format(lineTemplate, problem.Identifier)
                    );
                }
            }

            return commandLines;
        }

        private void NotifyStarted()
        {
            var eventArgs = new StartedEventArgs {
                TotalCommandLines = mProblems.Count * mCommandLineTemplates.Count
            };

            Notify(Started, eventArgs);
        }
    }
}
