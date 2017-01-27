using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore
{
    /// <summary>
    /// An object used to store child processes of the current one.
    /// A ProcessParent will automatically kill all child processes on destruction
    /// </summary>
    class ProcessParent
    {
        public static ProcessParent Instance { get; private set; } = new ProcessParent();
        private ProcessParent() { }

        private List<Process> runningProcesses = new List<Process>();

        private void KillProcess(Process p)
        {
            try
            {
                p.Kill();
            }
            catch { }
            
        }
        public void KillAllProcesses()
        {
            foreach (var v in runningProcesses) KillProcess(v);
            runningProcesses.Clear();
        }

        public Process StartProcess(ProcessStartInfo psi)
        {
            Process rt = Process.Start(psi);
            runningProcesses.Add(rt);
            return rt;
        }

        public Process StartProcess(ProcessStartInfo psi, TimeSpan lifespan)
        {
            Process rt = StartProcess(psi);
            Task.Run(async () =>
            {
                await Task.Delay(lifespan);
                KillProcess(rt);
            });
            return rt;
        }

        ~ProcessParent()
        {
            KillAllProcesses();
        }
    }
}
