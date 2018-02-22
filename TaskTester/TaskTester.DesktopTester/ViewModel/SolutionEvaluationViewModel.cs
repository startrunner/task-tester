using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    class SolutionEvaluationViewModel : ViewModelBase
    {
        
        public PathSetViewModel Executable { get; } = new PathSetViewModel();
        
        public ProblemViewModel Problem { get; set; } = new ProblemViewModel();
        
        public SolutionEvaluationTaskViewModel EvaluationTask { get; }
        public SolutionEvaluationViewModel()
        {
            EvaluationTask = new SolutionEvaluationTaskViewModel(Executable, Problem);
        }
    }
}
