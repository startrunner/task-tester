using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.OutputVerification
{
    public class DefaultOutputVerifier : IOutputVerifier
    {
        private static IEnumerable<string> Normalize(string text) =>
            text
            .Split(null as char[], StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        public OutputVerificationResult Verify(OutputVerificationInfo info) => VerifyAsync(info).GetAwaiter().GetResult();

        public double PointsPerTest { get; set; } = 1;

        public async Task<OutputVerificationResult> VerifyAsync(OutputVerificationInfo info)
        {
            await Task.Yield();
            var output = Normalize(info.StandardOutput.Str);
            var solution = Normalize(info.SolFile.Str);

            if (Enumerable.SequenceEqual(output, solution))
            {
                return new OutputVerificationResult(
                    OutputVerificationResultType.CorrectAnswer,
                    null,
                    PointsPerTest
                );
            }
            else
            {
                return new OutputVerificationResult(
                    OutputVerificationResultType.WrongAnswer,
                    null,
                    0
                );
            }
        }
    }
}
