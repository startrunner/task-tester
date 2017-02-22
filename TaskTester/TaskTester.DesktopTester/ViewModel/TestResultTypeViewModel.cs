using GalaSoft.MvvmLight;
using TaskTester.DesktopTester.Model;

namespace TaskTester.DesktopTester.ViewModel
{
    class TestResultTypeViewModel:ViewModelBase
    {
        TestResultType model;

        public string Icon { get; private set; }

        public TestResultTypeViewModel():this(TestResultType.CorrectAnswer) { }

        public TestResultTypeViewModel(TestResultType model)
        {
            this.model = model;
            this.Icon = "StopTime";
            switch(model)
            {
                case TestResultType.CorrectAnswer:
                    this.Icon = "StatusOK";
                    break;
                case TestResultType.CheckerCrashed:
                case TestResultType.ProgramCrashed:
                    this.Icon = "StatusSecurityWarning";
                    break;
                case TestResultType.Timeout:
                    this.Icon = "StopTime";
                    break;
                case TestResultType.WrongAnswer:
                    this.Icon = "Cancel";
                    break;
                case TestResultType.CouldNotBind:
                    this.Icon = "Settings";
                    break;
                case TestResultType.PartiallyCorrectAnswer:
                    this.Icon = "HideMember";
                    break;
            }
        }
    }
}
