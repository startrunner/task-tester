using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResiltBindings;
using TaskTester.CheckerCore.ProcessRunning;
using TaskTester.Noi2Evaluator.Infos;

namespace TaskTester.Noi2Evaluator
{
    class CompetitorResult
    {
        public TimeSpan EvaluationDuration { get; set; }
        public List<double> ProblemResults { get; private set; } = new List<double>();
        public double TotalResult => ProblemResults.Sum();
        public string Name { get; set; }
        public string Directory { get; set; }
    }
}
