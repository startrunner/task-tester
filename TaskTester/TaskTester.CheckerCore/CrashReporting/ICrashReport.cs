using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.CrashReporting
{
    public interface ICrashReport
    {
        string ExecutablePath { get; }
        string ExceptionCode { get; }
        string ExceptionMessage { get; }
        long ProcessID { get; }
    }

    public class CrashReportMutable:ICrashReport
    {
        public string ExecutablePath { get; set; }
        public string ExceptionCode { get; set; }
        public string ExceptionMessage { get; set; }
        public long ProcessID { get; set; }

        static internal ICrashReport Parse(string eventLogText)// => new CrashReportBuilder(CrashIniParser.Instance.Parse(ini.ToLower()));
        {
            var dic = CrashIniParser.Instance.Parse(eventLogText.ToLower());

            string exceptionMessage = "n/a";
            string exceptionCode = "0x0";
            string processId = "0x0";
            string executablePath = "0x0";

            dic.TryGetValue("exception code", out exceptionCode);
            dic.TryGetValue("faulting process id", out processId);
            dic.TryGetValue("faulting application path", out executablePath);
            Exceptions.ByCode.TryGetValue(exceptionCode, out exceptionMessage);

            return new CrashReportMutable() {
                ExceptionCode = exceptionCode,
                ExceptionMessage = exceptionMessage,
                ExecutablePath = executablePath,
                ProcessID = Convert.ToInt64(processId, 16)
            };
        }
    }
}
