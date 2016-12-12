using System;

using NReco.CF.Taste.Eval;

namespace NReco.CF.Taste.Impl.Eval
{
    public sealed class IRStatisticsImpl : IRStatistics
    {
        private double precision;
        private double recall;
        private double fallOut;
        private double ndcg;
        private double reach;

        public IRStatisticsImpl(double precision, double recall, double fallOut, double ndcg, double reach)
        {
            /*Preconditions.checkArgument(Double.isNaN(precision) || (precision >= 0.0 && precision <= 1.0),
                "Illegal precision: " + precision + ". Must be: 0.0 <= precision <= 1.0 or NaN");
            Preconditions.checkArgument(Double.isNaN(recall) || (recall >= 0.0 && recall <= 1.0), 
                "Illegal recall: " + recall + ". Must be: 0.0 <= recall <= 1.0 or NaN");
            Preconditions.checkArgument(Double.isNaN(fallOut) || (fallOut >= 0.0 && fallOut <= 1.0),
                "Illegal fallOut: " + fallOut + ". Must be: 0.0 <= fallOut <= 1.0 or NaN");
            Preconditions.checkArgument(Double.isNaN(ndcg) || (ndcg >= 0.0 && ndcg <= 1.0), 
                "Illegal nDCG: " + ndcg + ". Must be: 0.0 <= nDCG <= 1.0 or NaN");
            Preconditions.checkArgument(Double.isNaN(reach) || (reach >= 0.0 && reach <= 1.0), 
                "Illegal reach: " + reach + ". Must be: 0.0 <= reach <= 1.0 or NaN");*/
            this.precision = precision;
            this.recall = recall;
            this.fallOut = fallOut;
            this.ndcg = ndcg;
            this.reach = reach;
        }

        public double GetPrecision()
        {
            return precision;
        }

        public double GetRecall()
        {
            return recall;
        }

        public double GetFallOut()
        {
            return fallOut;
        }

        public double GetF1Measure()
        {
            return GetFNMeasure(1.0);
        }

        public double GetFNMeasure(double b)
        {
            double b2 = b * b;
            double sum = b2 * precision + recall;
            return sum == 0.0 ? Double.NaN : (1.0 + b2) * precision * recall / sum;
        }

        public double GetNormalizedDiscountedCumulativeGain()
        {
            return ndcg;
        }

        public double GetReach()
        {
            return reach;
        }

        public override string ToString()
        {
            return "IRStatisticsImpl[precision:" + precision + ",recall:" + recall + ",fallOut:"
                + fallOut + ",nDCG:" + ndcg + ",reach:" + reach + ']';
        }
    }
}