using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.CrashReporting
{
    public sealed class CrashReportFinder : ICrashReportFinder
    {
        public static readonly CrashReportFinder Instance = new CrashReportFinder();
        private CrashReportFinder() { }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(/*new Uri(path).LocalPath*/path)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        //public IReadOnlyList<CrashReport> FindCrashReports(long processID, string executablePath, int maxReportCount)
        public IEnumerable<ICrashReport> FindCrashReports(Process process, int maxReportCount = int.MaxValue)
        {
            long processID = process.Id;
            string executablePath = process.GetMainModuleFileName(); //process.MainModule.FileName throws
            return FindCrashReports(processID, executablePath, maxReportCount);
        }

        public IEnumerable<ICrashReport> FindCrashReports(long processID, string executablePath, int maxReportCount)
        {

            //List<CrashReport> rt = new List<CrashReport>();
            int foundCount = 0;

            using (var log = new EventLog("Application"))
            {
                foreach (EventLogEntry entry in log.Entries)
                {
                    if (foundCount >= maxReportCount) yield break;
                    //if (rt.Count >= maxReportCount) return rt;
                    if (entry.EntryType != EventLogEntryType.Error) continue;
                    if (entry.InstanceId != 1000) continue;

                    CrashReport report = CrashReport.Parse(entry.Message);
                    if ((executablePath == null || NormalizePath(report.ExecutablePath).ToLower() == NormalizePath(executablePath).ToLower())
                        && report.ProcessID == processID)
                    {
                        //rt.Add(report);
                        yield return report;
                        ++foundCount;
                    }

                }
            }

            //return rt;
        }
    }
}
