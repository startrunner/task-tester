using System;
using System.IO;
using NUnit.Framework;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResultBinders;
using TaskTester.CheckerCore.SolutionEvalutation;

namespace TaskTester.CheckerCore.Tests
{
    [TestFixture]
    public class OutputVerifierTests
    {
        private static string ExtractExe(byte[] bytes)
        {
            string rt = Path.GetTempFileName() + ".exe";
            File.WriteAllBytes(rt, bytes);
            return rt;
        }

        [Test]
        public void TestSum()
        {
            string checker = Path.GetFullPath("checker_sum.exe".GetLocalFilePath());
            IExecutableOutputVerifier verifier = new ExecutableOutputVerifierMutable {
                ConsoleApplication = new FileSystemConsoleApplication(checker),
                Bindings = new IVerifierResultBinder[] {
                    new StdOutContainsBinder("1", new OutputVerificationResult (
                          OutputVerificationResultType.CorrectAnswer,
                            1
                    )),
                    new StdOutContainsBinder("0", new OutputVerificationResult (
                        OutputVerificationResultType.WrongAnswer,
                        0
                    ))
                },
                Arguments = new VerifierArgumentType[] {
                     VerifierArgumentType.Solution,//number A
                     VerifierArgumentType.Stdout//number B
                },
                Stdin = VerifierArgumentType.ExitCode//number C
            };
            OutputVerificationInfo info = new OutputVerificationInfo(
                5,//C
                null,
                null,
                StringOrFile.FromText("2"),//B
                StringOrFile.FromText("3")//A
            );
            OutputVerificationResult result = verifier.Verify(info);
            Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
            ;
        }

        [Test]
        public void TestIsOdd()
        {
            string checker = Path.GetFullPath("checker_isodd.exe".GetLocalFilePath());
            Random r = new Random(1337);
            for (int i = 0; i < 12; i++)
            {
                int number = r.Next();

                IOutputVerifier verifier = new ExecutableOutputVerifierMutable {
                    ConsoleApplication = new FileSystemConsoleApplication(checker),
                    Stdin = VerifierArgumentType.Stdout,//number
                    Bindings = new IVerifierResultBinder[] {
                        new ExitCodeBinder(1, new OutputVerificationResult (
                             OutputVerificationResultType.CorrectAnswer,
                             1
                        )),
                        new ExitCodeBinder(0, new OutputVerificationResult (
                            OutputVerificationResultType.CorrectAnswer,
                            0
                        ))
                    }
                };
                OutputVerificationInfo info = new OutputVerificationInfo(
                    0,
                    null,
                    null,
                    StringOrFile.FromText(number.ToString()),
                    null
                );

                OutputVerificationResult result = verifier.Verify(info);
                Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
                Assert.AreEqual(result.ScoreMultiplier, number % 2);
            }
        }

        [Test]
        public void TestCmpFilesOK() => TestCmpfiles(true);

        [Test]
        public void TestCmpFilesNO() => TestCmpfiles(false);

        public void TestCmpfiles(bool okay)
        {
            string checker = Path.GetFullPath("checker_cmpfiles.exe".GetLocalFilePath());
            Random r = new Random(1337);

            string file1 = Path.GetTempFileName();
            string file2 = Path.GetTempFileName();

            File.WriteAllText(file1, "The quick brown fox jumps over the lazy dog");
            if (okay)
            {
                File.WriteAllText(file2, "The quick brown fox jumps over the lazy dog");
            }
            else
            {
                File.WriteAllText(file2, "The quick brown fox jumps over the lazy dog.");
            }

            IOutputVerifier verifier = new ExecutableOutputVerifierMutable {
                ConsoleApplication = new FileSystemConsoleApplication(checker),
                Arguments = new VerifierArgumentType[] {
                    VerifierArgumentType.FileStdout,
                    VerifierArgumentType.FileSolution },
                Bindings = new IVerifierResultBinder[] {
                    new StdOutContainsBinder("OK", new OutputVerificationResult (
                         OutputVerificationResultType.CorrectAnswer,
                         1
                    )),
                    new StdOutContainsBinder("NO", new OutputVerificationResult (
                        OutputVerificationResultType.WrongAnswer,
                        0
                    ))
                },
                Stdin = VerifierArgumentType.None
            };
            OutputVerificationInfo info = new OutputVerificationInfo(
                0,
                null,
                null,
                StringOrFile.FromFile(file1),
                StringOrFile.FromFile(file2)
            );
            OutputVerificationResult result = verifier.Verify(info);

            if (okay)
            {
                Assert.AreEqual(result.ScoreMultiplier, 1);
                Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
            }
            else
            {
                Assert.AreEqual(result.ScoreMultiplier, 0);
                Assert.AreEqual(result.Type, OutputVerificationResultType.WrongAnswer);
            }
        }
    }
}
