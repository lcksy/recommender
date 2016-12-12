using System;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;

namespace NReco.CF.Taste.Impl.Eval
{
    sealed class StatsCallable
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(StatsCallable));

        private Action _Delegate;
        private bool logStats;
        private IRunningAverageAndStdDev timing;
        private AtomicInteger noEstimateCounter;

        public StatsCallable(Action deleg,
                      bool logStats,
                      IRunningAverageAndStdDev timing,
                      AtomicInteger noEstimateCounter)
        {
            this._Delegate = deleg;
            this.logStats = logStats;
            this.timing = timing;
            this.noEstimateCounter = noEstimateCounter;
        }

        public void Call()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            _Delegate();
            stopWatch.Stop();
            timing.AddDatum(stopWatch.ElapsedMilliseconds);
            if (logStats)
            {
                int average = (int)timing.GetAverage();
                log.Info("Average time per recommendation: {}ms", average);
                long memory = GC.GetTotalMemory(false);
                log.Info("Approximate memory used: {}MB", memory / 1000000L);
                log.Info("Unable to recommend in {} cases", noEstimateCounter.Get());
            }
        }
    }
}