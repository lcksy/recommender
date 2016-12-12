using System.Threading;

namespace NReco.CF.Taste.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AtomicInteger
    {
        private int value;

        /// <summary>
        /// 
        /// </summary>
        public AtomicInteger()
        {
            value = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public AtomicInteger(int val)
        {
            value = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Get()
        {
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int IncrementAndGet()
        {
            //lock (this)
            //{
            //    return ++Value;
            //}
            return Interlocked.Increment(ref value);
        }
    }
}