using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Eval
{
    /// <summary>
    /// Implementations collect information retrieval-related statistics on a
    /// <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s performance, including precision, recall and
    /// f-measure.
    /// </summary>
    /// 
    /// <remarks>
    /// See <a href="http://en.wikipedia.org/wiki/Information_retrieval">Information retrieval</a>.
    /// </remarks>
    public interface IRecommenderIRStatsEvaluator
    {
        /// @param recommenderBuilder
        ///          object that can build a {@link NReco.CF.Taste.Recommender.Recommender} to test
        /// @param dataModelBuilder
        ///          {@link DataModelBuilder} to use, or if null, a default {@link DataModel} implementation will be
        ///          used
        /// @param dataModel
        ///          dataset to test on
        /// @param rescorer
        ///          if any, to use when computing recommendations
        /// @param at
        ///          as in, "precision at 5". The number of recommendations to consider when evaluating precision,
        ///          etc.
        /// @param relevanceThreshold
        ///          items whose preference value is at least this value are considered "relevant" for the purposes
        ///          of computations
        /// @return {@link IRStatistics} with resulting precision, recall, etc.
        /// @throws TasteException
        ///           if an error occurs while accessing the {@link DataModel}
        IRStatistics Evaluate(IRecommenderBuilder recommenderBuilder,
                              IDataModelBuilder dataModelBuilder,
                              IDataModel dataModel,
                              IDRescorer rescorer,
                              int at,
                              double relevanceThreshold,
                              double evaluationPercentage);
    }
}