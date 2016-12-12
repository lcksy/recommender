using System;

using System.Collections.Generic;

namespace NReco.Math3.Primes
{
    /// Methods related to prime numbers in the range of <code>int</code>:
    /// <ul>
    /// <li>primality test</li>
    /// <li>prime number generation</li>
    /// <li>factorization</li>
    /// </ul>
    ///
    /// @version $Id: Primes.java 1538368 2013-11-03 13:57:37Z erans $
    /// @since 3.2
    public class Primes
    {
        /// Hide utility class.
        private Primes()
        {
        }

        /// Primality test: tells if the argument is a (provable) prime or not.
        /// <p>
        /// It uses the Miller-Rabin probabilistic test in such a way that a result is guaranteed:
        /// it uses the firsts prime numbers as successive base (see Handbook of applied cryptography
        /// by Menezes, table 4.1).
        ///
        /// @param n number to test.
        /// @return true if n is prime. (All numbers &lt; 2 return false).
        public static bool IsPrime(int n)
        {
            if (n < 2)
            {
                return false;
            }

            foreach (int p in SmallPrimes.PRIMES)
            {
                if (0 == (n % p))
                {
                    return n == p;
                }
            }
            return SmallPrimes.millerRabinPrimeTest(n);
        }

        /// Return the smallest prime greater than or equal to n.
        ///
        /// @param n a positive number.
        /// @return the smallest prime greater than or equal to n.
        /// @; 0.
        public static int NextPrime(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException();// MathIllegalArgumentException(LocalizedFormats.NUMBER_TOO_SMALL, n, 0);
            }
            if (n == 2)
            {
                return 2;
            }
            n |= 1;//make sure n is odd
            if (n == 1)
            {
                return 2;
            }

            if (IsPrime(n))
            {
                return n;
            }

            // prepare entry in the +2, +4 loop:
            // n should not be a multiple of 3
            int rem = n % 3;
            if (0 == rem)
            { // if n % 3 == 0
                n += 2; // n % 3 == 2
            }
            else if (1 == rem)
            { // if n % 3 == 1
                // if (isPrime(n)) return n;
                n += 4; // n % 3 == 2
            }
            while (true)
            { // this loop skips all multiple of 3
                if (IsPrime(n))
                {
                    return n;
                }
                n += 2; // n % 3 == 1
                if (IsPrime(n))
                {
                    return n;
                }
                n += 4; // n % 3 == 2
            }
        }

        /// Prime factors decomposition
        ///
        /// @param n number to factorize: must be &ge; 2
        /// @return list of prime factors of n
        /// @; 2.
        public static List<int> PrimeFactors(int n)
        {
            if (n < 2)
            {
                throw new ArgumentException(); // MathIllegalArgumentException(LocalizedFormats.NUMBER_TOO_SMALL, n, 2);
            }
            // slower than trial div unless we do an awful lot of computation
            // (then it finally gets JIT-compiled efficiently
            // List<Integer> out = PollardRho.primeFactors(n);
            return SmallPrimes.trialDivision(n);
        }
    }
}