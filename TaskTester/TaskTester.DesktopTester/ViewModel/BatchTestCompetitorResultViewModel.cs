using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    public class BatchTestCompetitorResultViewModel : ViewModelBase
    {
        string mName;
        string mDirectory;
        string mDirectoryRelative;
        int mTotalCommandCount = 1;
        int mExecutedCommandCount = 0;
        double mTotalResult = 0;

        
        public Dictionary<string, BatchTestProblemResultViewModel> ProblemResults { get; }
            = new Dictionary<string, BatchTestProblemResultViewModel>();

        public int ExecutedCommandCount
        {
            get => mExecutedCommandCount;
            set
            {
                mExecutedCommandCount = value;
                RaisePropertyChanged(nameof(ExecutedCommandCount));
            }
        }

        public int TotalCommandCount
        {
            get => mTotalCommandCount;
            set
            {
                mTotalCommandCount = value;
                RaisePropertyChanged(nameof(TotalCommandCount));
            }
        }

        public string Name
        {
            get => mName ?? string.Empty;
            set
            {
                mName = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string Directory
        {
            get => mDirectory;
            set
            {
                mDirectory = value;
                RaisePropertyChanged(nameof(Directory));
            }
        }

        public string DirectoryRelative
        {
            get => mDirectoryRelative;
            set
            {
                mDirectoryRelative = value;
                RaisePropertyChanged(nameof(DirectoryRelative));
            }
        }

        [JsonIgnore]
        public string TotalResultFormatted => TotalResult.ToString("0");

        public int TotalResultRounded => (int)Math.Round(TotalResult);

        public double TotalResult
        {
            get => mTotalResult;
            set
            {
                mTotalResult = value;
                RaisePropertyChanged(nameof(TotalResult));
                RaisePropertyChanged(nameof(TotalResultFormatted));
            }
        }
    }
}
