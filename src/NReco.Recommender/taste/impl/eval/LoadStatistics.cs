using NReco.CF.Taste.Impl.Common;

namespace NReco.CF.Taste.Impl.Eval
{
    public sealed class LoadStatistics
    {
        private IRunningAverage timing;

        public LoadStatistics(IRunningAverage timing)
        {
            this.timing = timing;
        }

        public IRunningAverage GetTiming()
        {
            return timing;
        }
    }
}