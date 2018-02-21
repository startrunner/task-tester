using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.CrashReporting
{
    public class CrashReportFinder
    {
        public long ProcessID { get; }
        public string ExecutablePath { get; }
        public int MaxReportCount { get; }

        public CrashReportFinder(long processID, string executablePath, int maxReportCount = 10000)
        {
            ProcessID = processID;
            ExecutablePath = executablePath;
            MaxReportCount = maxReportCount;
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(/*new Uri(path).LocalPath*/path)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        public async Task<IReadOnlyList<CrashReport>> FindAsync()
        {
            await Task.Yield();

            List<CrashReport> rt = new List<CrashReport>();

            using (var log = new EventLog("Application"))
            {
                foreach (EventLogEntry entry in log.Entries)
                {
                    if (rt.Count >= MaxReportCount) return rt;
                    if (entry.EntryType != EventLogEntryType.Error) continue;
                    if (entry.InstanceId != 1000) continue;

                    CrashReport report = CrashReport.Parse(entry.Message);
                    if ((this.ExecutablePath == null || NormalizePath(report.ExecutablePath).ToLower() == NormalizePath(ExecutablePath).ToLower())
                        && report.ProcessID == ProcessID)
                    {
                        rt.Add(report);
                    }

                }
            }

            return rt;
        }

        public IReadOnlyList<CrashReport> Find() => FindAsync().GetAwaiter().GetResult();
    }
}
