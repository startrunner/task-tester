using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TaskTester.Imaging;
using TaskTester.Spreadsheets;

namespace TaskTester.Spreaadsheets.ExporterDemo
{
    public class CompetitorResult
    {
        static readonly IReadOnlyList<string> FirstNames = new[] {
            "Ivan", "Alexander", "Georgi", "Pesho", "Gosho"
        };

        static readonly IReadOnlyList<string> LastNames = new[] {
            "Ivanov", "Georgiev", "Petrov", "Alexandrov"
        };

        public static CompetitorResult GetRandom(Random generator)
        {
            return new CompetitorResult {
                FirstName = FirstNames[generator.Next(FirstNames.Count)],
                LastName = LastNames[generator.Next(LastNames.Count)],
                Statuses =
                    Enumerable
                    .Repeat(0, 3)
                    .Select(x =>
                        Enumerable
                        .Repeat(0, generator.Next(0, 15))
                        .Select(_ => (double)generator.Next(10))
                        .ToArray()
                    )
                    .ToArray()
            };
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{FirstName} {LastName}";
        public IReadOnlyList<IReadOnlyList<double>> Statuses { get; private set; }
    }

    public class Class1
    {
        public static void Main()
        {
            Random generator = new Random(Seed: 32);
            CompetitorResult[] results =
                Enumerable.Repeat(-1, 20)
                .Select(_ => CompetitorResult.GetRandom(generator))
                .ToArray();

            var exporter = new SpreadsheetExporter<CompetitorResult>("The best competition ever") {
                { "Full Name", x => x.FullName },
                { "Total", x=>x.Statuses.Select(y=>y.Sum()).Sum().ToString() }
            };

            exporter.Add("problem1", x => x.Statuses[0].Select(Convert.ToString));
            exporter.Add("problem2", x => x.Statuses[1].Select(Convert.ToString));
            exporter.Add("problem3", x => x.Statuses[2].Select(Convert.ToString));


            exporter.Export("wb.xlsx", data: results);

            Process.Start("wb.xlsx");
        }
    }
}
