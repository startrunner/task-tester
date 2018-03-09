using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using TaskTester.BatchEvaluation;
using TaskTester.DataExtraction;
using TaskTester.Spreadsheets;

namespace TaskTester.DesktopTester.ViewModel
{
    public class BatchTestProgressViewModel : ViewModelBase
    {
        readonly PathSetViewModel mRootDirectory;
        readonly PrimitiveViewModel<string> mDirectoryPathCriteria;
        readonly ObservableCollection<PrimitiveViewModel<string>> mCommandLines;
        readonly ObservableCollection<BatchTestProblemViewModel> mProblems;
        readonly PrimitiveViewModel<string> mTitle;

        BatchEvaluationTask mEvaluationTask;

        CancellationTokenSource mCancellationTokenSource;

        [JsonIgnore]
        bool mIsRunning = false;

        [JsonIgnore]
        public bool IsRunning
        {
            get => mIsRunning;
            private set
            {
                mIsRunning = value;
                RaisePropertyChanged(nameof(IsRunning));
                RaisePropertyChanged(nameof(Start));
                RaisePropertyChanged(nameof(StartCanExecute));
                RaisePropertyChanged(nameof(Cancel));
                RaisePropertyChanged(nameof(CancelCanExecute));
            }
        }


        public ObservableCollection<BatchTestCompetitorResultViewModel> CompetitorResults { get; } = new ObservableCollection<BatchTestCompetitorResultViewModel>();

        [JsonIgnore]
        public string SplitterColumnHeader { get; } = nameof(SplitterColumnHeader);

        [JsonIgnore]
        public ICommand Start { get; }
        public ICommand Cancel { get; }
        [JsonIgnore]
        public ICommand Export { get; }
        public event EventHandler Starting;

        public BatchTestProgressViewModel(
            PathSetViewModel rootDirectory,
            PrimitiveViewModel<string> directoryPathCriteria,
            PrimitiveViewModel<string> title,
            ObservableCollection<PrimitiveViewModel<string>> commandLines,
            ObservableCollection<BatchTestProblemViewModel> problems
        )
        {
            Start = new RelayCommand(StartExecute, () => StartCanExecute);
            Export = new RelayCommand(ExportExecute, ExportCanExecute);
            Cancel = new RelayCommand(CancelExecute, () => CancelCanExecute);

            mRootDirectory = rootDirectory;
            mDirectoryPathCriteria = directoryPathCriteria;
            mCommandLines = commandLines;
            mProblems = problems;
            mTitle = title;

            rootDirectory.PropertyChanged += OnChildPropertyChanged;
            directoryPathCriteria.PropertyChanged += OnChildPropertyChanged;
        }

        private void CancelExecute() =>
            mCancellationTokenSource?.Cancel();

        private void ExportExecute()
        {
            SaveFileDialog dialog = new SaveFileDialog() {
                Filter = "Excel | *.xlsx",
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {

                if (File.Exists(dialog.FileName))
                {
                    try { File.Delete(dialog.FileName); }
                    catch { return; }
                }

                ExportExecute(dialog.FileName);
            }
        }

        private void ExportExecute(string fileName)
        {
            if (fileName == null) ExportExecute();

            Ranking<BatchTestCompetitorResultViewModel> ranking =
                CompetitorResults.RankBy(x => x.TotalResultRounded);

            var exporter = new SpreadsheetExporter<Tuple<Rank, BatchTestCompetitorResultViewModel>>(mTitle.Value ?? "") {
                { "Rank", x=>x.Item1.ToString() },
                { "Name", x=>x.Item2.Name.ToString() },
                { "Directory", x=>x.Item2.DirectoryRelative },
                { "Total", x=>x.Item2.TotalResultRounded.ToString() }
            };

            foreach (BatchTestProblemViewModel problem in mProblems)
            {
                exporter.Add(problem.Identifier, x => x.Item2.ProblemResults[problem.Identifier].Score.ToString("0"));
            }

            foreach (BatchTestProblemViewModel problem in mProblems)
            {
                exporter.Add(problem.Identifier, x => TranslateTestResultTypesToText(x.Item2.ProblemResults[problem.Identifier].TestResults));
            }

            exporter.Export(fileName, ranking);
        }

        private string TranslateTestResultTypesToText(IEnumerable<BatchTestTestResultViewModel> results)
        {
            var builder = new StringBuilder();
            foreach (BatchTestTestResultViewModel x in results)
            {
                builder.Append(TranslateTestResultTypeToText(x));
            }
            return builder.ToString();
        }

        private string TranslateTestResultTypeToText(BatchTestTestResultViewModel result)
        {
            switch (result.Type)
            {
                case BatchTestTestResultTypeViewModel.CorrectAnswer:
                    return "C";
                case BatchTestTestResultTypeViewModel.WrongAnswer:
                    return "W";
                case BatchTestTestResultTypeViewModel.TimeLimitExceeded:
                    return "T";
                case BatchTestTestResultTypeViewModel.PartiallyCorrectAnswer:
                    return $"P";
                case BatchTestTestResultTypeViewModel.ProgramCrashed:
                    return $"E";
                default:
                    return "?";

            }
        }
        private bool ExportCanExecute() => IsRunning == false;

        private void OnChildPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Start));
            RaisePropertyChanged(nameof(ExportExecute));
        }

        private void StartExecute()
        {
            if (!StartCanExecute) return;

            var problems = new List<BatchEvaluationProblem>();
            foreach(BatchTestProblemViewModel problem in mProblems)
            {
                if(!problem.TryBuildModel(out BatchEvaluationProblem problemModel))
                {
                    MessageBox.Show("Could not evaluate. Check selected files.");
                    return;
                }
                problems.Add(problemModel);
            }

            Starting?.Invoke(this, EventArgs.Empty);

            IsRunning = true;
            mCancellationTokenSource = new CancellationTokenSource();

            CompetitorResults.Clear();
            mEvaluationTask = new BatchEvaluationTask(
                Dispatcher.CurrentDispatcher,
                rootDirectory: mRootDirectory.Path,
                cancellationToken: mCancellationTokenSource.Token,
                directoryPathCriteria: mDirectoryPathCriteria.Value,
                commandLineTemplates: mCommandLines.Select(x => x.Value).ToArray(),
                problems: problems
            );

            mEvaluationTask.TestEvaluated += HandleTestEvaluated;
            mEvaluationTask.CommandLineExecuted += HandleCommandLineExecuted;
            mEvaluationTask.CompetitorDiscovered += this.HandleCompetitorDiscovered;
            mEvaluationTask.Finished += this.HandleEvaluationTaskFinished;
            mEvaluationTask.SolutionGraded += HandleSolutionGraded;

            mEvaluationTask.Start();
        }

        private void HandleSolutionGraded(object sender, BatchEvaluationSolutionGraderTask.SolutionGradedEventArgs e)
        {
            CompetitorResults[e.Competitor.Index].ProblemResults[e.Problem.Identifier].Score = e.Score;
            CompetitorResults[e.Competitor.Index].TotalResult += e.Score;
        }

        private void HandleTestEvaluated(object sender, CompetitorEvaluationTask.TestEvaluatedEventArgs e)
        {
            BatchTestProblemResultViewModel problemResult =
                CompetitorResults[e.Competitor.Index]
                .ProblemResults[e.Problem.Identifier];

            problemResult.TestResults.Add(new BatchTestTestResultViewModel(e));
        }

        private void HandleCommandLineExecuted(object sender, CommandLineRunningTask.CommandRanEventArgs e)
        {
            CompetitorResults[e.Competitor.Index].ExecutedCommandCount++;
        }

        private void HandleEvaluationTaskFinished(object sender, EventArgs e)
            => this.IsRunning = false;

        private void HandleCompetitorDiscovered(object sender, CompetitorInfoExtractedEventArgs e)
        {
            while (CompetitorResults.Count <= e.Index) CompetitorResults.Add(new BatchTestCompetitorResultViewModel());
            BatchTestCompetitorResultViewModel competitor = CompetitorResults[e.Index];

            competitor.Name = e.Name;
            competitor.Directory = e.Directory;
            competitor.DirectoryRelative = e.DirectoryRelative;
            competitor.TotalCommandCount = mProblems.Count * mCommandLines.Count;

            foreach (BatchTestProblemViewModel problem in mProblems)
            {
                competitor.ProblemResults.Add(problem.Identifier, new BatchTestProblemResultViewModel());
            }
        }

        public bool StartCanExecute =>
            IsRunning == false &&
            mRootDirectory.Path != null &&
            Directory.Exists(mRootDirectory.Path);

        public bool CancelCanExecute => IsRunning;
    }
}
