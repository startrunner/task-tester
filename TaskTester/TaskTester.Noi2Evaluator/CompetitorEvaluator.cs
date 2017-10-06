using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTester.CheckerCore.Common;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.CheckerCore.OutputVerification.ResultBinders;
using TaskTester.CheckerCore.ProcessRunning;
using TaskTester.Noi2Evaluator.Infos;

namespace TaskTester.Noi2Evaluator
{
    class CompetitorEvaluator
    {
        CompetitionInfo competitionInfo;
        CompetitorInfo competitorInfo;

        public CompetitorEvaluator(CompetitionInfo competitionInfo, CompetitorInfo competitorInfo)
        {
            this.competitionInfo = competitionInfo;
            this.competitorInfo = competitorInfo;
        }

        void RunCommandLines()
        {
            foreach (string problem in competitionInfo.Problems.Select(x => x.Name))
            {
                foreach (string cmd in competitionInfo.CommandLines.Select(x => string.Format(x, problem)))
                {
                    Process cmdProc = new Process()
                    {
                        StartInfo = new ProcessStartInfo("cmd.exe")
                        {
                            Arguments = $"/c {cmd}",
                            WorkingDirectory = competitorInfo.Directory,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        },
                        EnableRaisingEvents = true
                    };
                    cmdProc.OutputDataReceived += ((s, e) => { });
                    cmdProc.ErrorDataReceived += ((s, e) => { });
                    cmdProc.Start();
                    cmdProc.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
                    if (!cmdProc.HasExited) cmdProc.Kill();
                }
            }
        }


        static double TestProblemForSingleTest(string exeDir, StringOrFile input, StringOrFile solution, double maxScore, double maxRuntime, IOutputVerifier verifier)
        {
            ApplicationRunner runner = new ApplicationRunner();

            ProcessRunResult runResult = runner.Run(
                exeDir,
                TimeSpan.FromSeconds(maxRuntime),
                input,
                processArguments: null,
                allowCrashReports: false
            );

            switch (runResult.ExitType)
            {
                case ProcessExitType.Crashed:
                    return 0;
                case ProcessExitType.Forced:
                    return 0;
                case ProcessExitType.Undetermined:
                    return 0;
                case ProcessExitType.Graceful:
                    break;
            }

            OutputVerificationInfo verificationInfo = new OutputVerificationInfo(
                runResult.ExitCode,
                runResult.StdErr,
                input,
                runResult.StdOut,
                solution
            );
            OutputVerificationResult verificationResult = verifier.Verify(verificationInfo);

            return verificationResult.Score;
        }

        double TestProblem(ProblemInfo problemInfo)
        {
            string exeDir = Path.Combine(competitorInfo.Directory, $"{problemInfo.Name}.exe");
            if (!File.Exists(exeDir)) return 0;

            string[] inFiles = Directory.GetFiles(Path.Combine("tests", problemInfo.Name), "*.in").ToArray();
            string[] solFiles = Directory.GetFiles(Path.Combine("tests", problemInfo.Name), "*.sol").ToArray();
            int[] testGroups = null;
            if (problemInfo.TestGroups != null && problemInfo.TestGroups.Length == inFiles.Length)
            {
                testGroups = problemInfo.TestGroups;
            }

            double pointsPerTest = 100.0 / inFiles.Length;

            IOutputVerifier verifier = new DefaultOutputVerifier()
            {
                PointsPerTest = pointsPerTest
            };
            if (problemInfo.CheckerBindings != null && problemInfo.CheckerBindings.Count != 0)
            {
                verifier = new ExecutableOutputVerifierMutable {
                    ExecutablePath = Path.GetFullPath(Path.Combine("tests", problemInfo.Name, "checker.exe")),
                    Bindings = problemInfo.CheckerBindings
                     .Select(x => new StdOutContainsBinder(x.SearchText, new OutputVerificationResult(
                         OutputVerificationResultType.CouldNotBind,
                         null,
                         x.Points
                     )))
                     .ToArray(),
                    Arguments = new VerifierArgumentType[] {
                          VerifierArgumentType.FileStdin,
                          VerifierArgumentType.FileStdout,
                          VerifierArgumentType.FileSolution
                     }
                };
            }


            double[] testPoints = new double[inFiles.Length];

            for (int i = 0; i < inFiles.Length; i++)
            {
                testPoints[i] = TestProblemForSingleTest(exeDir, StringOrFile.FromFile(inFiles[i]), StringOrFile.FromFile(solFiles[i]), pointsPerTest, problemInfo.TimeLimit, verifier);
            }

            if (testGroups != null)
            {
                var groupMap = new Dictionary<int, int>();
                //GROUP ID, COUNT
                foreach (int group in testGroups)
                {
                    if (!groupMap.ContainsKey(group)) groupMap[group] = 1;
                    else groupMap[group]++;
                }

                for (int i = 0; i < testPoints.Length; i++)
                {
                    if (testPoints[i] != 0)
                    {
                        groupMap[testGroups[i]]--;
                    }
                }

                double points = 0;

                for (int i = 0; i < testPoints.Length; i++)
                {
                    if (groupMap[testGroups[i]] == 0)
                    {
                        points += testPoints[i];
                    }
                }

                return points;
            }
            else
            {
                return testPoints.Sum();
            }
        }

        IEnumerable<double> TestProblems() => competitionInfo.Problems.Select(x => TestProblem(x));

        public CompetitorResult Evaluate()
        {
            Console.WriteLine($"Evaluating {competitorInfo.Name} at {competitorInfo.Directory}...");
            var rt = new CompetitorResult();
            DateTime evalStartTime = DateTime.Now;

            Console.Write("Running command lines...");
            RunCommandLines();
            Console.WriteLine("Done!");

            Console.Write("Testing problems...");
            rt.ProblemResults.AddRange(TestProblems());
            Console.WriteLine("Done!");

            Console.WriteLine("Results: " + string.Join(" ", rt.ProblemResults.Select(x => (int)x)));
            Console.WriteLine($"Total: {rt.TotalResult}");


            DateTime evalEndTime = DateTime.Now;
            TimeSpan evalDuration = (evalEndTime - evalStartTime);
            rt.EvaluationDuration = evalDuration;
            rt.Name = competitorInfo.Name;
            rt.Directory = competitorInfo.Directory;
            return rt;
        }
    }
}
