using System;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// <para>An implementation of a "similarity" based on the Euclidean "distance" between two users X and Y. Thinking
    /// of items as dimensions and preferences as points along those dimensions, a distance is computed using all
    /// items (dimensions) where both users have expressed a preference for that item. This is simply the square
    /// root of the sum of the squares of differences in position (preference) along each dimension.
    /// </para>
    /// 
    /// <para>The similarity could be computed as 1 / (1 + distance), so the resulting values are in the range (0,1].
    /// This would weight against pairs that overlap in more dimensions, which should indicate more similarity, 
    /// since more dimensions offer more opportunities to be farther apart. Actually, it is computed as 
    /// sqrt(n) / (1 + distance), where n is the number of dimensions, in order to help correct for this.
    /// sqrt(n) is chosen since randomly-chosen points have a distance that grows as sqrt(n).</para>
    ///
    /// <para>Note that this could cause a similarity to exceed 1; such values are capped at 1.</para>
    /// 
    /// <para>Note that the distance isn't normalized in any way; it's not valid to compare similarities computed from
    /// different domains (different rating scales, for example). Within one domain, normalizing doesn't matter much as
    /// it doesn't change ordering.
    /// </para>
    /// </summary>
    public sealed class EuclideanDistanceSimilarity : AbstractSimilarity
    {
        /// @{@link DataModel} does not have preference values
        public EuclideanDistanceSimilarity(IDataModel dataModel)
            : this(dataModel, Weighting.UNWEIGHTED)
        {

        }

        /// @{@link DataModel} does not have preference values
        public EuclideanDistanceSimilarity(IDataModel dataModel, Weighting weighting) :
            base(dataModel, weighting, false)
        {

            //Preconditions.checkArgument(dataModel.hasPreferenceValues(), "DataModel doesn't have preference values");
        }

        override protected double ComputeResult(int n, double sumXY, double sumX2, double sumY2, double sumXYdiff2)
        {
            return 1.0 / (1.0 + Math.Sqrt(sumXYdiff2) / Math.Sqrt(n));
        }
    }
}