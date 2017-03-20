using System;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;

namespace TaskTester.CheckerCore.ProcessRunning
{
    public class ApplicationRunner:IDisposable
    {
        object thisLock = new object();

        public string ExecutablePath { get; private set; }
        public TimeSpan MaxRuntime { get; set; } = TimeSpan.MaxValue;
        public StringOrFile StdIn { get; set; } = StringOrFile.FromText(string.Empty);
        public string ProcessArguments { get; set; } = "";
        public bool AllowCrashReports { get; set; } = true;

        public ApplicationRunner(string executablePath)
        {
            this.ExecutablePath = executablePath;
        }

        public async Task<IProcessRunResult> RunAsync()
        {
            await Task.Yield();
            ApplicationInstanceRunner run;
            lock (thisLock)
            {
                run = new ApplicationInstanceRunner(ExecutablePath)
                {
                    StdIn = StdIn,
                    MaxRuntime = MaxRuntime,
                    ProcessArguments = ProcessArguments,
                    AllowCrashReports = AllowCrashReports
                };
            }

            return await run.GoAsync();
        }

        public IProcessRunResult Run() =>
            RunAsync().Result;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~ApplicationRunner() { Dispose(false); }
        public void Dispose() => Dispose(true);
        #endregion
    }
}
