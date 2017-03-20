using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.Noi2Evaluator
{
    class CompetitionResult
    {
        public TimeSpan EvaluationDuration { get; set; }
        public List<CompetitorResult> CompetitorResults { get; set; } = new List<CompetitorResult>();
    }
}
