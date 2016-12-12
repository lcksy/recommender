using System;

using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Eval
{
    /// <summary>
    /// Implementations of this interface evaluate the quality of a
    /// <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s recommendations.
    /// </summary>
    public interface IRecommenderEvaluator
    {
        /// <summary>
        /// Evaluates the quality of a <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s recommendations.
        /// The range of values that may be returned depends on the implementation, but <em>lower</em> values must
        /// mean better recommendations, with 0 being the lowest / best possible evaluation, meaning a perfect match.
        /// This method does not accept a <see cref="NReco.CF.Taste.Recommender.IRecommender"/> directly, but
        /// rather a <see cref="NReco.CF.Taste.Recommender.IRecommenderBuilder"/> which can build the
        /// <see cref="NReco.CF.Taste.Recommender.IRecommender"/> to test on top of a given <see cref="IDataModel"/>.
        /// 
        /// <para>
        /// Implementations will take a certain percentage of the preferences supplied by the given <see cref="IDataModel"/>
        /// as "training data". This is typically most of the data, like 90%. This data is used to produce
        /// recommendations, and the rest of the data is compared against estimated preference values to see how much
        /// the <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s predicted preferences match the user's
        /// real preferences. Specifically, for each user, this percentage of the user's ratings are used to produce
        /// recommendations, and for each user, the remaining preferences are compared against the user's real
        /// preferences.
        /// </para>
        ///
        /// <para>
        /// For large datasets, it may be desirable to only evaluate based on a small percentage of the data.
        /// <code>evaluationPercentage</code> controls how many of the <see cref="IDataModel"/>'s users are used in
        /// evaluation.
        /// </para>
        ///
        /// <para>
        /// To be clear, <code>trainingPercentage</code> and <code>evaluationPercentage</code> are not related. They
        /// do not need to add up to 1.0, for example.
        /// </para>
        /// </summary>
        /// <param name="recommenderBuilder">object that can build a <see cref="NReco.CF.Taste.Recommender.IRecommender"/> to test</param>
        /// <param name="dataModelBuilder"><see cref="IDataModelBuilder"/> to use, or if null, a default <see cref="IDataModel"/> implementation will be used</param>     
        /// <param name="dataModel">dataset to test on</param>  
        /// <param name="trainingPercentage">
        /// percentage of each user's preferences to use to produce recommendations; the rest are compared
        /// to estimated preference values to evaluate
        /// <see cref="NReco.CF.Taste.Recommender.IRecommender"/> performance
        /// </param>
        /// <param name="evaluationPercentage">
        /// percentage of users to use in evaluation
        /// </param>
        /// <returns>
        /// a "score" representing how well the <see cref="NReco.CF.Taste.Recommender.IRecommender"/>'s
        /// estimated preferences match real values; <em>lower</em> scores mean a better match and 0 is a perfect match
        /// </returns>
        double Evaluate(IRecommenderBuilder recommenderBuilder,
                        IDataModelBuilder dataModelBuilder,
                        IDataModel dataModel,
                        double trainingPercentage,
                        double evaluationPercentage);

        /// @deprecated see {@link DataModel#getMaxPreference()}
        [Obsolete]
        float GetMaxPreference();

        [Obsolete]
        void SetMaxPreference(float maxPreference);

        /// @deprecated see {@link DataModel#getMinPreference()}
        [Obsolete]
        float GetMinPreference();

        [Obsolete]
        void SetMinPreference(float minPreference);
    }
}