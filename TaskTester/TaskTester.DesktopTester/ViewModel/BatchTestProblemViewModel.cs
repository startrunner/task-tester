using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using TaskTester.BatchEvaluation;

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

        public BatchEvaluationProblem BuildModel()
        {
            return new BatchEvaluationProblem(Identifier, Problem.CreateModel());
        }
    }
}
