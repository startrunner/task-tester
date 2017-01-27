using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.DefaultChecker
{
   class Program
   {
      static void Main(string[] args)
      {
         string solutionFile = args[0];
         string answer = args[1];

         string solution = File.ReadAllText(solutionFile);
         if (answer.Trim() == solution.Trim())
         {
            Console.WriteLine("Correct Answer");
            Environment.Exit(0);
         }
         else
         {
            Console.WriteLine("Wrong Answer");
            Environment.Exit(1);
         }
      }
   }
}
