using System;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.CommandWpf;
using TaskTester.Model;

namespace TaskTester.ViewModel
{
   class TestResultViewModel
   {
      public ExecutionResult Model { get; private set; }
      public Brush BackgroundColor
      {
         get
         {
            if (Model.Type == TestResultType.CorrectAnswer) return new SolidColorBrush(Colors.Green);
            else if (Model.Type == TestResultType.ProgramCrashed) return new SolidColorBrush(Colors.Orange);
            else if (Model.Type == TestResultType.WriteTimeout) return new SolidColorBrush(Colors.Crimson);
            else return new SolidColorBrush(Colors.Red);
         }
      }
      public string Message
      {
         get
         {
            switch (Model.Type)
            {
               case TestResultType.CorrectAnswer: return "Correct Answer";
               case TestResultType.ProgramCrashed: return "Program Crashed";
               case TestResultType.Timeout: return "Time Limit Exceeded";
               case TestResultType.WrongAnswer: return "Wrong Answer";
               case TestResultType.Skipped: return "Skipped";
               case TestResultType.WriteTimeout: return "Write Timeout";
            }
            return "Nope";
         }
      }
      public string ExpectedAnswer
      {
         get { return Model.ExpectedAnswer; }
      }
      public string SolutionAnswer
      {
         get { return Model.SolutionAnswer; }
      }
      public string ExecutionTime
      {
         get
         {
            if (Model.ExecutionTime != null) return Math.Round(Model.ExecutionTime.Value.TotalSeconds, 3).ToString() + "s";
            else return "Not Measured.";
         }
      }
      public int ExecutionNumber => Model.IdentifierIndex;

      public ICommand ViewDetail { get; private set; }
      private void ViewDetailExecute() { new View.TestResultView { DataContext = this }.Show(); }

      public TestResultViewModel(ExecutionResult model)
      {
         Model = model;
         Construct();
      }
      public TestResultViewModel()
      {
         Model = new ExecutionResult();
         Construct();
      }

      private void Construct()
      {
         ViewDetail = new RelayCommand(ViewDetailExecute);
      }
   }
}
