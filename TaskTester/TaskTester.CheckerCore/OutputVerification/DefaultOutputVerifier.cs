using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class DefaultOutputVerifier : IOutputVerifier
    {
        public bool UsesSolution => true;
        public bool UsesStdout => true;
        public bool UsesStdErr => false;
        public bool UsesStdIn => false;

        private static IEnumerable<string> Normalize(string text) =>
            text
            .Split(null as char[], StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        public IOutputVerificationResult Verify(IProcessVerificationInfo info)
        {
            var output = Normalize(info.StandardOutput.Str);
            var solution = Normalize(info.SolFile.Str);

            if (Enumerable.SequenceEqual(output, solution))
            {
                return new OutputVerificationResultMutable() {
                    Score = 1,
                    Type = OutputVerificationType.CorrectAnswer
                };
            }
            else
            {
                return new OutputVerificationResultMutable() {
                    Score = 0,
                    Type = OutputVerificationType.WrongAnswer
                };
            }
        }
    }
}
