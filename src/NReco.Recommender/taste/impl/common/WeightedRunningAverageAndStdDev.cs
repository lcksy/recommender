using System;

namespace NReco.CF.Taste.Impl.Common
{
    /// This subclass also provides for a weighted estimate of the sample standard deviation.
    /// See <a href="http://en.wikipedia.org/wiki/Mean_square_weighted_deviation">estimate formulae here</a>.
    public sealed class WeightedRunningAverageAndStdDev : WeightedRunningAverage, IRunningAverageAndStdDev
    {
        private double totalSquaredWeight;
        private double totalWeightedData;
        private double totalWeightedSquaredData;

        public WeightedRunningAverageAndStdDev()
        {
            totalSquaredWeight = 0.0;
            totalWeightedData = 0.0;
            totalWeightedSquaredData = 0.0;
        }

        public override void AddDatum(double datum, double weight)
        {
            lock (this)
            {
                base.AddDatum(datum, weight);
                totalSquaredWeight += weight * weight;
                double weightedData = datum * weight;
                totalWeightedData += weightedData;
                totalWeightedSquaredData += weightedData * datum;
            }
        }

        public override void RemoveDatum(double datum, double weight)
        {
            lock (this)
            {
                base.RemoveDatum(datum, weight);
                totalSquaredWeight -= weight * weight;
                if (totalSquaredWeight <= 0.0)
                {
                    totalSquaredWeight = 0.0;
                }
                double weightedData = datum * weight;
                totalWeightedData -= weightedData;
                if (totalWeightedData <= 0.0)
                {
                    totalWeightedData = 0.0;
                }
                totalWeightedSquaredData -= weightedData * datum;
                if (totalWeightedSquaredData <= 0.0)
                {
                    totalWeightedSquaredData = 0.0;
                }
            }
        }

        /// @throws NotSupportedException
        public void ChangeDatum(double delta, double weight)
        {
            throw new NotSupportedException();
        }

        public double GetStandardDeviation()
        {
            double totalWeight = GetTotalWeight();
            return Math.Sqrt((totalWeightedSquaredData * totalWeight - totalWeightedData * totalWeightedData)
                                 / (totalWeight * totalWeight - totalSquaredWeight));
        }

        public new IRunningAverageAndStdDev Inverse()
        {
            return new InvertedRunningAverageAndStdDev(this);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", GetAverage(), GetStandardDeviation());
        }
    }
}