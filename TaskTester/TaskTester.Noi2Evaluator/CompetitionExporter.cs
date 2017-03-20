using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.Noi2Evaluator.Infos;

namespace TaskTester.Noi2Evaluator
{
    class CompetitionExporter
    {
        private CompetitionInfo competitionInfo;
        private List<CompetitorResult> results;

        public CompetitionExporter(CompetitionResult result, CompetitionInfo info)
        {
            this.results = result.CompetitorResults
                .OrderByDescending(x => x.TotalResult)
                .ToList();
            this.competitionInfo = info;
        }

        public void Export(string filename)
        {
            var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Results");
            ;

            sheet.Cell("A1").Value = "Rank";
            sheet.Cell("B1").Value = "Name";
            sheet.Cell("C1").Value = "Directory";
            int i = 0;
            foreach(var problem in competitionInfo.Problems)
            {
                sheet.Cell($"{(char)('D' + i)}1").Value = $"Problem: {problem.Name}";
                i++;
            }
            sheet.Cell($"{(char)('D' + i)}1").Value = "Total";


            i = 0;
            foreach(var competitor in results)
            {
                sheet.Cell($"A{i + 2}").Value = i + 1;
                sheet.Cell($"B{i + 2}").Value = competitor.Name;
                sheet.Cell($"C{i + 2}").Value = competitor.Directory;

                int j = 0;
                foreach(int result in competitor.ProblemResults)
                {
                    sheet.Cell($"{(char)('D' + j)}{i + 2}").Value = result;
                    j++;
                }
                sheet.Cell($"{(char)('D' + j)}{i + 2}").Value = competitor.TotalResult;
                i++;
            }

            workbook.SaveAs("standing.xlsx", false);
        }
    }
}
