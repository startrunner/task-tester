using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.Noi2Evaluator.Infos;

namespace TaskTester.Noi2Evaluator
{
    class CompetitionEvaluator
    {
        CompetitionInfo competitionInfo;
        private List<CompetitorInfo> competitors;

        public CompetitionEvaluator(CompetitionInfo competitionInfo, List<CompetitorInfo> competitors)
        {
            this.competitionInfo = competitionInfo;
            this.competitors = competitors;
        }

        public CompetitionResult Evaluate()
        {
            CompetitionResult rt = new CompetitionResult();
            DateTime evalStart = DateTime.Now;

            int totalCount = competitors.Count;
            int evaluatedCount = 0;
            double averageEvalTime = -1;

            foreach(var competitorInfo in competitors)
            {
                CompetitorEvaluator evaluator = new CompetitorEvaluator(competitionInfo, competitorInfo);
                var result = evaluator.Evaluate();
                rt.CompetitorResults.Add(result);

                if(evaluatedCount==0)
                {
                    averageEvalTime = result.EvaluationDuration.TotalSeconds;
                }
                else
                {
                    averageEvalTime = (averageEvalTime * evaluatedCount + result.EvaluationDuration.TotalSeconds) / (evaluatedCount + 1);
                }

                evaluatedCount++;

                int competitorsLeft = totalCount - evaluatedCount;
                Console.WriteLine($"Evaluated {evaluatedCount} of {totalCount} competitors; {competitorsLeft} remaining");
                Console.WriteLine($"Average eval time: {averageEvalTime}s.");
                Console.WriteLine($"Expected time left: {TimeSpan.FromSeconds(competitorsLeft * averageEvalTime)}");

                Console.WriteLine();
            }

            DateTime evalEnd = DateTime.Now;
            TimeSpan evalDuration = evalEnd - evalStart;

            rt.EvaluationDuration = evalDuration;
            return rt;
        }
    }
}
