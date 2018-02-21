using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TaskTester.Imaging;
using TaskTester.Spreadsheets;

namespace TaskTester.Spreaadsheets.ExporterDemo
{
    public class Class1
    {
        public static void Main()
        {
            var exporter = new SpreadsheetExporter<int>("The best competition ever") {
                { "Full Name", x => $"FullName of {x}" },
                { "Directory", x=>$"Directory of {x}" },
                { "Total", x=>$"Total of {x}" }
            };

            exporter.Add("problem1", x => new [] { "a1", "a2", "a3" });
            exporter.Add("problem2", x => new [] { "b1", "b2", "b3", "b4" });
            exporter.Add("problem3", x => new [] { "c1", "c2", "c3", "c4", "c5" });


            exporter.Export("wb.xlsx", Enumerable.Range(0, 20));

            Process.Start("wb.xlsx");
        }
    }
}
