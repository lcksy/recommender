using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Eval
{
    /// <summary>
    /// Implementations of this inner interface are simple helper classes which create a <see cref="IRecommender"/> to be
    /// evaluated based on the given <see cref="IDataModel"/>.
    /// </summary>
    public interface IRecommenderBuilder
    {
        /// <summary>
        /// Builds a <see cref="IRecommender"/> implementation to be evaluated, using the given <see cref="IDataModel"/>.
        /// </summary>
        /// <param name="dataModel">{@link DataModel} to build the {@link Recommender} on</param>
        /// <returns>{@link Recommender} based upon the given {@link DataModel}</returns>
        /// <remarks>Throws TasteException if an error occurs while accessing the <see cref="IDataModel"/></remarks>
        IRecommender BuildRecommender(IDataModel dataModel);
    }
}