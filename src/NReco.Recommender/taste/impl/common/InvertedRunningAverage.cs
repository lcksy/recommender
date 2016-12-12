using System;

namespace NReco.CF.Taste.Impl.Common
{
    public sealed class InvertedRunningAverage : IRunningAverage
    {
        private IRunningAverage _Delegate;

        public InvertedRunningAverage(IRunningAverage deleg)
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

        public IRunningAverage Inverse()
        {
            return _Delegate;
        }
    }
}