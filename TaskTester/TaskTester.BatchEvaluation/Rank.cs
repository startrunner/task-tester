namespace TaskTester.BatchEvaluation
{
    public sealed class Rank
    {
        public int From { get; internal set; }
        public int To { get; internal set; }

        public override string ToString()
        {
            if (From == To) return From.ToString();
            else return $"{From} - {To}";
        }
    }
}
