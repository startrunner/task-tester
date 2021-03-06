﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTester.CheckerCore.CrashReporting
{
    internal class CrashIniParser
    {
        static Lazy<CrashIniParser> instanceLazy = new Lazy<CrashIniParser>(() => new CrashIniParser());
        public static CrashIniParser Instance => instanceLazy.Value;

        public IReadOnlyDictionary<string, string> Parse(string ini)
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
