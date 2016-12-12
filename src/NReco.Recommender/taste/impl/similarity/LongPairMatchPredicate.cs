using System;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary> A match predicate which will match an ID against either element of a
    /// <see cref="Tuple<long,long>"/>.
    /// </summary>
    public sealed class LongPairMatchPredicate
    {
        private long id;

        internal LongPairMatchPredicate(long id)
        {
            this.id = id;
        }

        public bool Matches(Tuple<long, long> pair)
        {
            return pair.Item1 == id || pair.Item2 == id;
        }
    }
}