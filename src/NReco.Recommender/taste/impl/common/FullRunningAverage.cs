using System;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// A simple class that can keep track of a running average of a series of numbers. One can add to or remove
    /// from the series, as well as update a datum in the series. The class does not actually keep track of the
    /// series of values, just its running average, so it doesn't even matter if you remove/change a value that
    /// wasn't added.
    /// </summary>
    public class FullRunningAverage : IRunningAverage
    {
        private int count;
        private double average;

        public FullRunningAverage()
            : this(0, Double.NaN)
        {
        }

        public FullRunningAverage(int count, double average)
        {
            this.count = count;
            this.average = average;
        }

        /// @param datum
        /// new item to add to the running average
        public virtual void AddDatum(double datum)
        {
            if (++count == 1)
            {
                average = datum;
            }
            else
            {
                average = average * (count - 1) / count + datum / count;
            }
        }

        /// @param datum
        ///          item to remove to the running average
        /// @throws InvalidOperationException
        ///           if count is 0
        public virtual void RemoveDatum(double datum)
        {
            if (count == 0)
            {
                throw new InvalidOperationException();
            }
            if (--count == 0)
            {
                average = Double.NaN;
            }
            else
            {
                average = average * (count + 1) / count - datum / count;
            }
        }

        /// @param delta
        ///          amount by which to change a datum in the running average
        /// @throws InvalidOperationException
        /// if count is 0
        public virtual void ChangeDatum(double delta)
        {
            if (count == 0)
            {
                throw new InvalidOperationException();
            }
            average += delta / count;
        }

        public virtual int GetCount()
        {
            return count;
        }

        public virtual double GetAverage()
        {
            return average;
        }

        public IRunningAverage Inverse()
        {
            return new InvertedRunningAverage(this);
        }

        public override string ToString()
        {
            return Convert.ToString(average);
        }
    }
}