using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.CrashReporting
{
    public class CrashReportFinder
    {
        public long ProcessID { get; private set; }
        public string ExecutablePath { get;  set; }
        public int MaxReportCount { get; set; }

        public CrashReportFinder(long processID)
        {
            ProcessID = processID;
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        public async Task<IReadOnlyList<ICrashReport>> FindAsync()
        {
            await Task.Yield();

            List<ICrashReport> rt = new List<ICrashReport>();

            using (var log = new EventLog("Application"))
            {
                foreach (EventLogEntry entry in log.Entries)
                {
                    if (rt.Count >= MaxReportCount) return rt;
                    if (entry.EntryType != EventLogEntryType.Error) continue;
                    if (entry.InstanceId != 1000) continue;

                    ICrashReport report = CrashReportMutable.Parse(entry.Message);
                    if ((this.ExecutablePath == null || NormalizePath(report.ExecutablePath).ToLower() == NormalizePath(ExecutablePath).ToLower())
                        && report.ProcessID == ProcessID)
                    {
                        rt.Add(report);
                    }

                }
            }

            return rt;
        }

        public IReadOnlyList<ICrashReport> Find() => FindAsync().GetAwaiter().GetResult();
    }
}
