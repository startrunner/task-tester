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

        public IOutputVerificationResult Verify(IOutputVerificationInfo info) => VerifyAsync(info).GetAwaiter().GetResult();

        public async Task<IOutputVerificationResult> VerifyAsync(IOutputVerificationInfo info)
        {
            await Task.Yield();
            var output = Normalize(info.StandardOutput.Str);
            var solution = Normalize(info.SolFile.Str);

            if (Enumerable.SequenceEqual(output, solution))
            {
                return new OutputVerificationResultMutable() {
                    Score = 1,
                    Type = OutputVerificationResultType.CorrectAnswer
                };
            }
            else
            {
                return new OutputVerificationResultMutable() {
                    Score = 0,
                    Type = OutputVerificationResultType.WrongAnswer
                };
            }
        }
    }
}
