using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public class CheckerViewModel : ViewModelBase
    {
        string[] mExecutablePathArray;

        public string[] ExecutablePathArray
        {
            get => mExecutablePathArray;
            set
            {
                mExecutablePathArray = value;
                RaisePropertyChanged(nameof(ExecutablePathArray));
                RaisePropertyChanged(nameof(ExecutableFilename));
                RaisePropertyChanged(nameof(ExecutablePath));
            }
        }
        public string ExecutablePath => ExecutablePathArray?.Single();
        public string ExecutableFilename => ExecutablePathArray?.Select(Path.GetFileName)?.SingleOrDefault();

        public ObservableCollection<CheckerBindingViewModel> Bindings { get; } = new ObservableCollection<CheckerBindingViewModel>();
        public ObservableCollection<EnumViewModel<ArgumentTypeViewModel>> Arguments { get; } = new ObservableCollection<EnumViewModel<ArgumentTypeViewModel>>();

        public ICommand AddArgument { get; }
        public ICommand AddBinding { get; }

        public CheckerViewModel()
        {
            AddArgument = new RelayCommand(AddArgumentExecute);
            AddBinding = new RelayCommand(AddBindingExecute);
        }

        private void AddBindingExecute() => Bindings.Add(new CheckerBindingViewModel());
        private void AddArgumentExecute() => Arguments.Add(default(ArgumentTypeViewModel));

        public IOutputVerifier CreateModel()
        {
            if (!File.Exists(ExecutablePath)) return DefaultOutputVerifier.Instance;
            var model = new ExecutableOutputVerifierMutable {
                Arguments = Arguments.Select(x => TranslateArgument(x.SelectedValue)).ToArray(),
                Bindings = Bindings.Select(x => x.CreateModel()).ToArray(),
                Stdin = VerifierArgumentType.None,
                ConsoleApplication = new FileSystemConsoleApplication(ExecutablePath)
            };
            return model;
        }
        private VerifierArgumentType TranslateArgument(ArgumentTypeViewModel selectedValue)
        {
            switch (selectedValue)
            {
                case ArgumentTypeViewModel.InputFilePath:
                    return VerifierArgumentType.FileStdin;
                case ArgumentTypeViewModel.OutputFilePath:
                    return VerifierArgumentType.FileStdout;
                case ArgumentTypeViewModel.SolutionFilePath:
                    return VerifierArgumentType.FileSolution;
                case ArgumentTypeViewModel.None:
                    return VerifierArgumentType.None;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}