using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    public class BatchTestProblemResultViewModel : ViewModelBase
    {
        double mScore = double.NaN;

        [JsonIgnore]
        public string ScoreFormatted => Score.ToString("0");

        public double Score
        {
            get => mScore;
            set
            {
                mScore = value;
                RaisePropertyChanged(nameof(Score));
                RaisePropertyChanged(nameof(HasScore));
                RaisePropertyChanged(nameof(ScoreFormatted));
            }
        }

        [JsonIgnore]
        public bool HasScore => !double.IsNaN(Score);

        public ObservableCollection<BatchTestTestResultViewModel> TestResults { get; }
            = new ObservableCollection<BatchTestTestResultViewModel>();
    }
}
