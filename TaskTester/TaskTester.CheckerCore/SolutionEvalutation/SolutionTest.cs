using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;

namespace TaskTester.CheckerCore.SolutionEvalutation
{
    public sealed class SolutionTest
    {
        public StringOrFile Input { get; }
        public StringOrFile ExpectedOutput { get; }
        public IOutputVerifier OutputVerifier { get; }
        public string ProcessArguments { get; }
        public string TestGroup { get; }
        public TimeSpan TimeLimit { get; }
        public double MaxScore { get; }

        public SolutionTest(
            StringOrFile inputFile,
            StringOrFile expectedOutputFile,
            IOutputVerifier outputVerifier,
            string processArguments,
            TimeSpan timeLimit,
            double maxScore,
            string testGroup
        )
        {
            if (inputFile == null)
                throw new ArgumentNullException(nameof(inputFile));
            if (expectedOutputFile == null)
                throw new ArgumentNullException(nameof(expectedOutputFile));
            if (outputVerifier == null)
                throw new ArgumentNullException(nameof(outputVerifier));
            if (testGroup == null)
                throw new ArgumentNullException(nameof(testGroup));
            ProcessArguments = processArguments;
            Input = inputFile;
            ExpectedOutput = expectedOutputFile;
            OutputVerifier = outputVerifier;
            TimeLimit = timeLimit;
            MaxScore = maxScore;
            TestGroup = testGroup;
        }
    }
}
