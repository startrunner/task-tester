using System;
using Newtonsoft.Json;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResultBinders;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class CheckerBindingViewModel
    {
        
        public string SearchText { get; set; }
        
        public double ScoreMultiplier { get; set; }
        
        public EnumViewModel<TestResultTypeViewModel> ResultType { get; set; } = TestResultTypeViewModel.CouldNotBind;

        internal IVerifierResultBinder CreateModel()
        {
            return new StdOutContainsBinder(
                searchText: SearchText,
                result: new OutputVerificationResult(
                    type: TranslateType(ResultType.SelectedValue),
                    scoreMultiplier: ScoreMultiplier
                )
            );
        }

        private OutputVerificationResultType TranslateType(TestResultTypeViewModel selectedValue)
        {
            switch (selectedValue)
            {
                case TestResultTypeViewModel.CorrectAnswer:
                    return OutputVerificationResultType.CorrectAnswer;
                case TestResultTypeViewModel.WrongAnswer:
                    return OutputVerificationResultType.WrongAnswer;
                case TestResultTypeViewModel.PartiallyCorrectAnswer:
                    return OutputVerificationResultType.PartiallyCorrectAnswer;
                case TestResultTypeViewModel.CouldNotBind:
                    return OutputVerificationResultType.CouldNotBind;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
