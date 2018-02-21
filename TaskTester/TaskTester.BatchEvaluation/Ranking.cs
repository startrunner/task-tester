using System;
using System.Collections;
using System.Collections.Generic;

namespace TaskTester.BatchEvaluation
{
    public sealed class Ranking<TItem> : IReadOnlyList<Tuple<Rank, TItem>>
    {
        internal Rank[] RanksMutable { get; }
        internal TItem[] ItemsMutable { get; }

        internal Ranking(TItem[] items)
        {
            ItemsMutable = items;
            RanksMutable = new Rank[items.Length];
        }

        public IReadOnlyList<Rank> Ranks => RanksMutable;
        public IReadOnlyList<TItem> Items => ItemsMutable;

        public int Count => RanksMutable.Length;

        public Tuple<Rank, TItem> this[int index] =>
            Tuple.Create(RanksMutable[index], ItemsMutable[index]);


        public IEnumerator<Tuple<Rank, TItem>> GetEnumerator() => Enumerate().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

        private IEnumerable<Tuple<Rank, TItem>> Enumerate()
        {
            for(int i=0;i<RanksMutable.Length;i++)
            {
                yield return Tuple.Create(RanksMutable[i], ItemsMutable[i]);
            }
            yield break;
        }
    }
}
