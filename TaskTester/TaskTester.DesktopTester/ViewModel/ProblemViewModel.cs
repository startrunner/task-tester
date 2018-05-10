using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class ProblemViewModel : ViewModelBase
    {
        public PathSetViewModel TestInputFiles { get; } = new PathSetViewModel();
        [JsonProperty]
        public PathSetViewModel TestSolutionFiles { get; } = new PathSetViewModel();

        public CheckerViewModel Checker { get; } = new CheckerViewModel();

        public double TimeLimitSeconds { get; set; } = 1.0;

        public bool SortFilenamesAlphabetically { get; set; } = true;

        public ObservableCollection<TestViewModel> Tests { get; } = new ObservableCollection<TestViewModel>();

        [JsonIgnore]
        public ICommand SelectChecker { get; }

        public ProblemViewModel()
        {
            this.TestInputFiles.PropertyChanged += HandleInputFilesPropertyChanged;
            this.TestInputFiles.PropertyChanged += (x, e) => HandlePropertyChanged();
            this.TestSolutionFiles.PropertyChanged += (x, e) => HandlePropertyChanged();
        }

        private void HandlePropertyChanged()
        {
            RaisePropertyChanged(nameof(CanCreateTests));
        }

        private void HandleInputFilesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int testCount = TestInputFiles.PathsArray?.Length ?? 0;

            while (Tests.Count > testCount) Tests.RemoveAt(Tests.Count - 1);
            while(Tests.Count < testCount)
            {
                string testName = Path.GetFileNameWithoutExtension(TestInputFiles.PathsArray[Tests.Count]);
                Tests.Add(new TestViewModel(testName) {
                    MaxScore = Tests.Count == 0 ? 0 : double.NaN,
                    TestGroup = string.Empty
                });
            }
        }

        public bool CanCreateTests =>
            TestInputFiles.PathsArray != null &&
            TestInputFiles.PathsArray.Length == TestSolutionFiles.PathsArray?.Length;
        public bool TryCreateModel(out Problem problem)
        {
            if(TryCreateTestViewModels(out List<SolutionTest> tests))
            {
                problem = new Problem(tests);
                return true;
            }
            else
            {
                problem = null;
                return false;
            }
        }

        public bool TryCreateTestViewModels(out List<SolutionTest> tests)
        {
            //TODO: Fix this shit because it's ugly Java-ish practice
            if (!CanCreateTests)
            {
                tests = null;
                return false;
            }

            string[] inputs = TestInputFiles.PathsArray.ToArray();
            string[] expectedOutputs = TestSolutionFiles.PathsArray.ToArray();

            if (SortFilenamesAlphabetically)
            {
                Array.Sort<string>(inputs, (x, y) => x.CompareTo(y));
                Array.Sort<string>(expectedOutputs, (x, y) => x.CompareTo(y));
            }
            ;

            double totalExplicitScore =
                Tests
                .Where(x => !double.IsNaN(x.MaxScore))
                .Sum(x => x.MaxScore);

            int autoScoreCount =
                Tests
                .Where(x => double.IsNaN(x.MaxScore))
                .Count();

            double autoScore = (100.00 - totalExplicitScore) / autoScoreCount;


            tests = new List<SolutionTest>();
            for (int i = 0; i < inputs.Length; i++)
            {
                tests.Add(new SolutionTest(
                    inputFile: StringOrFile.FromFile(inputs[i]),
                    expectedOutputFile: StringOrFile.FromFile(expectedOutputs[i]),
                    outputVerifier: Checker.CreateModel(),
                    processArguments: String.Empty,
                    timeLimit: TimeSpan.FromSeconds(TimeLimitSeconds),
                    maxScore:
                        !double.IsNaN(Tests[i].MaxScore) ?
                        Tests[i].MaxScore :
                        autoScore,
                    testGroup: Tests[i].TestGroup ?? string.Empty
                ));
            }

            return true;
        }
    }
}
