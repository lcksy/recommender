
namespace NReco.CF.Taste.Eval
{
    /// <summary>
    /// Implementations encapsulate information retrieval-related statistics about a
    /// <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s recommendations.
    /// </summary>
    /// <remarks>See <a href="http://en.wikipedia.org/wiki/Information_retrieval">Information retrieval</a>.</remarks>
    public interface IRStatistics
    {
        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval#Precision">Precision</a>.
        /// </summary>
        double GetPrecision();

        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval#Recall">Recall</a>.
        /// </summary>
        double GetRecall();

        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval#Fall-Out">Fall-Out</a>.
        /// </summary>
        double GetFallOut();

        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval#F-measure">F-measure</a>.
        /// </summary>
        double GetF1Measure();

        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval#F-measure">F-measure</a>.
        /// </summary>
        double GetFNMeasure(double n);

        /// <summary>
        /// See <a href="http://en.wikipedia.org/wiki/Discounted_cumulative_gain#Normalized_DCG">
        /// Normalized Discounted Cumulative Gain</a>.
        /// </summary>
        double GetNormalizedDiscountedCumulativeGain();

        /// <summary>
        /// The fraction of all users for whom recommendations could be produced
        /// </summary>
        double GetReach();
    }
}