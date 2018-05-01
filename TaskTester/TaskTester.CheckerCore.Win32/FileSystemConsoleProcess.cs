using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TaskTester.CheckerCore.CrashReporting;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.CheckerCore.ProcessRunning
{
    internal class FileSystemConsoleProcess : IDisposable, IConsoleProcess
    {
        string mExecutablePath;
        int mProcessID;
        Process mProcess;

        private object
            errLock = new object(),
            outLock = new object();
        private StringBuilder
            stdErrBuilder = new StringBuilder(),
            stdOutBuilder = new StringBuilder();

        public static FileSystemConsoleProcess Start(string executablePath, string processArguments)
        {
            Process osProcess = new Process {
                StartInfo = new ProcessStartInfo(executablePath) {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    Arguments = processArguments
                },
                EnableRaisingEvents = true
            };

            osProcess.Start();
            osProcess.BeginOutputReadLine();
            osProcess.BeginErrorReadLine();

            FileSystemConsoleProcess process = new FileSystemConsoleProcess(executablePath, osProcess);
            return process;
        }

        private FileSystemConsoleProcess(string executablePath, Process process)
        {
            mProcess = process;
            mProcessID = process.Id;
            mExecutablePath = executablePath;

            mProcess.ErrorDataReceived += HandleStandardErrorDataReceived;
            mProcess.OutputDataReceived += HandleStandardOutputDataReceived;
        }

        public void WriteStandardInput(string input)
        {
            mProcess.StandardInput.Write(input);
        }

        public int ExitCode => mProcess.ExitCode;

        public bool HasExited => mProcess.HasExited;

        public void CloseStandardInput()
        {
            try
            {
                mProcess.StandardInput.Flush();
                mProcess.StandardInput.Close();
            }
            catch { }
        }

        public void WaitForExit(int timeLimit = int.MaxValue) => mProcess.WaitForExit(timeLimit);

        public string GetStandardOutput()
        { lock (outLock) { return stdOutBuilder.ToString(); } }

        public string GetStandardError()
        { lock (errLock) { return stdErrBuilder.ToString(); } }

        public void HandleStandardOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            lock (outLock)
            {
                stdOutBuilder.AppendLine(args.Data);
            }
        }

        public void HandleStandardErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            lock (errLock)
            {
                stdErrBuilder.Append(args.Data);
            }
        }

        void IDisposable.Dispose() => EnsureKilled();

        public TimeSpan ExecutionTime
        {
            get
            {
                if (mProcess.HasExited) return mProcess.TotalProcessorTime;
                else return DateTime.Now - mProcess.StartTime;
            }
        }

        public void EnsureKilled()
        {
            try { mProcess.Kill(); }
            catch { }
        }

        public bool HasCrashed(out ICrashReport report)
        {
            if (mProcess.ExitCode != 0)
            {
                report =
                    CrashReportFinder
                    .Instance
                    .FindAll(mProcessID, mExecutablePath, maxReportCount: 1)
                    .FirstOrDefault();

                return report != null;
            }
            else
            {
                report = null;
                return false;
            }
        }

        ~FileSystemConsoleProcess() => (this as IDisposable).Dispose();
    }
}
