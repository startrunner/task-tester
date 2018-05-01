using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.CrashReporting
{
    public sealed class CrashReportFinder
    {
        public static readonly CrashReportFinder Instance = new CrashReportFinder();
        private CrashReportFinder() { }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(/*new Uri(path).LocalPath*/path)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        public IReadOnlyList<CrashReport> FindAll(long processID, string executablePath, int maxReportCount)
        {
            List<CrashReport> rt = new List<CrashReport>();

            using (var log = new EventLog("Application"))
            {
                foreach (EventLogEntry entry in log.Entries)
                {
                    if (rt.Count >= maxReportCount) return rt;
                    if (entry.EntryType != EventLogEntryType.Error) continue;
                    if (entry.InstanceId != 1000) continue;

                    CrashReport report = CrashReport.Parse(entry.Message);
                    if ((executablePath == null || NormalizePath(report.ExecutablePath).ToLower() == NormalizePath(executablePath).ToLower())
                        && report.ProcessID == processID)
                    {
                        rt.Add(report);
                    }

                }
            }

            return rt;
        }
    }
}
