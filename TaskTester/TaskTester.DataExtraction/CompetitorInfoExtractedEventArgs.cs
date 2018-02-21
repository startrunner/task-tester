using System;

namespace TaskTester.DataExtraction
{
    public sealed class CompetitorInfoExtractedEventArgs : EventArgs
    {
        public string Directory { get; }
        public string DirectoryRelative { get; }
        public string Name { get; }
        public int Index { get; }

        public CompetitorInfoExtractedEventArgs(string directory, string directoryRelative, string name, int index)
        {
            Directory = directory;
            DirectoryRelative = directoryRelative;
            Name = name;
            Index = index;
        }
    }
}
