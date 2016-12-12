using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Neighborhood;

namespace NReco.CF.Taste.Impl.Neighborhood
{
    /// <summary>
    /// A caching wrapper around an underlying <see cref="IUserNeighborhood"/> implementation. 
    /// </summary>
    public sealed class CachingUserNeighborhood : IUserNeighborhood
    {
        private IUserNeighborhood neighborhood;
        private Cache<long, long[]> neighborhoodCache;

        public CachingUserNeighborhood(IUserNeighborhood neighborhood, IDataModel dataModel)
        {
            //Preconditions.checkArgument(neighborhood != null, "neighborhood is null");
            this.neighborhood = neighborhood;
            int maxCacheSize = dataModel.GetNumUsers(); // just a dumb heuristic for sizing
            this.neighborhoodCache = new Cache<long, long[]>(new NeighborhoodRetriever(neighborhood), maxCacheSize);
        }

        public long[] GetUserNeighborhood(long userID)
        {
            return neighborhoodCache.Get(userID);
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            neighborhoodCache.Clear();
            var refreshed = RefreshHelper.BuildRefreshed(alreadyRefreshed);
            RefreshHelper.MaybeRefresh(refreshed, neighborhood);
        }

        private sealed class NeighborhoodRetriever : IRetriever<long, long[]>
        {
            private IUserNeighborhood neighborhood;

            internal NeighborhoodRetriever(IUserNeighborhood neighborhood)
            {
                this.neighborhood = neighborhood;
            }

            public long[] Get(long key)
            {
                return neighborhood.GetUserNeighborhood(key);
            }
        }
    }
}