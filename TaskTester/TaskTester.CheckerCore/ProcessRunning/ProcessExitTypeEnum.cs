namespace TaskTester.CheckerCore.ProcessRunning
{
    public enum ProcessExitType
    {
        /// <summary>
        /// Process has exited in time without crashing
        /// </summary>
        Graceful,

        /// <summary>
        /// Process' execution has been too slow and it's been killed by the instance runner.
        /// </summary>
        Forced,

        /// <summary>
        /// RiP
        /// </summary>
        Crashed,

        /// <summary>
        /// Fallback value; Should never get assigned
        /// </summary>
        Undetermined
    }
}
