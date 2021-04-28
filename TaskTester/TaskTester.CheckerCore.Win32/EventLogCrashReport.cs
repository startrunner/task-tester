using System;
using System.Collections.Generic;
using TaskTester.CheckerCore.ProcessRunning;

namespace TaskTester.CheckerCore.CrashReporting
{
    public class CrashReport: ICrashReport
    {
        public string ExecutablePath { get; }
        public string ExceptionCode { get; }
        public string ErrorMessage { get; }
        public long ProcessID { get; }

        public CrashReport(string executablePath, string exceptionCode, string exceptionMessage, long processID)
        {
            ExecutablePath = executablePath;
            ExceptionCode = exceptionCode;
            ErrorMessage = exceptionMessage;
            ProcessID = processID;
        }

        static internal CrashReport Parse(string eventLogText)// => new CrashReportBuilder(CrashIniParser.Instance.Parse(ini.ToLower()));
        {
            IReadOnlyDictionary<string, string> dic = CrashIniParser.Instance.Parse(eventLogText.ToLower());

            string exceptionMessage = "n/a";
            string exceptionCode = "0x0";
            string processId = "0x0";
            string executablePath = "0x0";

            dic.TryGetValue("exception code", out exceptionCode);
            dic.TryGetValue("faulting process id", out processId);
            dic.TryGetValue("faulting application path", out executablePath);
            Exceptions.ByCode.TryGetValue(exceptionCode, out exceptionMessage);

            return new CrashReport(
                executablePath,
                exceptionCode,
                exceptionMessage,
                Convert.ToInt64(processId, 16)
            );
        }
    }
}
