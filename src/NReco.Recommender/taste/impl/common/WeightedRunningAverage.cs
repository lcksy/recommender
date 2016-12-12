using System;

namespace NReco.CF.Taste.Impl.Common
{
    public class WeightedRunningAverage : IRunningAverage
    {
        private double totalWeight;
        private double average;

        public WeightedRunningAverage()
        {
            totalWeight = 0.0;
            average = Double.NaN;
        }

        public virtual void AddDatum(double datum)
        {
            AddDatum(datum, 1.0);
        }

        public virtual void AddDatum(double datum, double weight)
        {
            double oldTotalWeight = totalWeight;
            totalWeight += weight;
            if (oldTotalWeight <= 0.0)
            {
                average = datum;
            }
            else
            {
                average = average * oldTotalWeight / totalWeight + datum * weight / totalWeight;
            }
        }

        public virtual void RemoveDatum(double datum)
        {
            RemoveDatum(datum, 1.0);
        }

        public virtual void RemoveDatum(double datum, double weight)
        {
            double oldTotalWeight = totalWeight;
            totalWeight -= weight;
            if (totalWeight <= 0.0)
            {
                average = Double.NaN;
                totalWeight = 0.0;
            }
            else
            {
                average = average * oldTotalWeight / totalWeight - datum * weight / totalWeight;
            }
        }

        public virtual void ChangeDatum(double delta)
        {
            ChangeDatum(delta, 1.0);
        }

        public virtual void ChangeDatum(double delta, double weight)
        {
            //Preconditions.checkArgument(weight <= totalWeight, "weight must be <= totalWeight");
            average += delta * weight / totalWeight;
        }

        public virtual double GetTotalWeight()
        {
            return totalWeight;
        }

        /// @return {@link #getTotalWeight()} 
        public virtual int GetCount()
        {
            return (int)totalWeight;
        }

        public virtual double GetAverage()
        {
            return average;
        }

        public virtual IRunningAverage Inverse()
        {
            return new InvertedRunningAverage(this);
        }

        public override string ToString()
        {
            return Convert.ToString(average);
        }
    }
}