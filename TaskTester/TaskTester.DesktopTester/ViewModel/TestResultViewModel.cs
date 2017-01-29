using System;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.CommandWpf;
using TaskTester.DesktopTester.Model;

namespace TaskTester.DesktopTester.ViewModel
{
    class TestResultViewModel
    {
        public IExecutionResult Model { get; private set; }

        public string ExceptionMessage => Model?.CrashReport?.ExceptionMessage;
        public TestResultTypeViewModel Type => new TestResultTypeViewModel(Model.Type);

        public double Score
        {
            get
            {
                if (Model == null) return 10;
                return Model.Score;
            }
        }

        public Brush BackgroundColor
        {
            get
            {
                if (Model.Type == TestResultType.CorrectAnswer) return new SolidColorBrush(Colors.Green);
                else if (Model.Type == TestResultType.ProgramCrashed) return new SolidColorBrush(Colors.Orange);
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
                    case TestResultType.CouldNotBind: return "Couldn't bind checker";
                    case TestResultType.PartiallyCorrectAnswer: return "Partially correct";
                    case TestResultType.CheckerCrashed: return "Checker Crashed";
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
        private void ViewDetailExecute() { new View.TestResultView { DataContext = this }.ShowDialog(); }

        public TestResultViewModel(IExecutionResult model) 
        {
            Model = model;
            Construct();
        }
        public TestResultViewModel()
        {
            Model = new ExecutionResultMutable();
            Construct();
        }

        private void Construct()
        {
            ViewDetail = new RelayCommand(ViewDetailExecute);
        }
    }
}
