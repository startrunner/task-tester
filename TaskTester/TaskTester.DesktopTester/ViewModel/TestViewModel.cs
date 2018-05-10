using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.DesktopTester.ViewModel
{
    public class TestViewModel
    {
        public string TestName { get; }
        public double MaxScore { get; set; } = double.NaN;
        public string TestGroup { get; set; } = "";

        public TestViewModel(string testName)
        {
            if (testName == null)
                throw new ArgumentNullException(nameof(testName));
            TestName = testName;
        }
    }
}
