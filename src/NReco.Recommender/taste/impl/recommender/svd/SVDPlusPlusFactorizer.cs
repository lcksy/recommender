using System;
using System.Collections.Generic;

using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>
    /// SVD++, an enhancement of classical matrix factorization for rating prediction.
    /// Additionally to using ratings (how did people rate?) for learning, this model also takes into account
    /// who rated what.
    ///
    /// Yehuda Koren: Factorization Meets the Neighborhood: a Multifaceted Collaborative Filtering Model, KDD 2008.
    /// http://research.yahoo.com/files/kdd08koren.pdf
    /// </summary>
    public sealed class SVDPlusPlusFactorizer : RatingSGDFactorizer
    {
        private double[][] p;
        private double[][] y;
        private IDictionary<int, List<int>> itemsByUser;

        public SVDPlusPlusFactorizer(IDataModel dataModel, int numFeatures, int numIterations) :
            this(dataModel, numFeatures, 0.01, 0.1, 0.01, numIterations, 1.0)
        {
            biasLearningRate = 0.7;
            biasReg = 0.33;
        }

        public SVDPlusPlusFactorizer(IDataModel dataModel, int numFeatures, double learningRate, double preventOverfitting,
            double randomNoise, int numIterations, double learningRateDecay) :
            base(dataModel, numFeatures, learningRate, preventOverfitting, randomNoise, numIterations, learningRateDecay)
        {
        }

        protected override void PrepareTraining()
        {
            base.PrepareTraining();
            var random = RandomUtils.getRandom();

            p = new double[dataModel.GetNumUsers()][];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new double[numFeatures];
                for (int feature = 0; feature < FEATURE_OFFSET; feature++)
                {
                    p[i][feature] = 0;
                }
                for (int feature = FEATURE_OFFSET; feature < numFeatures; feature++)
                {
                    p[i][feature] = random.nextGaussian() * randomNoise;
                }
            }

            y = new double[dataModel.GetNumItems()][];
            for (int i = 0; i < y.Length; i++)
            {
                y[i] = new double[numFeatures];
                for (int feature = 0; feature < FEATURE_OFFSET; feature++)
                {
                    y[i][feature] = 0;
                }
                for (int feature = FEATURE_OFFSET; feature < numFeatures; feature++)
                {
                    y[i][feature] = random.nextGaussian() * randomNoise;
                }
            }

            /// get internal item IDs which we will need several times 
            itemsByUser = new Dictionary<int, List<int>>();
            var userIDs = dataModel.GetUserIDs();
            while (userIDs.MoveNext())
            {
                long userId = userIDs.Current;
                int userIdx = UserIndex(userId);
                FastIDSet itemIDsFromUser = dataModel.GetItemIDsFromUser(userId);
                List<int> itemIndexes = new List<int>(itemIDsFromUser.Count());
                itemsByUser[userIdx] = itemIndexes;
                foreach (long itemID2 in itemIDsFromUser)
                {
                    int i2 = ItemIndex(itemID2);
                    itemIndexes.Add(i2);
                }
            }
        }

        public override Factorization Factorize()
        {
            PrepareTraining();

            base.Factorize();

            for (int userIndex = 0; userIndex < userVectors.Length; userIndex++)
            {
                foreach (int itemIndex in itemsByUser[userIndex])
                {
                    for (int feature = FEATURE_OFFSET; feature < numFeatures; feature++)
                    {
                        userVectors[userIndex][feature] += y[itemIndex][feature];
                    }
                }
                double denominator = Math.Sqrt(itemsByUser[userIndex].Count);
                for (int feature = 0; feature < userVectors[userIndex].Length; feature++)
                {
                    userVectors[userIndex][feature] =
                        (float)(userVectors[userIndex][feature] / denominator + p[userIndex][feature]);
                }
            }

            return CreateFactorization(userVectors, itemVectors);
        }

        protected override void UpdateParameters(long userID, long itemID, float rating, double currentLearningRate)
        {
            int userIdx = UserIndex(userID);
            int itemIdx = ItemIndex(itemID);

            double[] userVector = p[userIdx];
            double[] itemVector = itemVectors[itemIdx];

            double[] pPlusY = new double[numFeatures];
            foreach (int i2 in itemsByUser[userIdx])
            {
                for (int f = FEATURE_OFFSET; f < numFeatures; f++)
                {
                    pPlusY[f] += y[i2][f];
                }
            }
            double denominator = Math.Sqrt(itemsByUser[userIdx].Count);
            for (int feature = 0; feature < pPlusY.Length; feature++)
            {
                pPlusY[feature] = (float)(pPlusY[feature] / denominator + p[userIdx][feature]);
            }

            double prediction = PredictRating(pPlusY, itemIdx);
            double err = rating - prediction;
            double normalized_error = err / denominator;

            // adjust user bias
            userVector[USER_BIAS_INDEX] +=
                biasLearningRate * currentLearningRate * (err - biasReg * preventOverfitting * userVector[USER_BIAS_INDEX]);

            // adjust item bias
            itemVector[ITEM_BIAS_INDEX] +=
                biasLearningRate * currentLearningRate * (err - biasReg * preventOverfitting * itemVector[ITEM_BIAS_INDEX]);

            // adjust features
            for (int feature = FEATURE_OFFSET; feature < numFeatures; feature++)
            {
                double pF = userVector[feature];
                double iF = itemVector[feature];

                double deltaU = err * iF - preventOverfitting * pF;
                userVector[feature] += currentLearningRate * deltaU;

                double deltaI = err * pPlusY[feature] - preventOverfitting * iF;
                itemVector[feature] += currentLearningRate * deltaI;

                double commonUpdate = normalized_error * iF;
                foreach (int itemIndex2 in itemsByUser[userIdx])
                {
                    double deltaI2 = commonUpdate - preventOverfitting * y[itemIndex2][feature];
                    y[itemIndex2][feature] += learningRate * deltaI2;
                }
            }
        }

        private double PredictRating(double[] userVector, int itemID)
        {
            double sum = 0;
            for (int feature = 0; feature < numFeatures; feature++)
            {
                sum += userVector[feature] * itemVectors[itemID][feature];
            }
            return sum;
        }
    }
}