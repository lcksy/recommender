using System;

using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A simple <see cref="IRescorer"/> which always returns the original score.
    /// </summary>
    public sealed class NullRescorer<T> : IRescorer<T>, IDRescorer
    {
        internal NullRescorer()
        {
        }

        /// @param thing
        ///          to rescore
        /// @param originalScore
        ///          current score for item
        /// @return same originalScore as new score, always
        public double Rescore(T thing, double originalScore)
        {
            return originalScore;
        }

        public bool IsFiltered(T thing)
        {
            return false;
        }

        public double Rescore(long id, double originalScore)
        {
            return originalScore;
        }

        public bool IsFiltered(long id)
        {
            return false;
        }

        public override string ToString()
        {
            return "NullRescorer";
        }
    }

    public static class NullRescorer
    {
        private static IDRescorer USER_OR_ITEM_INSTANCE = new NullRescorer<long>();
        private static IRescorer<Tuple<long, long>> ITEM_ITEM_PAIR_INSTANCE = new NullRescorer<Tuple<long, long>>();
        private static IRescorer<Tuple<long, long>> USER_USER_PAIR_INSTANCE = new NullRescorer<Tuple<long, long>>();

        public static IDRescorer GetItemInstance()
        {
            return USER_OR_ITEM_INSTANCE;
        }

        public static IDRescorer GetUserInstance()
        {
            return USER_OR_ITEM_INSTANCE;
        }

        public static IRescorer<Tuple<long, long>> GetItemItemPairInstance()
        {
            return ITEM_ITEM_PAIR_INSTANCE;
        }

        public static IRescorer<Tuple<long, long>> GetUserUserPairInstance()
        {
            return USER_USER_PAIR_INSTANCE;
        }
    }
}