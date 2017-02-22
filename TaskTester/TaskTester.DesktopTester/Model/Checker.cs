using System.Linq;

namespace TaskTester.DesktopTester.Model
{
    class Checker
    {
        public string ExecutablePath { get; set; }
        public Argument[] Args { get; set; } = Enumerable.Repeat(true, 5).Select(x => new Argument() { Type = ArgType.None }).ToArray();
        public CheckerBinding[] Bindings { get; set; } = Enumerable.Repeat(true, 3).Select(x => new CheckerBinding()).ToArray();
    }
}
