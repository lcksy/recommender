using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl;
using NReco.CF.Taste.Impl.Neighborhood;
using NReco.CF.Taste.Impl.Similarity;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Neighborhood;
using NReco.CF.Taste.Recommender;
using NReco.CF.Taste.Similarity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NReco.CF.Taste.Impl.Recommender;

namespace NReco.Recommender.Test.VS.taste.impl.recommender
{
    /// <p>Tests {@link GenericUserBasedRecommender}.</p>
    [TestClass]
    public class GenericUserBasedRecommenderTest : TasteTestCase
    {
        [TestMethod]
        public void testRecommender()
        {
            IRecommender recommender = buildRecommender();
            IList<IRecommendedItem> recommended = recommender.Recommend(1, 1);
            Assert.IsNotNull(recommended);
            Assert.AreEqual(1, recommended.Count);
            IRecommendedItem firstRecommended = recommended[0];
            Assert.AreEqual(2, firstRecommended.GetItemID());
            Assert.AreEqual(0.1f, firstRecommended.GetValue(), EPSILON);
            recommender.Refresh(null);
            Assert.AreEqual(2, firstRecommended.GetItemID());
            Assert.AreEqual(0.1f, firstRecommended.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testHowMany()
        {
            IDataModel dataModel = getDataModel(
                new long[] { 1, 2, 3, 4, 5 },
                new Double?[][] {
                    new double?[]{0.1, 0.2},
                    new double?[]{0.2, 0.3, 0.3, 0.6},
                    new double?[]{0.4, 0.4, 0.5, 0.9},
                    new double?[]{0.1, 0.4, 0.5, 0.8, 0.9, 1.0},
                    new double?[]{0.2, 0.3, 0.6, 0.7, 0.1, 0.2}
                }
            );
            IUserSimilarity similarity = new PearsonCorrelationSimilarity(dataModel);
            IUserNeighborhood neighborhood = new NearestNUserNeighborhood(2, similarity, dataModel);
            IRecommender recommender = new GenericUserBasedRecommender(dataModel, neighborhood, similarity);
            IList<IRecommendedItem> fewRecommended = recommender.Recommend(1, 2);
            IList<IRecommendedItem> moreRecommended = recommender.Recommend(1, 4);
            for (int i = 0; i < fewRecommended.Count; i++)
            {
                Assert.AreEqual(fewRecommended[i].GetItemID(), moreRecommended[i].GetItemID());
            }
            recommender.Refresh(null);
            for (int i = 0; i < fewRecommended.Count; i++)
            {
                Assert.AreEqual(fewRecommended[i].GetItemID(), moreRecommended[i].GetItemID());
            }
        }

        [TestMethod]
        public void testRescorer()
        {
            IDataModel dataModel = getDataModel(
                new long[] { 1, 2, 3 },
                new Double?[][] {
                    new double?[]{0.1, 0.2},
                    new double?[]{0.2, 0.3, 0.3, 0.6},
                    new double?[]{0.4, 0.5, 0.5, 0.9}
                }
            );
            IUserSimilarity similarity = new PearsonCorrelationSimilarity(dataModel);
            IUserNeighborhood neighborhood = new NearestNUserNeighborhood(2, similarity, dataModel);
            IRecommender recommender = new GenericUserBasedRecommender(dataModel, neighborhood, similarity);
            IList<IRecommendedItem> originalRecommended = recommender.Recommend(1, 2);

            IList<IRecommendedItem> rescoredRecommended = recommender.Recommend(1, 2, new ReversingRescorer<long>());
            Assert.IsNotNull(originalRecommended);
            Assert.IsNotNull(rescoredRecommended);
            Assert.AreEqual(2, originalRecommended.Count);
            Assert.AreEqual(2, rescoredRecommended.Count);
            Assert.AreEqual(originalRecommended[0].GetItemID(), rescoredRecommended[1].GetItemID());
            Assert.AreEqual(originalRecommended[1].GetItemID(), rescoredRecommended[0].GetItemID());
        }

        [TestMethod]
        public void testEstimatePref()
        {
            IRecommender recommender = buildRecommender();
            Assert.AreEqual(0.1f, recommender.EstimatePreference(1, 2), EPSILON);
        }

        [TestMethod]
        public void testBestRating()
        {
            IRecommender recommender = buildRecommender();
            IList<IRecommendedItem> recommended = recommender.Recommend(1, 1);
            Assert.IsNotNull(recommended);
            Assert.AreEqual(1, recommended.Count);
            IRecommendedItem firstRecommended = recommended[0];
            // item one should be recommended because it has a greater rating/score
            Assert.AreEqual(2, firstRecommended.GetItemID());
            Assert.AreEqual(0.1f, firstRecommended.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testMostSimilar()
        {
            IUserBasedRecommender recommender = buildRecommender();
            long[] similar = recommender.MostSimilarUserIDs(1, 2);
            Assert.IsNotNull(similar);
            Assert.AreEqual(2, similar.Length);
            Assert.AreEqual(2, similar[0]);
            Assert.AreEqual(3, similar[1]);
        }

        [TestMethod]
        public void testIsolatedUser()
        {
            IDataModel dataModel = getDataModel(
                new long[] { 1, 2, 3, 4 },
                new Double?[][] {
                    new double?[]{0.1, 0.2},
                    new double?[]{0.2, 0.3, 0.3, 0.6},
                    new double?[]{0.4, 0.4, 0.5, 0.9},
                    new double?[]{null, null, null, null, 1.0}
                }
            );
            IUserSimilarity similarity = new PearsonCorrelationSimilarity(dataModel);
            IUserNeighborhood neighborhood = new NearestNUserNeighborhood(3, similarity, dataModel);
            IUserBasedRecommender recommender = new GenericUserBasedRecommender(dataModel, neighborhood, similarity);
            long[] mostSimilar = recommender.MostSimilarUserIDs(4, 3);
            Assert.IsNotNull(mostSimilar);
            Assert.AreEqual(0, mostSimilar.Length);
        }

        private static IUserBasedRecommender buildRecommender()
        {
            IDataModel dataModel = getDataModel();
            IUserSimilarity similarity = new PearsonCorrelationSimilarity(dataModel);
            IUserNeighborhood neighborhood = new NearestNUserNeighborhood(2, similarity, dataModel);
            return new GenericUserBasedRecommender(dataModel, neighborhood, similarity);
        }
    }
}