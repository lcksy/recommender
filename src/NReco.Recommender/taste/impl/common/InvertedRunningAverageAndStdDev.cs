using System;

namespace NReco.CF.Taste.Impl.Common
{
    public sealed class InvertedRunningAverageAndStdDev : IRunningAverageAndStdDev
    {
        private IRunningAverageAndStdDev _Delegate;

        public InvertedRunningAverageAndStdDev(IRunningAverageAndStdDev deleg)
        {
            this._Delegate = deleg;
        }

        public void AddDatum(double datum)
        {
            throw new NotSupportedException();
        }

        public void RemoveDatum(double datum)
        {
            throw new NotSupportedException();
        }

        public void ChangeDatum(double delta)
        {
            throw new NotSupportedException();
        }

        public int GetCount()
        {
            return _Delegate.GetCount();
        }

        public double GetAverage()
        {
            return -_Delegate.GetAverage();
        }

        public double GetStandardDeviation()
        {
            return _Delegate.GetStandardDeviation();
        }

        public IRunningAverageAndStdDev Inverse()
        {
            return _Delegate;
        }

        IRunningAverage IRunningAverage.Inverse()
        {
            return Inverse();
        }
    }
}