using System;

namespace TaskTester.BatchEvaluation
{
    public sealed class BatchEvaluationCompetitor
    {
        public int Index { get; }
        public string Directory { get; }

        public BatchEvaluationCompetitor(int index, string directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));
            Index = index;
            Directory = directory;
        }
    }
}
