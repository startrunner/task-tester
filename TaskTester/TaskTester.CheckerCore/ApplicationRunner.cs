using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore
{
    public class ApplicationRunner:IDisposable
    {
        object thisLock = new object();

        public string ExecutablePath { get; private set; }
        public TimeSpan MaxRuntime { get; set; } = TimeSpan.MaxValue;
        public string StdIn { get; set; } = string.Empty;

        public ApplicationRunner(string executablePath)
        {
            this.ExecutablePath = executablePath;
        }

        public async Task<ProcessRunResult> RunAsync()
        {
            await Task.Yield();
            ApplicationInstanceRunner run;
            lock (thisLock)
            {
                run = new ApplicationInstanceRunner(ExecutablePath) {
                    StdIn = StdIn,
                    MaxRuntime = MaxRuntime,
                };
            }

            return await run.GoAsync();
        }

        public ProcessRunResult Run() =>
            RunAsync().GetAwaiter().GetResult();

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
