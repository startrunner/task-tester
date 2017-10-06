using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResultBinders;
using TaskTester.CheckerCore.Tests.Properties;

namespace TaskTester.CheckerCore.Tests
{
    [TestClass]
    public class OutputVerifierTests
    {
        private static string ExtractExe(byte[] bytes)
        {
            string rt = Path.GetTempFileName() + ".exe";
            File.WriteAllBytes(rt, bytes);
            return rt;
        }

        [TestMethod]
        public void TestSum()
        {
            string checker = ExtractExe(Resources.checker_sum);
            IExecutableOutputVerifier verifier = new ExecutableOutputVerifierMutable {
                ExecutablePath= checker,
                Bindings = new IVerifierResultBinder[] {
                    new StdOutContainsBinder("1", new OutputVerificationResult (
                          OutputVerificationResultType.CorrectAnswer,
                          null,
                            1
                    )),
                    new StdOutContainsBinder("0", new OutputVerificationResult (
                        OutputVerificationResultType.WrongAnswer,
                         null,
                        0
                    ))
                },
                Arguments=new VerifierArgumentType[] {
                     VerifierArgumentType.Solution,//number A
                     VerifierArgumentType.Stdout//number B
                },
                 Stdin= VerifierArgumentType.ExitCode//number C
            };
            OutputVerificationInfo info = new OutputVerificationInfo (
                5,//C
                null,
                null,
                StringOrFile.FromText("2"),//B
                StringOrFile.FromText("3")//A
            );
            var result = verifier.Verify(info);
            Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
            ;
        }

        [TestMethod]
        public void TestIsOdd()
        {
            string checker = ExtractExe(Resources.checker_isodd);
            Random r = new Random(1337);
            for(int i=0;i<12;i++)
            {
                int number = r.Next();

                IOutputVerifier verifier = new ExecutableOutputVerifierMutable {
                    ExecutablePath = checker,
                    Stdin = VerifierArgumentType.Stdout,//number
                    Bindings = new IVerifierResultBinder[] {
                        new ExitCodeBinder(1, new OutputVerificationResult (
                             OutputVerificationResultType.CorrectAnswer,
                             null,
                             1
                        )),
                        new ExitCodeBinder(0, new OutputVerificationResult (
                            OutputVerificationResultType.CorrectAnswer,
                            null,
                            0
                        ))
                    }
                };
                OutputVerificationInfo info = new OutputVerificationInfo (
                    0,
                    null,
                    null,
                    StringOrFile.FromText(number.ToString()),
                    null
                );

                OutputVerificationResult result = verifier.Verify(info);
                Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
                Assert.AreEqual(result.Score, number % 2);
            }
        }

        [TestMethod]
        public void TestCmpFilesOK() => TestCmpfiles(true);

        [TestMethod]
        public void TestCmpFilesNO() => TestCmpfiles(false);

        public void TestCmpfiles(bool okay)
        {
            string checker = ExtractExe(Resources.checker_cmpfiles);
            Random r = new Random(1337);

            var file1 = Path.GetTempFileName();
            var file2 = Path.GetTempFileName();

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
                ExecutablePath = checker,
                Arguments = new VerifierArgumentType[] {
                    VerifierArgumentType.FileStdout,
                    VerifierArgumentType.FileSolution },
                Bindings = new IVerifierResultBinder[] {
                    new StdOutContainsBinder("OK", new OutputVerificationResult (
                         OutputVerificationResultType.CorrectAnswer,
                         null,
                         1
                    )),
                    new StdOutContainsBinder("NO", new OutputVerificationResult (
                        OutputVerificationResultType.WrongAnswer,
                        null,
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
            var result = verifier.Verify(info);

            if (okay)
            {
                Assert.AreEqual(result.Score, 1);
                Assert.AreEqual(result.Type, OutputVerificationResultType.CorrectAnswer);
            }
            else
            {
                Assert.AreEqual(result.Score, 0);
                Assert.AreEqual(result.Type, OutputVerificationResultType.WrongAnswer);
            }
        }
    }
}
