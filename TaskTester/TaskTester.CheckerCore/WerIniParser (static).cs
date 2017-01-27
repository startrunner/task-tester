using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore
{
    internal static class WerIniParser
    {
        public static IReadOnlyDictionary<string, string> Parse(string ini)
        {
            string[] lines = ini
                .Split('\n')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            Dictionary<string, string> rt=new Dictionary<string, string>();

            foreach(string line in lines)
            {
                int firstColon = line.IndexOf(':');
                string key = line
                    .Substring(0, firstColon)
                    .Trim();
                string value = line
                    .Substring(firstColon + 1)
                    .Trim();

                rt.Add(key, value);
            }

            return rt;
        }
    }
}
