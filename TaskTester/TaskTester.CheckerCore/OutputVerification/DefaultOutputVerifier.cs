using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class DefaultOutputVerifier : IOutputVerifier
    {
        public static readonly DefaultOutputVerifier Instance = new DefaultOutputVerifier();
        private DefaultOutputVerifier() { }

        private static IEnumerable<string> Normalize(string text) =>
            text
            .Split(null as char[], StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        public OutputVerificationResult Verify(OutputVerificationInfo info)
        {
            IEnumerable<string> output = Normalize(info.StandardOutput.StringValue);
            IEnumerable<string> solution = Normalize(info.ExpectedOutput.StringValue);

            if (Enumerable.SequenceEqual(output, solution))
            {
                return new OutputVerificationResult(
                    OutputVerificationResultType.CorrectAnswer,
                    scoreMultiplier: 1
                );
            }
            else
            {
                return new OutputVerificationResult(
                    OutputVerificationResultType.WrongAnswer,
                    0
                );
            }
        }
    }
}
