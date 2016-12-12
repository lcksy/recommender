using System;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// An implementation of the Pearson correlation. 
    /// </summary>
    /// <remarks>
    /// For users X and Y, the following values are calculated:
    /// <ul>
    /// <li>sumX2: sum of the square of all X's preference values</li>
    /// <li>sumY2: sum of the square of all Y's preference values</li>
    /// <li>sumXY: sum of the product of X and Y's preference value for all items for which both X and Y express a
    /// preference</li>
    /// </ul>
    /// The correlation is then:
    ///
    /// <para>
    /// <code> sumXY / sqrt(sumX2 * sumY2) </code>
    /// </para>
    ///
    /// <para>
    /// Note that this correlation "centers" its data, shifts the user's preference values so that each of their
    /// means is 0. This is necessary to achieve expected behavior on all data sets.
    /// </para>
    ///
    /// <para>
    /// This correlation implementation is equivalent to the cosine similarity since the data it receives
    /// is assumed to be centered -- mean is 0. The correlation may be interpreted as the cosine of the angle
    /// between the two vectors defined by the users' preference values.
    /// </para>
    ///
    /// <para>
    /// For cosine similarity on uncentered data, see <see cref="UncenteredCosineSimilarity"/>.
    /// </para> 
    /// </remarks>
    public sealed class PearsonCorrelationSimilarity : AbstractSimilarity
    {
        /// @{@link DataModel} does not have preference values
        public PearsonCorrelationSimilarity(IDataModel dataModel)
            : this(dataModel, Weighting.UNWEIGHTED)
        {

        }

        /// @{@link DataModel} does not have preference values
        public PearsonCorrelationSimilarity(IDataModel dataModel, Weighting weighting)
            : base(dataModel, weighting, true)
        {
            //Preconditions.checkArgument(dataModel.hasPreferenceValues(), "DataModel doesn't have preference values");
        }

        protected override double ComputeResult(int n, double sumXY, double sumX2, double sumY2, double sumXYdiff2)
        {
            if (n == 0)
            {
                return Double.NaN;
            }
            // Note that sum of X and sum of Y don't appear here since they are assumed to be 0;
            // the data is assumed to be centered.
            double denominator = Math.Sqrt(sumX2) * Math.Sqrt(sumY2);
            if (denominator == 0.0)
            {
                // One or both parties has -all- the same ratings;
                // can't really say much similarity under this measure
                return Double.NaN;
            }
            return sumXY / denominator;
        }
    }
}