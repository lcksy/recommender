using System;
using System.Collections;
using System.Collections.Generic;

using NReco.Math3.Distribution;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// Wraps a <see cref="IEnumerator<long>"/> and returns only some subset of the elements that it would,
    /// as determined by a sampling rate parameter.
    /// </summary>
    public sealed class SamplinglongPrimitiveIterator : IEnumerator<long>
    {
        private PascalDistribution geometricDistribution;
        private IEnumerator<long> enumerator;

        public SamplinglongPrimitiveIterator(IEnumerator<long> enumerator, double samplingRate)
            : this(RandomUtils.getRandom(), enumerator, samplingRate)
        {

        }

        public SamplinglongPrimitiveIterator(RandomWrapper random, IEnumerator<long> enumerator, double samplingRate)
        {
            if (enumerator == null)
                throw new ArgumentException("enumerator");
            if (!(samplingRate > 0.0 && samplingRate <= 1.0))
                throw new ArgumentException("samplingRate");
            //Preconditions.checkArgument(samplingRate > 0.0 && samplingRate <= 1.0, "Must be: 0.0 < samplingRate <= 1.0");
            // Geometric distribution is special case of negative binomial (aka Pascal) with r=1:
            geometricDistribution = new PascalDistribution(random.getRandomGenerator(), 1, samplingRate);
            this.enumerator = enumerator;

            SkipNext();
        }

        public void Remove()
        {
            throw new NotSupportedException();
        }

        public void Skip(int n)
        {
            int toSkip = 0;
            for (int i = 0; i < n; i++)
            {
                toSkip += geometricDistribution.Sample();
            }

            for (int i = 0; i < toSkip; i++)
            {
                if (!enumerator.MoveNext())
                    break;
            }
        }

        public static IEnumerator<long> MaybeWrapIterator(IEnumerator<long> enumerator, double samplingRate)
        {
            return samplingRate >= 1.0 ? enumerator : new SamplinglongPrimitiveIterator(enumerator, samplingRate);
        }

        public long Current
        {
            get { return enumerator.Current; }
        }

        public void Dispose()
        {

        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        protected void SkipNext()
        {
            int toSkip = geometricDistribution.Sample();

            //_Delegate.skip(toSkip);
            for (int i = 0; i < toSkip; i++)
            {
                if (!enumerator.MoveNext())
                    break;
            }
        }

        public bool MoveNext()
        {
            SkipNext();
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }
}