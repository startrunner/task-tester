using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResiltBindings;
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
                Bindings = new IVerifierResultBinding[] {
                    new StdOutContainsBinding("1", new OutputVerificationResultMutable {
                         Score=1,
                          Type= OutputVerificationResultType.CorrectAnswer
                    }),
                    new StdOutContainsBinding("0", new OutputVerificationResultMutable {
                        Score=0,
                         Type= OutputVerificationResultType.WrongAnswer
                    })
                },
                Arguments=new VerifierArgumentType[] {
                     VerifierArgumentType.Solution,//number A
                     VerifierArgumentType.Stdout//number B
                },
                 Stdin= VerifierArgumentType.ExitCode//number C
            };
            IOutputVerificationInfo info = new OutputVerificationInfoMutable {
                ExitCode = 5,//C
                SolFile = StringOrFile.FromText("3"),//A
                StandardOutput = StringOrFile.FromText("2")//B
            };
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
                    Bindings = new IVerifierResultBinding[] {
                        new ExitCodeBinding(1, new OutputVerificationResultMutable {
                             Score=1,
                             Type= OutputVerificationResultType.CorrectAnswer
                        }),
                        new ExitCodeBinding(0, new OutputVerificationResultMutable {
                            Score=0,
                            Type= OutputVerificationResultType.CorrectAnswer
                        })
                    }
                };
                IOutputVerificationInfo info = new OutputVerificationInfoMutable {
                    StandardOutput = StringOrFile.FromText(number.ToString()),
                    ExitCode = 0
                };

                IOutputVerificationResult result = verifier.Verify(info);
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
                Bindings = new IVerifierResultBinding[] {
                    new StdOutContainsBinding("OK", new OutputVerificationResultMutable {
                         Score=1,
                         Type= OutputVerificationResultType.CorrectAnswer
                    }),
                    new StdOutContainsBinding("NO", new OutputVerificationResultMutable {
                        Score=0,
                        Type= OutputVerificationResultType.WrongAnswer
                    })
                },
                Stdin = VerifierArgumentType.None
            };
            IOutputVerificationInfo info = new OutputVerificationInfoMutable {
                StandardOutput = StringOrFile.FromFile(file1),
                SolFile = StringOrFile.FromFile(file2)
            };
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
