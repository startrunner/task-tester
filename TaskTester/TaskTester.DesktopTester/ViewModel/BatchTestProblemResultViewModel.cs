using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace TaskTester.DesktopTester.ViewModel
{
    public class BatchTestProblemResultViewModel : ViewModelBase
    {
        double mScore = double.NaN;

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

        public bool HasScore => !double.IsNaN(Score);

        public string Text => "Kur";
        public ObservableCollection<BatchTestTestResultViewModel> TestResults { get; }
            = new ObservableCollection<BatchTestTestResultViewModel>();
    }
}
