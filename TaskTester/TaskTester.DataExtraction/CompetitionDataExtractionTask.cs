using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskTester.CheckerCore;
using TaskTester.Tasking;

namespace TaskTester.DataExtraction
{
    public sealed class CompetitionDataExtractionTask : BackgroundTask
    {
        readonly string mRootDirectory;
        readonly string mDirectoryPathCriteria;
        bool mStarted = false;

        public bool HasBeenStarted => mStarted;

        public event EventHandler<CompetitorInfoExtractedEventArgs> CompetitorInfoExtracted;

        public CompetitionDataExtractionTask(
            Dispatcher eventDispatcher,
            CancellationToken cancellationToken,
            string rootDirectory, 
            string directoryPathCriteria
        )
            :base(eventDispatcher, cancellationToken)
        {
            mRootDirectory = rootDirectory;
            mDirectoryPathCriteria = directoryPathCriteria;
            mEventDispatcher = eventDispatcher;
        }

        public override void Start() => Start(Run);

        private void Run()
        {
            string[] subDirectories = Directory.GetDirectories(mRootDirectory, "*", SearchOption.AllDirectories);
            int index = 0;
            foreach (string subDirectory in subDirectories.Where(x => x.Contains(mDirectoryPathCriteria)))
            {
                ExtractCompetitorData(subDirectory, index);
                index++;
            }
        }

        private void ExtractCompetitorData(string directory, int index)
        {
            if (!CompetitorDataExtractor.Instance.TryExtractCompetitorName(directory, out string competitorName))
            {
                competitorName = string.Empty;
            }

            string relativePath = GetRelativePath(mRootDirectory, directory);

            var eventArgs = new CompetitorInfoExtractedEventArgs(directory, relativePath, competitorName, index);
            Notify(CompetitorInfoExtracted, eventArgs);
        }

        string GetRelativePath(string rootDirectory, string fullPath)
        {
            var root = new Uri(rootDirectory);
            var full = new Uri(fullPath);
            return root.MakeRelativeUri(full).ToString();
        }

        private bool TryMarkAsStarted()
        {
            if (mStarted) return false;
            lock (mLock)
            {
                if (!mStarted)
                {
                    mStarted = true;
                    return true;
                }
            }
            return false;
        }
    }
}
