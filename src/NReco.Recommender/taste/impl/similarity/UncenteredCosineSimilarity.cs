using System;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// An implementation of the cosine similarity. The result is the cosine of the angle formed between
    /// the two preference vectors.
    /// </summary>
    /// <remarks>
    /// Note that this similarity does not "center" its data, shifts the user's preference values so that each of their
    /// means is 0. For this behavior, use {@link PearsonCorrelationSimilarity}, which actually is mathematically
    /// equivalent for centered data.
    /// </remarks>
    public sealed class UncenteredCosineSimilarity : AbstractSimilarity
    {
        /// @{@link DataModel} does not have preference values
        public UncenteredCosineSimilarity(IDataModel dataModel)
            : this(dataModel, Weighting.UNWEIGHTED)
        {

        }

        /// @{@link DataModel} does not have preference values
        public UncenteredCosineSimilarity(IDataModel dataModel, Weighting weighting)
            : base(dataModel, weighting, false)
        {

            //Preconditions.checkArgument(dataModel.hasPreferenceValues(), "DataModel doesn't have preference values");
        }

        override protected double ComputeResult(int n, double sumXY, double sumX2, double sumY2, double sumXYdiff2)
        {
            if (n == 0)
            {
                return Double.NaN;
            }
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