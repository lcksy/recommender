using System;
using System.Linq;

using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// A simplified and streamlined version of BitSet
    /// </summary>
    [Serializable]
    public sealed class BitSet : ICloneable
    {
        private long[] bits;

        public BitSet(int numBits)
            : this((uint)numBits)
        {
        }

        public BitSet(uint numBits)
        {
            uint numlongs = numBits >> 6;
            if ((numBits & 0x3F) != 0)
            {
                numlongs++;
            }
            bits = new long[numlongs];
        }

        private BitSet(long[] bits)
        {
            this.bits = bits;
        }

        public bool Get(int index)
        {
            // skipping range check for speed
            return (bits[index >> 6] & 1L << (int)(index & 0x3F)) != 0L;
        }

        public void Set(int index)
        {
            // skipping range check for speed
            bits[index >> 6] |= 1L << (int)(index & 0x3F);
        }

        public void Clear(int index)
        {
            // skipping range check for speed
            bits[(uint)index >> 6] &= ~(1L << (index & 0x3F));
        }

        public void Clear()
        {
            int length = bits.Length;
            for (int i = 0; i < length; i++)
            {
                bits[i] = 0L;
            }
        }

        public object Clone()
        {
            return new BitSet((long[])bits.Clone());
        }

        public override int GetHashCode()
        {
            return Utils.GetArrayHashCode(bits);
        }

        public override bool Equals(Object o)
        {
            if (!(o is BitSet))
            {
                return false;
            }
            BitSet other = (BitSet)o;
            return Enumerable.SequenceEqual(bits, other.bits);
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder(64 * bits.Length);
            foreach (long l in bits)
            {
                for (int j = 0; j < 64; j++)
                {
                    result.Append((l & 1L << j) == 0 ? '0' : '1');
                }
                result.Append(' ');
            }
            return result.ToString();
        }
    }
}