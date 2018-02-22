using System;
using System.Linq;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    [JsonObject]
    public sealed class PathSetViewModel : ViewModelBase
    {
        [JsonProperty]
        private string[] mPathsArray = Array.Empty<string>();

        [JsonIgnore]
        public string[] PathsArray
        {
            get => mPathsArray;
            set
            {
                mPathsArray = value ?? Array.Empty<string>();
                RaisePropertyChanged(nameof(PathsArray));
                RaisePropertyChanged(nameof(Path));
            }
        }

        [JsonIgnore]
        public string Path => PathsArray?.SingleOrDefault();
    }
}
