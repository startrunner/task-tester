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
        static List<CompetitorInfo> Competitors = null;
        static CompetitionInfo CompetitionInfo = null;

        static void Init()
        {
            if(!File.Exists("info.json"))
            {
                File.WriteAllText("info.json", JsonConvert.SerializeObject(new CompetitionInfo()));
            }
            if(!Directory.Exists("tests"))
            {
                Directory.CreateDirectory("tests");
            }
            if(!Directory.Exists("works"))
            {
                Directory.CreateDirectory("works");
            }

            string batchText = File.ReadAllText("info.json");
            CompetitionInfo = JsonConvert.DeserializeObject<CompetitionInfo>(batchText);


            Competitors = Directory
            .GetDirectories("works", "*", SearchOption.AllDirectories)
            .Where(dir=>dir.ToLower().Contains(CompetitionInfo.FolderCriteria.ToLower()))
            .Select(dir => CompetitorInfo.Get(dir))
            .ToList();

        }


        static void Main(string[] args)
        {
            Init();

            CompetitionResult result = null;

            if (!File.Exists("results.json"))
            {
                CompetitionEvaluator evaluator = new CompetitionEvaluator(CompetitionInfo, Competitors);
                result = evaluator.Evaluate();
            }
            else
            {
                result = JsonConvert.DeserializeObject<CompetitionResult>(File.ReadAllText("results.json"));
            }


            CompetitionExporter exporter = new CompetitionExporter(result, CompetitionInfo);
            exporter.Export("results");
            ;
        }
    }
}
