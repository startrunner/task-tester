using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


        public ObservableCollection<PrimitiveViewModel<double>> TestMaxScores { get; } = new ObservableCollection<PrimitiveViewModel<double>>();

        public ObservableCollection<PrimitiveViewModel<string>> TestGroups { get; } = new ObservableCollection<PrimitiveViewModel<string>>();

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
            while (TestMaxScores.Count > testCount) TestMaxScores.RemoveAt(TestMaxScores.Count - 1);
            while (TestMaxScores.Count < testCount)
            {
                if (TestMaxScores.Count == 0) TestMaxScores.Add(0);
                else TestMaxScores.Add(double.NaN);
            }
            while (TestGroups.Count > testCount) TestGroups.RemoveAt(TestGroups.Count - 1);
            while (TestGroups.Count < testCount) TestGroups.Add(string.Empty);
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
                TestMaxScores
                .Where(x => !double.IsNaN(x.Value))
                .Sum(x => x.Value);

            int autoScoreCount =
                TestMaxScores
                .Where(x => double.IsNaN(x.Value))
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
                        !double.IsNaN(TestMaxScores[i].Value) ?
                        TestMaxScores[i].Value :
                        autoScore,
                    testGroup: TestGroups[i].Value ?? string.Empty
                ));
            }

            return true;
        }
    }
}
