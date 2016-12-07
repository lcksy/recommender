using Microsoft.VisualStudio.TestTools.UnitTesting;
using NReco.CF.Taste.Impl.Model.File;
using NReco.CF.Taste.Impl.Neighborhood;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Impl.Similarity;

namespace NReco.Recommender.Test.VS.mytest
{
    [TestClass]
    public class MyRecommender
    {
        [TestMethod]
        public void Test()
        {
            var model = new FileDataModel("data.csv");
            var similarity = new LogLikelihoodSimilarity(model);
            var neighborhood = new NearestNUserNeighborhood(3, similarity, model);
            var recommender = new GenericUserBasedRecommender(model, neighborhood, similarity);


            //var recommender2 = new GenericItemBasedRecommender(model,

            var recommendedItems = recommender.Recommend(1, 5);

            
        }
    }
}