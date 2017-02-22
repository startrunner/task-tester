using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using TaskTester.DesktopTester.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace TaskTester.DesktopTester.ViewModel
{
    class CheckerViewModel:ViewModelBase
    {
        public Checker Model { get; private set; }

        public string ExecutablePath
        {
            get { return Model.ExecutablePath; }
            set
            {
                Model.ExecutablePath = value;
                RaisePropertyChanged(nameof(ExecutablePath));
            }
        }
        public ObservableCollection<CheckerBindingViewModel> Bindings { get; private set; } = new ObservableCollection<CheckerBindingViewModel>();
        public ObservableCollection<ArgumentViewModel> Arguments { get; private set; } = new ObservableCollection<ArgumentViewModel>();

        public ICommand BrowseExecutable { get; private set; }

        public CheckerViewModel():this(null)
        {

        }

        public CheckerViewModel(Checker model = null)
        {
            this.Model = model;

            BrowseExecutable = new RelayCommand(BrowseExecutableExecute);

            if (model == null) return;



            foreach(var bind in model.Bindings)
            {
                Bindings.Add(new CheckerBindingViewModel(bind));
            }
            foreach(var arg in model.Args)
            {
                Arguments.Add(new ArgumentViewModel(arg));
            }
        }

        private void BrowseExecutableExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = false;
            ofd.Multiselect = false;
            ofd.Filter = "Console Application |*.exe";

            ofd.ShowDialog();

            if (string.IsNullOrEmpty(ofd.FileName) || ofd.CheckFileExists)
            {
                ExecutablePath = ofd.FileName;
                RaisePropertyChanged("ExecutablePath");
                RaisePropertyChanged("RunTests");
            }
        }
    }
}
