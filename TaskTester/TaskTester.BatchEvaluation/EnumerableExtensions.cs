using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTester.BatchEvaluation
{

    public static class EnumerableExtensions
    {
        public static Ranking<T> RankBy<T, TOrderBy>(this IEnumerable<T> items, Func<T, TOrderBy> rankBy)
        {
            TOrderBy[] criteria = items.Select(rankBy).ToArray();
            var ranking = new Ranking<T>(items.OrderByDescending(rankBy).ToArray());
            int count = ranking.Items.Count;

            for (int i = 0; i < count; i++)
            {
                if (i == 0 || !criteria[i - 1].Equals(criteria[i]))
                {
                    ranking.RanksMutable[i] = new Rank { From = i + 1, To = i + 1 };
                }
                else
                {
                    ranking.RanksMutable[i] = ranking.RanksMutable[i - 1];
                    ranking.RanksMutable[i].To = i + 1;
                }
            }

            return ranking;
        }
    }
}
