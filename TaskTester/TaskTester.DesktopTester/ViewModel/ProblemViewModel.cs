using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class ProblemViewModel
    {
        public PathSetViewModel TestInputFiles { get; } = new PathSetViewModel();
        public PathSetViewModel TestSolutionFiles { get; } = new PathSetViewModel();
        public CheckerViewModel Checker { get; } = new CheckerViewModel();
        public double TimeLimitSeconds { get; set; } = 1.0;
        public ICommand SelectChecker { get; }
        public bool SortFilenamesAlphabetically { get; set; } = true;

        public ObservableCollection<PrimitiveViewModel<double>> TestMaxScores { get; } = new ObservableCollection<PrimitiveViewModel<double>>();
        public ObservableCollection<PrimitiveViewModel<string>> TestGroups { get; } = new ObservableCollection<PrimitiveViewModel<string>>();


        public ProblemViewModel()
        {
            this.TestInputFiles.PropertyChanged += HandleInputFilesPropertyChanged;
        }

        private void HandleInputFilesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int testCount = TestInputFiles.PathsArray?.Length ?? 0;
            while (TestMaxScores.Count > testCount) TestMaxScores.RemoveAt(TestMaxScores.Count - 1);
            while (TestMaxScores.Count < testCount) TestMaxScores.Add(double.NaN);
            while (TestGroups.Count > testCount) TestGroups.RemoveAt(TestGroups.Count - 1);
            while (TestGroups.Count < testCount) TestGroups.Add(string.Empty);
        }

        public bool CanCreateTests =>
            TestInputFiles.PathsArray != null &&
            TestInputFiles.PathsArray.Length == TestSolutionFiles.PathsArray?.Length;
        public Problem CreateModel() => new Problem(CreateTestModels());

        public List<SolutionTest> CreateTestModels()
        {
            if (!CanCreateTests) throw new InvalidOperationException();

            string[] inputs = TestInputFiles.PathsArray.ToArray();
            string[] expectedOutputs = TestSolutionFiles.PathsArray.ToArray();

            if (SortFilenamesAlphabetically)
            {
                Array.Sort<string>(inputs, (x, y) => x.CompareTo(y));
                Array.Sort<string>(expectedOutputs, (x, y) => x.CompareTo(y));
            }
            ;


            var tests = new List<SolutionTest>();
            double lastValidTestScore = 100.0 / inputs.Length;
            for (int i = 0; i < inputs.Length; i++)
            {
                if(!double.IsNaN(TestMaxScores[i].Value))
                {
                    lastValidTestScore = TestMaxScores[i].Value;
                }

                tests.Add(new SolutionTest(
                    inputFile: StringOrFile.FromFile(inputs[i]),
                    expectedOutputFile: StringOrFile.FromFile(expectedOutputs[i]),
                    outputVerifier: Checker.CreateModel(),
                    processArguments: String.Empty,
                    timeLimit: TimeSpan.FromSeconds(TimeLimitSeconds),
                    maxScore: lastValidTestScore,
                    testGroup: TestGroups[i].Value ?? string.Empty
                ));
            }

            return tests;
        }
    }
}
