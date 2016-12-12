using System;

namespace NReco.CF.Taste.Impl.Common
{
    /// <p>
    /// A simple class that represents a fixed value of an average and count. This is useful
    /// when an API needs to return {@link RunningAverage} but is not in a position to accept
    /// updates to it.
    /// </p>
    public class FixedRunningAverage : IRunningAverage
    {
        private double average;
        private int count;

        public FixedRunningAverage(double average, int count)
        {
            this.average = average;
            this.count = count;
        }

        public void AddDatum(double datum)
        {
            throw new NotSupportedException();
        }

        public void RemoveDatum(double datum)
        {
            throw new NotSupportedException();
        }

        /// @throws NotSupportedException
        public void ChangeDatum(double delta)
        {
            throw new NotSupportedException();
        }

        public int GetCount()
        {
            return count;
        }

        public double GetAverage()
        {
            return average;
        }

        public IRunningAverage Inverse()
        {
            return new InvertedRunningAverage(this);
        }

        public override string ToString()
        {
            return average.ToString();
        }
    }
}