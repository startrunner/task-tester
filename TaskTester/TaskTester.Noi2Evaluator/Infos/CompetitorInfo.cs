using Code7248.word_reader;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaskTester.Noi2Evaluator.Infos
{
    class CompetitorInfo
    {
        public string Name { get; }
        public string Directory { get; }

        public CompetitorInfo(string name, string directory)
        {
            Name = name;
            Directory = directory;
        }

        private static string GetName(string folder)
        {
            var dirInfo = new DirectoryInfo(folder);

            List<string> files = dirInfo.GetFiles("info*.doc*").Select(x=>x.FullName).ToList();
            files.AddRange(dirInfo.GetFiles("info*.txt*").Select(x => x.FullName));

            foreach(string file in files)
            {
                if (file.EndsWith(".docx") || file.EndsWith(".doc"))
                {
                    try
                    {
                        TextExtractor t = new TextExtractor(file);

                        string txt = t.ExtractText();
                        ;
                        string rt = txt
                            .Split('\n', '\r', ',')
                            .Where(x => !x.ToLower().Contains("олимпиада"))
                            .Select(x => x.Replace("Име", "").Replace(":", "").Trim())
                            .FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        ;
                        return rt;
                    }
                    catch { }
                }
                else if (file.EndsWith(".txt"))
                {
                    return File
                    .ReadLines(file)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x));
                }
            }

            return string.Empty;
        }

        public static CompetitorInfo Get(string directory)
        {
            if(!System.IO.Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }

            string name = GetName(directory);
            ;
            return new CompetitorInfo(name, directory);
        }
    }
}
