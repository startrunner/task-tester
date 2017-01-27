using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using TaskTester.Model;
using System.IO;
using System.Collections.ObjectModel;
using TestLib;

namespace TaskTester.ViewModel
{
   class ProblemViewModel : ViewModelBase
   {
      public Problem Model { get; private set; }

      public int CompletedTestsCount
      {
         get
         {
            if (Model == null) return 3;
            return Feedback.Count;
         }
      }
      public int AllTestsCount
      {
         get
         {
            if (Model == null || Model.InputFiles == null) return 10;
            return Model.InputFiles.Count;
         }
      }

      public string ExecutablePath { get; private set; }
      public string InputPaths { get; private set; }
      public string SolutionPaths { get; private set; }

      private string _timeLimit;
      public string TimeLimit
      {
         get { return _timeLimit; }
         set
         {
            _timeLimit = value;

            double timeLimit;
            if (double.TryParse(value, out timeLimit))
            {
               Model.TimeLimit = TimeSpan.FromSeconds(timeLimit);
            }
            else
            {
               Model.TimeLimit = null;
            }

            RaisePropertyChanged("RunTests");
         }
      }

      public ICommand BrowseExecutable { get; private set; }
      public ICommand BrowseInputs { get; private set; }
      public ICommand BrowseSolutions { get; private set; }
      public ICommand RunTests { get; private set; }

      public ProblemViewModel()
      {
         BrowseExecutable = new RelayCommand(BrowseExecutableExecute);
         BrowseInputs = new RelayCommand(BrowseInputsExecute);
         BrowseSolutions = new RelayCommand(BrowseSolutionsExecute);
         RunTests = new RelayCommand(RunTestsExecute, RunTestsCanExecute);
      }

      private bool RunTestsCanExecute()
      {
         if (IsInDesignMode) return true;
         return Model.IsValidForExecution;
      }

      private void RunTestsExecute()
      {
         Feedback.Clear();
         RaisePropertyChanged("CompletedTestsCount");
         RaisePropertyChanged("AllTestsCount");

         RaisePropertyChanged("Feedback");

         Model.RunTestsAsync().GetAwaiter();
      }

      private void BrowseSolutionsExecute()
      {
         OpenFileDialog ofd = new OpenFileDialog();
         ofd.AddExtension = false;
         ofd.Multiselect = true;
         ofd.Filter = "Test Solution File |*.sol; *.out |Any File|*.*";

         ofd.ShowDialog();

         if (ofd.FileNames.Length != 0 && ofd.CheckFileExists)
         {
            List<FileInfo> files = ofd.FileNames.Select(x => new FileInfo(x)).ToList();
            Model.SolutionFiles = files;

            SolutionPaths = string.Join(";", ofd.FileNames);
            RaisePropertyChanged("SolutionPaths");
            RaisePropertyChanged("RunTests");
         }
      }

      private void BrowseInputsExecute()
      {
         OpenFileDialog ofd = new OpenFileDialog();
         ofd.AddExtension = false;
         ofd.Multiselect = true;
         ofd.Filter = "Test Input File |*.in; *.inp |Any File|*.*";

         ofd.ShowDialog();

         if (ofd.FileNames.Length != 0 && ofd.CheckFileExists)
         {
            List<FileInfo> files = ofd.FileNames.Select(x => new FileInfo(x)).ToList();
            Model.InputFiles = files;

            InputPaths = string.Join(";", ofd.FileNames);
            RaisePropertyChanged("InputPaths");
            RaisePropertyChanged("RunTests");
         }
      }

      private void BrowseExecutableExecute()
      {
         OpenFileDialog ofd = new OpenFileDialog();
         ofd.AddExtension = false;
         ofd.Multiselect = false;
         ofd.Filter = "Console Application |*.exe";

         ofd.ShowDialog();

         if (ofd.FileName != "" && ofd.CheckFileExists)
         {
            Model.ExecutableFile = new FileInfo(ofd.FileName);
            ExecutablePath = ofd.FileName;
            RaisePropertyChanged("ExecutablePath");
            RaisePropertyChanged("RunTests");
         }

      }



      public ProblemViewModel(Problem problem) : this()
      {
         this.Model = problem;
         this._timeLimit = Model.TimeLimit.GetValueOrDefault().TotalSeconds.ToString();
         problem.OneTestCompleted += OnOneTestCompleted;
         problem.AllTestsCompleted += OnAllTestsCompleted;
      }

      private void OnAllTestsCompleted(Problem sender, List<ExecutionResult> results)
      {
         RaisePropertyChanged("RunTests");
      }

      public ObservableCollection<TestResultViewModel> Feedback { get; set; } = new ObservableCollection<TestResultViewModel>();

      private void OnOneTestCompleted(Problem sender, ExecutionResult result)
      {
         Feedback.Add(new TestResultViewModel(result));
         //RaisePropertyChanged("Feedback");
         RaisePropertyChanged("CompletedTestsCount");
      }
   }
}
