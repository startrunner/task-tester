using System.Linq;
using GalaSoft.MvvmLight;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class PathSetViewModel : ViewModelBase
    {
        private string[] mPathsArray = null;

        public string[] PathsArray
        {
            get => mPathsArray;
            set
            {
                mPathsArray = value;
                RaisePropertyChanged(nameof(PathsArray));
                RaisePropertyChanged(nameof(Path));
            }
        }
        public string Path => PathsArray?.SingleOrDefault();
    }
}
