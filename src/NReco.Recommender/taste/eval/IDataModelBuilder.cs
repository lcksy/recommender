using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Eval
{
    /// <summary>
    /// Implementations of this inner interface are simple helper classes which create a <see cref="IDataModel"/> to be
    /// used while evaluating a <see cref="NReco.CF.Taste.Recommender.IRecommender"/>.
    /// </summary>
    /// <seealso cref="IRecommenderBuilder"/>
    /// <seealso cref="IRecommenderEvaluator"/>
    public interface IDataModelBuilder
    {
        /// <summary>
        /// Builds a {@link DataModel} implementation to be used in an evaluation, given training data.
        /// </summary>
        /// <param name="trainingData">data to be used in the <see cref="IDataModel"/></param>
        /// <returns><see cref="IDataModel"/> based upon the given data</returns>
        IDataModel BuildDataModel(FastByIDMap<IPreferenceArray> trainingData);
    }
}