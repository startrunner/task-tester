using System;
using System.Diagnostics;
using System.Text;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class ApplicationRunnerContext : IDisposable
    {
        public int processID;
        public Process process;

        private object
            errLock = new object(),
            outLock = new object();
        private StringBuilder
            stdErrBuilder = new StringBuilder(),
            stdOutBuilder = new StringBuilder();

        public ApplicationRunnerContext(Process process)
        {
            this.process = process;
            processID = process.Id;
        }

        public string GetStandardOutput()
        { lock (outLock) { return stdOutBuilder.ToString(); } }

        public string GetStandardError()
        { lock (errLock) { return stdErrBuilder.ToString(); } }

        public void AppendStandardOutput(string str)
        { lock (outLock) { stdOutBuilder.AppendLine(str); } }

        public void AppendStandardError(string str)
        { lock (errLock) { stdErrBuilder.Append(str); } }

        void IDisposable.Dispose()
        {
            try { process.Kill(); }
            catch { }
        }

        ~ApplicationRunnerContext() => (this as IDisposable).Dispose();
    }
}
