﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using TaskTester.CheckerCore.ProcessRunning;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public class SolutionEvaluationTaskViewModel : ViewModelBase
    {

        private readonly PathSetViewModel mExecutable;

        private readonly ProblemViewModel mProblem;

        private int mTotalTestCount;

        private bool mCheckerIdle = true;
        CancellationTokenSource mCancellationTokenSource;

        [JsonIgnore]
        public ICommand EvaluateSolution { get; }
        [JsonIgnore]
        public ICommand CancelEvaluation { get; }

        [JsonIgnore]
        public bool CheckerIdle
        {
            get => mCheckerIdle;
            private set
            {
                mCheckerIdle = value;
                RaisePropertyChanged(nameof(CheckerIdle));
                RaisePropertyChanged(nameof(EvaluateSolution));
                RaisePropertyChanged(nameof(EvaluateSolutionCanExecute));
                RaisePropertyChanged(nameof(CancelEvaluation));
                RaisePropertyChanged(nameof(CancelEvaluationCanExecute));
            }
        }

        public int TotalTestCount
        {
            get => mTotalTestCount;
            set
            {
                mTotalTestCount = value;
                RaisePropertyChanged(nameof(TotalTestCount));
            }

        }


        public ObservableCollection<TestResultViewModel> TestResults { get; }
            = new ObservableCollection<TestResultViewModel>();

        public SolutionEvaluationTaskViewModel(PathSetViewModel mExecutable, ProblemViewModel mProblem)
        {
            EvaluateSolution = new RelayCommand(EvaluateSolutionExecute, () => EvaluateSolutionCanExecute);
            CancelEvaluation = new RelayCommand(CancelEvaluationExecute, () => CancelEvaluationCanExecute);

            if (mExecutable == null)
                throw new ArgumentNullException(nameof(mExecutable));
            if (mProblem == null)
                throw new ArgumentNullException(nameof(mProblem));

            mExecutable.PropertyChanged += OnPathChanged;
            mProblem.TestInputFiles.PropertyChanged += OnPathChanged;
            mProblem.TestSolutionFiles.PropertyChanged += OnPathChanged;

            this.mExecutable = mExecutable;
            this.mProblem = mProblem;
        }

        private void OnPathChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(EvaluateSolution));
        }

        private async void EvaluateSolutionExecute()
        {
            if (!CheckerIdle) return;

            CheckerIdle = false;
            await Task.Yield();

            IConsoleApplication application = new FileSystemConsoleApplication(mExecutable.Path, CheckerCore.CrashReporting.CrashReportFinder.Instance);

            if (!mProblem.TryCreateModel(out Problem problem))
            {
                MessageBox.Show("Could not run evaluate. Check selected files.");
                return;
            }

            IReadOnlyList<SolutionTest> tests = problem.Tests;

            this.TestResults.Clear();
            this.TotalTestCount = tests.Count;
            this.mCancellationTokenSource = new CancellationTokenSource();

            SolutionEvaluationTask task = new SolutionEvaluationTask(
                (a, e) => Dispatcher.CurrentDispatcher.Invoke(a, e),
                mCancellationTokenSource.Token,
                application,
                tests
            );

            task.TestEvaluated += Task_TestEvaluated;
            task.Finished += (sender, e) =>
            {
                Debug.WriteLine("Test Evaluated.");
                task.TestEvaluated -= Task_TestEvaluated;
                CheckerIdle = true;
            };

            task.Start();
        }

        private void Task_TestEvaluated(object sender, SolutionEvaluationTestResult e)
        {
            this.TestResults.Add(new TestResultViewModel
            {
                Type = TranslateTestResultType(e.Type),
                CrashMessage = e.RunResult?.CrashReport?.ErrorMessage,
                Score = e.Score,
                //ScoreMultiplier = e.Score,
                ExpectedOutput = e.ExpectedOutput,
                SolutionOutput = e.RunResult.Output,
                ExecutionTime = e.RunResult.ExecutionTime
            });
        }

        TestResultTypeViewModel TranslateTestResultType(SolutionEvaluationTestResultType type)
        {
            switch (type)
            {
                case SolutionEvaluationTestResultType.CheckerCouldNotBind:
                    return TestResultTypeViewModel.CouldNotBind;
                case SolutionEvaluationTestResultType.CheckerCrashed:
                    return TestResultTypeViewModel.CheckerCrashed;
                case SolutionEvaluationTestResultType.CorrectAnswer:
                    return TestResultTypeViewModel.CorrectAnswer;
                case SolutionEvaluationTestResultType.MemoryLimitExceeded:
                    return TestResultTypeViewModel.MemoryLimitExceeded;
                case SolutionEvaluationTestResultType.PartiallyCorrectAnswer:
                    return TestResultTypeViewModel.PartiallyCorrectAnswer;
                case SolutionEvaluationTestResultType.ProgramCrashed:
                    return TestResultTypeViewModel.ProgramCrashed;
                case SolutionEvaluationTestResultType.TimeLimitExceeded:
                    return TestResultTypeViewModel.TimeLimitExceeded;
                case SolutionEvaluationTestResultType.WrongAnswer:
                    return TestResultTypeViewModel.WrongAnswer;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CancelEvaluationExecute() =>
            mCancellationTokenSource.Cancel();

        public bool CancelEvaluationCanExecute => !CheckerIdle;
        public bool EvaluateSolutionCanExecute =>
            CheckerIdle &&
            mExecutable.Path != null &&
            File.Exists(mExecutable.Path) &&
            mProblem.CanCreateTests;
    }
}
