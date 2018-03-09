using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using TaskTester.BatchEvaluation;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class BatchTestProblemViewModel : ViewModelBase
    {
        public string Identifier { get; set; }

        public ProblemViewModel Problem { get; } = new ProblemViewModel();

        public event EventHandler RemoveRequested;

        [JsonIgnore]
        public ICommand Remove { get; }

        public BatchTestProblemViewModel()
        {
            Remove = new RelayCommand(() => this.RemoveRequested?.Invoke(this, EventArgs.Empty));
        }

        public bool TryBuildModel(out BatchEvaluationProblem problem)
        {
            if(Problem.TryCreateModel(out Problem prob))
            {
                problem = new BatchEvaluationProblem(Identifier, prob);
                return true;
            }
            else
            {
                problem = null ;
                return false;
            }
        }
    }
}
