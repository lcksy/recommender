
namespace NReco.CF.Taste.Impl.Common
{
    /// <p>
    /// A simple class that represents a fixed value of an average, count and standard deviation. This is useful
    /// when an API needs to return {@link RunningAverageAndStdDev} but is not in a position to accept
    /// updates to it.
    /// </p>
    public sealed class FixedRunningAverageAndStdDev : FixedRunningAverage, IRunningAverageAndStdDev
    {
        private double stdDev;

        public FixedRunningAverageAndStdDev(double average, double stdDev, int count)
            : base(average, count)
        {
            this.stdDev = stdDev;
        }

        public new IRunningAverageAndStdDev Inverse()
        {
            return new InvertedRunningAverageAndStdDev(this);
        }

        public override string ToString()
        {
            return base.ToString() + ',' + stdDev.ToString();
        }

        public double GetStandardDeviation()
        {
            return stdDev;
        }
    }
}