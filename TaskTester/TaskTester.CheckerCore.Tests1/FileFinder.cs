using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.Tests
{
    public static class FileFinder
    {
        public static string GetLocalFilePath(this string relativePath)
        {
            string directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string result = Path.Combine(directory, "LocalFiles", relativePath);
            return result;
        }
    }
}
