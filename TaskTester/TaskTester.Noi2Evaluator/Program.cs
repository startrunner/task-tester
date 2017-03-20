using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.Noi2Evaluator.Infos;

namespace TaskTester.Noi2Evaluator
{
    class Program
    {
        static List<CompetitorInfo> competitors = null;
        static CompetitionInfo competitionInfo = null;

        static void Init()
        {
            string batchText = File.ReadAllText("info.json");
            competitionInfo = JsonConvert.DeserializeObject<CompetitionInfo>(batchText);


            competitors = Directory
            .GetDirectories("works", "*", SearchOption.AllDirectories)
            .Where(dir=>dir.ToLower().Contains(competitionInfo.FolderCriteria.ToLower()))
            .Select(dir => CompetitorInfo.Get(dir))
            .ToList();

        }


        static void Main(string[] args)
        {
            Init();

            CompetitionResult result = null;

            if (!File.Exists("results.json"))
            {
                CompetitionEvaluator evaluator = new CompetitionEvaluator(competitionInfo, competitors);
                result = evaluator.Evaluate();
            }
            else
            {
                result = JsonConvert.DeserializeObject<CompetitionResult>(File.ReadAllText("results.json"));
            }


            CompetitionExporter exporter = new CompetitionExporter(result, competitionInfo);
            exporter.Export("results");
            ;
        }
    }
}
