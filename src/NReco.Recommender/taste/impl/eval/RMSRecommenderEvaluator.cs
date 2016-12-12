using System;

using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Eval
{
    /// <summary>
    /// A <see cref="NReco.CF.Taste.Eval.IRecommenderEvaluator"/> which computes the "root mean squared"
    /// difference between predicted and actual ratings for users. This is the square root of the average of this
    /// difference, squared.
    /// </summary>
    public sealed class RMSRecommenderEvaluator : AbstractDifferenceRecommenderEvaluator
    {
        private IRunningAverage average;

        protected override void Reset()
        {
            average = new FullRunningAverage();
        }

        protected override void ProcessOneEstimate(float estimatedPreference, IPreference realPref)
        {
            double diff = realPref.GetValue() - estimatedPreference;
            average.AddDatum(diff * diff);
        }

        protected override double ComputeFinalEvaluation()
        {
            return Math.Sqrt(average.GetAverage());
        }

        public override string ToString()
        {
            return "RMSRecommenderEvaluator";
        }
    }
}