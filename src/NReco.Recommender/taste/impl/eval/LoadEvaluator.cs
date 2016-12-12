using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Eval
{
    /// <summary>
    /// Simple helper class for running load on a Recommender.
    /// </summary>
    public sealed class LoadEvaluator
    {
        private LoadEvaluator() { }

        public static LoadStatistics RunLoad(IRecommender recommender)
        {
            return RunLoad(recommender, 10);
        }

        public static LoadStatistics RunLoad(IRecommender recommender, int howMany)
        {
            IDataModel dataModel = recommender.GetDataModel();
            int numUsers = dataModel.GetNumUsers();
            double sampleRate = 1000.0 / numUsers;
            var userSampler =
                SamplinglongPrimitiveIterator.MaybeWrapIterator(dataModel.GetUserIDs(), sampleRate);

            if (userSampler.MoveNext())
                recommender.Recommend(userSampler.Current, howMany); // Warm up

            var callables = new List<Action>();
            while (userSampler.MoveNext())
            {
                callables.Add(new LoadCallable(recommender, userSampler.Current).Call);
            }
            AtomicInteger noEstimateCounter = new AtomicInteger();
            IRunningAverageAndStdDev timing = new FullRunningAverageAndStdDev();
            AbstractDifferenceRecommenderEvaluator.Execute(callables, noEstimateCounter, timing);
            return new LoadStatistics(timing);
        }
    }
}