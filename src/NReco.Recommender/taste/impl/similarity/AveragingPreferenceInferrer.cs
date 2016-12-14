using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// Implementations of this interface compute an inferred preference for a user and an item that the user has
    /// not expressed any preference for. This might be an average of other preferences scores from that user, for
    /// example. This technique is sometimes called "default voting".
    /// </summary>
    public sealed class AveragingPreferenceInferrer : IPreferenceInferrer
    {
        private static float ZERO = 0.0f;

        private IDataModel dataModel;
        private Cache<long, float> averagePreferenceValue;

        public AveragingPreferenceInferrer(IDataModel dataModel)
        {
            this.dataModel = dataModel;
            IRetriever<long, float> retriever = new PrefRetriever(this);
            averagePreferenceValue = new Cache<long, float>(retriever, dataModel.GetNumUsers());
            Refresh(null);
        }

        public float InferPreference(long userID, long itemID)
        {
            return averagePreferenceValue.Get(userID);
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            averagePreferenceValue.Clear();
        }

        private sealed class PrefRetriever : IRetriever<long, float>
        {
            AveragingPreferenceInferrer inf;

            public PrefRetriever(AveragingPreferenceInferrer inf)
            {
                this.inf = inf;
            }

            public float Get(long key)
            {
                IPreferenceArray prefs = inf.dataModel.GetPreferencesFromUser(key);
                int size = prefs.Length();
                if (size == 0)
                {
                    return ZERO;
                }
                IRunningAverage average = new FullRunningAverage();
                for (int i = 0; i < size; i++)
                {
                    average.AddDatum(prefs.GetValue(i));
                }
                return (float)average.GetAverage();
            }
        }

        public override string ToString()
        {
            return "AveragingPreferenceInferrer";
        }
    }
}