using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.DesktopTester.ViewModel
{
    class SolutionEvaluationViewModel
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
