using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Eval
{
    public sealed class LoadCallable
    {
        private IRecommender recommender;
        private long userID;

        public LoadCallable(IRecommender recommender, long userID)
        {
            this.recommender = recommender;
            this.userID = userID;
        }

        public void Call()
        {
            recommender.Recommend(userID, 10);
        }
    }
}