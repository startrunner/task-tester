using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code7248.word_reader;

namespace TaskTester.DataExtraction
{
    public sealed class CompetitorDataExtractor
    {
        public static readonly CompetitorDataExtractor Instance =
            new CompetitorDataExtractor();

        private CompetitorDataExtractor() { }

        public bool TryExtractCompetitorName(string folder, out string name)
        {
            var dirInfo = new DirectoryInfo(folder);

            List<string> files = dirInfo.GetFiles("info*.doc*").Select(x => x.FullName).ToList();
            files.AddRange(dirInfo.GetFiles("info*.txt*").Select(x => x.FullName));

            foreach (string file in files)
            {
                if (TryExtractText(file, out string fileText))
                {
                    name =
                        fileText
                        .Split('\n', '\r', ',')
                        .Where(x => !x.ToLower().Contains("олимпиада"))
                        .Select(x => x.Replace("Име", "").Replace(":", "").Trim())
                        .FirstOrDefault(x => !string.IsNullOrEmpty(x));
                    return true;
                }
            }

            name = null;
            return false;
        }

        private static bool TryExtractText(string file, out string fileText)
        {
            fileText = null;
            bool success = false;
            if (file.EndsWith(".docx") || file.EndsWith(".doc"))
            {
                try
                {
                    TextExtractor t = new TextExtractor(file);
                    fileText = t.ExtractText();
                    success = true;
                }
                catch { }
            }
            else if (file.EndsWith(".txt"))
            {
                try
                {
                    fileText = File.ReadAllText(file);
                    success = true;
                }
                catch { }
            }

            return success;
        }
    }
}
