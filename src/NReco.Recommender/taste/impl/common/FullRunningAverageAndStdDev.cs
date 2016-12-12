using System;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// Extends <see cref="FullRunningAverage"/> to add a running standard deviation computation.
    /// Uses Welford's method, as described at http://www.johndcook.com/standard_deviation.html
    /// </summary>
    public sealed class FullRunningAverageAndStdDev : FullRunningAverage, IRunningAverageAndStdDev
    {
        private double stdDev;
        private double mk;
        private double sk;

        public FullRunningAverageAndStdDev()
        {
            mk = 0.0;
            sk = 0.0;
            recomputeStdDev();
        }

        public FullRunningAverageAndStdDev(int count, double average, double mk, double sk)
            : base(count, average)
        {
            this.mk = mk;
            this.sk = sk;
            recomputeStdDev();
        }

        public double GetMk()
        {
            return mk;
        }

        public double GetSk()
        {
            return sk;
        }

        public double GetStandardDeviation()
        {
            return stdDev;
        }

        public override void AddDatum(double datum)
        {
            lock (this)
            {
                base.AddDatum(datum);
                int count = GetCount();
                if (count == 1)
                {
                    mk = datum;
                    sk = 0.0;
                }
                else
                {
                    double oldmk = mk;
                    double diff = datum - oldmk;
                    mk += diff / count;
                    sk += diff * (datum - mk);
                }
                recomputeStdDev();
            }
        }

        public override void RemoveDatum(double datum)
        {
            lock (this)
            {
                int oldCount = GetCount();
                base.RemoveDatum(datum);
                double oldmk = mk;
                mk = (oldCount * oldmk - datum) / (oldCount - 1);
                sk -= (datum - mk) * (datum - oldmk);
                recomputeStdDev();
            }
        }

        /// @throws NotSupportedException
        public override void ChangeDatum(double delta)
        {
            throw new NotSupportedException();
        }

        private void recomputeStdDev()
        {
            int count = GetCount();
            stdDev = count > 1 ? Math.Sqrt(sk / (count - 1)) : Double.NaN;
        }

        public IRunningAverageAndStdDev Inverse()
        {
            return new InvertedRunningAverageAndStdDev(this);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", GetAverage(), stdDev);
        }
    }
}