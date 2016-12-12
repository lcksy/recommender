using System;

using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Eval
{
    /// <summary>
    /// A <see cref="NReco.CF.Taste.Eval.IRecommenderEvaluator"/> which computes the average absolute
    /// difference between predicted and actual ratings for users.
    /// </summary>
    /// <remarks>This algorithm is also called "mean average error".</remarks>
    public sealed class AverageAbsoluteDifferenceRecommenderEvaluator : AbstractDifferenceRecommenderEvaluator
    {
        private IRunningAverage average;

        protected override void Reset()
        {
            average = new FullRunningAverage();
        }

        protected override void ProcessOneEstimate(float estimatedPreference, IPreference realPref)
        {
            average.AddDatum(Math.Abs(realPref.GetValue() - estimatedPreference));
        }

        protected override double ComputeFinalEvaluation()
        {
            return average.GetAverage();
        }

        public override string ToString()
        {
            return "AverageAbsoluteDifferenceRecommenderEvaluator";
        }
    }
}