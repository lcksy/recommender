using NReco.CF.Taste.Impl.Model;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    sealed class SVDPreference : GenericPreference
    {
        private double cache;

        public SVDPreference(long userID, long itemID, float value, double cache)
            : base(userID, itemID, value)
        {
            SetCache(cache);
        }

        public double GetCache()
        {
            return cache;
        }

        public void SetCache(double value)
        {
            //Preconditions.checkArgument(!Double.isNaN(value), "NaN cache value");
            this.cache = value;
        }
    }
}