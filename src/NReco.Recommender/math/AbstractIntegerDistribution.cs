using System;

using NReco.Math3.Exception;
using NReco.Math3.Random;
using NReco.Math3.Util;

namespace NReco.Math3.Distribution
{
    /// Base class for integer-valued discrete distributions.  Default
    /// implementations are provided for some of the methods that do not vary
    /// from distribution to distribution.
    ///
    /// @version $Id: AbstractIntegerDistribution.java 1547633 2013-12-03 23:03:06Z tn $
    public abstract class AbstractIntegerDistribution /*: IntegerDistribution */ 
    {
        /// RNG instance used to generate samples from the distribution.
        /// @since 3.1
        protected IRandomGenerator random;

        /// @param rng Random number generator.
        /// @since 3.1
        protected AbstractIntegerDistribution(IRandomGenerator rng)
        {
            random = rng;
        }

        public abstract double CumulativeProbability(int x);

        public abstract int GetSupportLowerBound();

        public abstract int GetSupportUpperBound();

        public abstract double GetNumericalMean();

        public abstract double GetNumericalVariance();

        public abstract double Probability(int x);

        /// {@inheritDoc}
        ///
        /// The default implementation uses the identity
        /// <p>{@code P(x0 < X <= x1) = P(X <= x1) - P(X <= x0)}</p>
        public double CumulativeProbability(int x0, int x1)
        {
            if (x1 < x0)
            {
                throw new ArgumentException(String.Format("LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT ({0}, {1})", x0, x1));
                //NumberIsTooLargeException(LocalizedFormats.LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT, x0, x1, true);
            }
            return CumulativeProbability(x1) - CumulativeProbability(x0);
        }

        /// {@inheritDoc}
        ///
        /// The default implementation returns
        /// <ul>
        /// <li>{@link #getSupportLowerBound()} for {@code p = 0},</li>
        /// <li>{@link #getSupportUpperBound()} for {@code p = 1}, and</li>
        /// <li>{@link #solveInverseCumulativeProbability(double, int, int)} for
        ///     {@code 0 < p < 1}.</li>
        /// </ul>
        public int InverseCumulativeProbability(double p)
        {
            if (p < 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException("p", p, "Should be in (0, 1)");

            int lower = GetSupportLowerBound();
            if (p == 0.0) return lower;

            if (lower == Int32.MinValue)
            {
                if (CheckedCumulativeProbability(lower) >= p)
                {
                    return lower;
                }
            }
            else
            {
                lower -= 1; // this ensures CumulativeProbability(lower) < p, which is important for the solving step
            }

            int upper = GetSupportUpperBound();
            if (p == 1.0) return upper;

            // use the one-sided Chebyshev inequality to narrow the bracket
            // cf. AbstractRealDistribution.inverseCumulativeProbability(double)
            double mu = GetNumericalMean();
            double sigma = Math.Sqrt(GetNumericalVariance());
            bool chebyshevApplies = !(Double.IsInfinity(mu) || Double.IsNaN(mu) || Double.IsInfinity(sigma) || Double.IsNaN(sigma) || sigma == 0.0);
            if (chebyshevApplies)
            {
                double k = Math.Sqrt((1.0 - p) / p);
                double tmp = mu - k * sigma;
                if (tmp > lower)
                {
                    lower = ((int)Math.Ceiling(tmp)) - 1;
                }
                k = 1.0 / k;
                tmp = mu + k * sigma;
                if (tmp < upper)
                {
                    upper = ((int)Math.Ceiling(tmp)) - 1;
                }
            }
            return SolveInverseCumulativeProbability(p, lower, upper);
        }

        /// This is a utility function used by {@link
        /// #inverseCumulativeProbability(double)}. It assumes {@code 0 < p < 1} and
        /// that the inverse cumulative probability lies in the bracket {@code
        /// (lower, upper]}. The implementation does simple bisection to find the
        /// smallest {@code p}-quantile <code>inf{x in Z | P(X<=x) >= p}</code>.
        ///
        /// @param p the cumulative probability
        /// @param lower a value satisfying {@code cumulativeProbability(lower) < p}
        /// @param upper a value satisfying {@code p <= cumulativeProbability(upper)}
        /// @return the smallest {@code p}-quantile of this distribution
        protected int SolveInverseCumulativeProbability(double p, int lower, int upper)
        {
            while (lower + 1 < upper)
            {
                int xm = (lower + upper) / 2;
                if (xm < lower || xm > upper)
                {
                    // Overflow.
                    // There will never be an overflow in both calculation methods
                    // for xm at the same time
                    xm = lower + (upper - lower) / 2;
                }

                double pm = CheckedCumulativeProbability(xm);
                if (pm >= p)
                {
                    upper = xm;
                }
                else
                {
                    lower = xm;
                }
            }
            return upper;
        }

        /// {@inheritDoc} */
        public void ReseedRandomGenerator(long seed)
        {
            random.SetSeed(seed);
        }

        /// {@inheritDoc}
        ///
        /// The default implementation uses the
        /// <a href="http://en.wikipedia.org/wiki/Inverse_transform_sampling">
        /// inversion method</a>.
        public int Sample()
        {
            return InverseCumulativeProbability(random.NextDouble());
        }

        /// {@inheritDoc}
        ///
        /// The default implementation generates the sample by calling
        /// {@link #sample()} in a loop.
        public int[] Sample(int length)
        {
            if (length <= 0)
                throw new NotStrictlyPositiveException(length);

            int[] outArr = new int[length];
            for (int i = 0; i < length; i++)
            {
                outArr[i] = Sample();
            }
            return outArr;
        }

        /// Computes the cumulative probability function and checks for {@code NaN}
        /// values returned. Throws {@code MathInternalError} if the value is
        /// {@code NaN}. Rethrows any exception encountered evaluating the cumulative
        /// probability function. Throws {@code MathInternalError} if the cumulative
        /// probability function returns {@code NaN}.
        ///
        /// @param argument input value
        /// @return the cumulative probability
        /// @{@code NaN}
        private double CheckedCumulativeProbability(int argument)
        {
            double result = CumulativeProbability(argument);
            if (Double.IsNaN(result))
                throw new System.Exception("DISCRETE_CUMULATIVE_PROBABILITY_RETURNED_NAN");

            return result;
        }

        /// For a random variable {@code X} whose values are distributed according to
        /// this distribution, this method returns {@code log(P(X = x))}, where
        /// {@code log} is the natural logarithm. In other words, this method
        /// represents the logarithm of the probability mass function (PMF) for the
        /// distribution. Note that due to the floating point precision and
        /// under/overflow issues, this method will for some distributions be more
        /// precise and faster than computing the logarithm of
        /// {@link #probability(int)}.
        /// <p>
        /// The default implementation simply computes the logarithm of {@code probability(x)}.</p>
        ///
        /// @param x the point at which the PMF is evaluated
        /// @return the logarithm of the value of the probability mass function at {@code x}
        public virtual double LogProbability(int x)
        {
            return MathUtil.Log(Probability(x));
        }
    }
}