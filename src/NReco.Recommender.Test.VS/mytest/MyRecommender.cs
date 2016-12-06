using Microsoft.VisualStudio.TestTools.UnitTesting;
using NReco.CF.Taste.Impl.Model.File;
using NReco.CF.Taste.Impl.Neighborhood;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Impl.Similarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var recommendedItems = recommender.Recommend(1, 5);
        }
    }
}