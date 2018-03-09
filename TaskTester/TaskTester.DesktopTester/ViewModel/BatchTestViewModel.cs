using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    class BatchTestViewModel : ViewModelBase
    {
        public event EventHandler ProblemPropertyChanged;
        public ObservableCollection<BatchTestProblemViewModel> Problems { get; }
            = new ObservableCollection<BatchTestProblemViewModel>();

        
        public ObservableCollection<PrimitiveViewModel<string>> CommandLines { get; }
            = new ObservableCollection<PrimitiveViewModel<string>>();

        
        public PrimitiveViewModel<string> Title { get; set; } = string.Empty;

        
        public PrimitiveViewModel<string> FolderPathCriteria { get; set; } = String.Empty;

        
        public PathSetViewModel RootDirectoy { get; set; } = new PathSetViewModel();

        
        public BatchTestProgressViewModel Progress { get; }

        [JsonIgnore]
        public ICommand AddProblem { get; }
        [JsonIgnore]
        public ICommand AddCommandLine { get; }

        public BatchTestViewModel()
        {
            AddProblem = new RelayCommand(AddProblemExecute);
            AddCommandLine = new RelayCommand(AddCommandLineExecute);
            AddProblemExecute();


            AddCommandLines(
                "g++ -o {0}.exe {0}.cpp -O2",
                "g++ -o {0}.exe {0}.cpp -O2 -std=c++11",
                "g++ -o {0}.exe {0}.cpp -O2 -std=c++17"
            );
            FolderPathCriteria = @"\A\";
            Title.Value = "Competition";

            Progress = new BatchTestProgressViewModel(
                rootDirectory: RootDirectoy,
                title: Title,
                directoryPathCriteria: FolderPathCriteria,
                commandLines: CommandLines,
                problems: Problems
            );
        }

        public BatchTestViewModel(IMessenger messenger) : base(messenger)
        {

        }

        private void AddCommandLines(params string[] lines)
        {
            foreach (string line in lines)
            {
                var commandline = new PrimitiveViewModel<string>(line);
                commandline.RemoveRequested += HandleCommandLineRemoveRequested;
                CommandLines.Add(commandline);
            }
        }

        private void AddCommandLineExecute()
        {
            PrimitiveViewModel<string> commandLine = string.Empty;
            commandLine.RemoveRequested += HandleCommandLineRemoveRequested;
            CommandLines.Add(commandLine);
        }

        private void HandleCommandLineRemoveRequested(object s, EventArgs e)
        {
            if (CommandLines.Count < 2) return;
            var sender = (PrimitiveViewModel<string>)s;
            this.CommandLines.Remove(sender);
        }

        private void AddProblemExecute()
        {
            var problem = new BatchTestProblemViewModel {
                Identifier = $"problem{Problems.Count}"
            };

            problem.Problem.PropertyChanged += Problem_PropertyChanged;
            problem.RemoveRequested += Problem_RemoveRequested;
            Problems.Add(problem);
        }

        private void Problem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => ProblemPropertyChanged?.Invoke(this, e);

        private void Problem_RemoveRequested(object sender, EventArgs e)
        {
            if (Problems.Count < 2) return;
            var problem = (BatchTestProblemViewModel)sender;
            problem.Problem.PropertyChanged -= Problem_PropertyChanged;
            Problems.Remove(problem);
        }
    }
}
