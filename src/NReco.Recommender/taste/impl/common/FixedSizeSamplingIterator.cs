using System.Collections;
using System.Collections.Generic;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// Wraps a <see cref="IEnumerator<T>"/> and returns only some subset of the elements that it would,
    /// as determined by a sampling rate parameter.
    /// </summary>
    public sealed class FixedSizeSamplingIterator<T> : IEnumerator<T>
    {
        List<T> buf;
        IEnumerator<T> enumerator;

        public FixedSizeSamplingIterator(int size, IEnumerator<T> source)
        {
            buf = new List<T>(size);

            int sofar = 0;
            var random = RandomUtils.getRandom();
            while (source.MoveNext())
            {
                T v = source.Current;
                sofar++;
                if (buf.Count < size)
                {
                    buf.Add(v);
                }
                else
                {
                    int position = random.nextInt(sofar);
                    if (position < buf.Count)
                    {
                        buf[position] = v;
                    }
                }
            }
            enumerator = buf.GetEnumerator();
        }

        public T Current
        {
            get { return enumerator.Current; }
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }
}