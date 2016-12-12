using System;

using NReco.Math3.Exception;
using NReco.Math3.Util;

namespace NReco.Math3.Random
{
    /// Base class for random number generators that generates bits streams.
    ///
    /// @version $Id: BitsStreamGenerator.java 1538368 2013-11-03 13:57:37Z erans $
    /// @since 2.0
    public abstract class BitsStreamGenerator : IRandomGenerator
    {
        // Serializable version identifier */
        //private static long SERIALVERSIONUID = 20130104L;
        // Next gaussian. */
        private double _nextGaussian;

        /// Creates a new random number generator.
        public BitsStreamGenerator()
        {
            _nextGaussian = Double.NaN;
        }

        /// {@inheritDoc} */
        public abstract void SetSeed(int seed);

        /// {@inheritDoc} */
        public abstract void SetSeed(int[] seed);

        /// {@inheritDoc} */
        public abstract void SetSeed(long seed);

        /// Generate next pseudorandom number.
        /// <p>This method is the core generation algorithm. It is used by all the
        /// public generation methods for the various primitive types {@link
        /// #nextBoolean()}, {@link #nextBytes(byte[])}, {@link #nextDouble()},
        /// {@link #nextFloat()}, {@link #nextGaussian()}, {@link #nextInt()},
        /// {@link #next(int)} and {@link #nextlong()}.</p>
        /// @param bits number of random bits to produce
        /// @return random bits generated
        protected abstract int Next(int bits);

        /// {@inheritDoc} */
        public bool NextBoolean()
        {
            return Next(1) != 0;
        }

        /// {@inheritDoc} */
        public void NextBytes(byte[] bytes)
        {
            int i = 0;
            int iEnd = bytes.Length - 3;
            int random;
            while (i < iEnd)
            {
                random = Next(32);
                bytes[i] = (byte)(random & 0xff);
                bytes[i + 1] = (byte)((random >> 8) & 0xff);
                bytes[i + 2] = (byte)((random >> 16) & 0xff);
                bytes[i + 3] = (byte)((random >> 24) & 0xff);
                i += 4;
            }
            random = Next(32);
            while (i < bytes.Length)
            {
                bytes[i++] = (byte)(random & 0xff);
                random >>= 8;
            }
        }

        /// {@inheritDoc} */
        static readonly double minNonZeroDouble = Math.Pow(2, -52);

        public double NextDouble()
        {
            long high = ((long)Next(26)) << 26;
            long low = Next(26);
            return (high | low) * minNonZeroDouble; // * 0x1.0p-52d
        }

        /// {@inheritDoc} */
        public float NextFloat()
        {
            return Next(23) * 1E-23f;
        }

        /// {@inheritDoc} */
        public double NextGaussian()
        {
            double random;
            if (Double.IsNaN(_nextGaussian))
            {
                // generate a new pair of gaussian numbers
                double x = NextDouble();
                double y = NextDouble();
                double alpha = 2 * Math.PI * x;
                double r = Math.Sqrt(-2 * MathUtil.Log(y));
                random = r * Math.Cos(alpha);
                _nextGaussian = r * Math.Sin(alpha);
            }
            else
            {
                // use the second element of the pair already generated
                random = _nextGaussian;
                _nextGaussian = Double.NaN;
            }

            return random;
        }

        /// {@inheritDoc} */
        public int NextInt()
        {
            return Next(32);
        }

        /// {@inheritDoc}
        /// <p>This default implementation is copied from Apache Harmony
        /// java.util.Random (r929253).</p>
        ///
        /// <p>Implementation notes: <ul>
        /// <li>If n is a power of 2, this method returns
        /// {@code (int) ((n * (long) next(31)) >> 31)}.</li>
        ///
        /// <li>If n is not a power of 2, what is returned is {@code next(31) % n}
        /// with {@code next(31)} values rejected (i.e. regenerated) until a
        /// value that is larger than the remainder of {@code Integer.MAX_VALUE / n}
        /// is generated. Rejection of this initial segment is necessary to ensure
        /// a uniform distribution.</li></ul></p>
        public int NextInt(int n)
        {
            if (n > 0)
            {
                if ((n & -n) == n)
                {
                    return (int)((n * (long)Next(31)) >> 31);
                }
                int bits;
                int val;
                do
                {
                    bits = Next(31);
                    val = bits % n;
                } while (bits - val + (n - 1) < 0);
                return val;
            }
            throw new NotStrictlyPositiveException(n);
        }

        /// {@inheritDoc} */
        public long Nextlong()
        {
            long high = ((long)Next(32)) << 32;
            long low = ((long)Next(32)) & 0xffffffffL;
            return high | low;
        }

        /// Returns a pseudorandom, uniformly distributed <tt>long</tt> value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.
        ///
        /// @param n the bound on the random number to be returned.  Must be
        /// positive.
        /// @return  a pseudorandom, uniformly distributed <tt>long</tt>
        /// value between 0 (inclusive) and n (exclusive).
        /// @throws IllegalArgumentException  if n is not positive.
        public long Nextlong(long n)
        {
            if (n > 0)
            {
                long bits;
                long val;
                do
                {
                    bits = ((long)Next(31)) << 32;
                    bits |= ((long)Next(32)) & 0xffffffffL;
                    val = bits % n;
                } while (bits - val + (n - 1) < 0);
                return val;
            }
            throw new NotStrictlyPositiveException(n);
        }

        /// Clears the cache used by the default implementation of
        /// {@link #nextGaussian}.
        public void Clear()
        {
            _nextGaussian = Double.NaN;
        }
    }
}