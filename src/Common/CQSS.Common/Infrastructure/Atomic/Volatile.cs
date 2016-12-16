using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CQSS.Common.Infrastructure.Atomic
{
    public static class Volatile
    {
        public struct Boolean
        {
            private int _value;
            private const int False = 0;
            private const int True = 1;

            public Boolean(bool value)
            {
                _value = value ? True : False;
            }

            public bool ReadUnfenced()
            {
                return ToBool(_value);
            }

            public bool ReadAcquireFence()
            {
                var value = ToBool(_value);
                Thread.MemoryBarrier();
                return value;
            }

            public bool ReadFullFence()
            {
                var value = ToBool(_value);
                Thread.MemoryBarrier();
                return value;
            }

            [MethodImpl(MethodImplOptions.NoOptimization)]
            public bool ReadCompilerOnlyFence()
            {
                return ToBool(_value);
            }

            public void WriteReleaseFence(bool newValue)
            {
                var newValueInt = ToInt(newValue);
                Thread.MemoryBarrier();
                _value = newValueInt;
            }
            public void WriteFullFence(bool newValue)
            {
                var newValueInt = ToInt(newValue);
                Thread.MemoryBarrier();
                _value = newValueInt;
            }

            [MethodImpl(MethodImplOptions.NoOptimization)]
            public void WriteCompilerOnlyFence(bool newValue)
            {
                _value = ToInt(newValue);
            }

            public void WriteUnfenced(bool newValue)
            {
                _value = ToInt(newValue);
            }

            public bool AtomicCompareExchange(bool newValue, bool comparand)
            {
                var newValueInt = ToInt(newValue);
                var comparandInt = ToInt(comparand);

                return Interlocked.CompareExchange(ref _value, newValueInt, comparandInt) == comparandInt;
            }

            public bool AtomicExchange(bool newValue)
            {
                var newValueInt = ToInt(newValue);
                var originalValue = Interlocked.Exchange(ref _value, newValueInt);
                return ToBool(originalValue);
            }

            public override string ToString()
            {
                var value = ReadFullFence();
                return value.ToString();
            }

            private static bool ToBool(int value)
            {
                if (value != False && value != True)
                    throw new ArgumentOutOfRangeException("value");

                return value == True;
            }

            private static int ToInt(bool value)
            {
                return value ? True : False;
            }
        }

        public struct Integer
        {
            private int _value;

            public Integer(int value)
            {
                _value = value;
            }

            public int ReadUnfenced()
            {
                return _value;
            }

            public int ReadAcquireFence()
            {
                var value = _value;
                Thread.MemoryBarrier();
                return value;
            }

            public int ReadFullFence()
            {
                var value = _value;
                Thread.MemoryBarrier();
                return value;
            }

            [MethodImpl(MethodImplOptions.NoOptimization)]
            public int ReadCompilerOnlyFence()
            {
                return _value;
            }

            public void WriteReleaseFence(int newValue)
            {
                _value = newValue;
                Thread.MemoryBarrier();
            }

            public void WriteFullFence(int newValue)
            {
                _value = newValue;
                Thread.MemoryBarrier();
            }

            [MethodImpl(MethodImplOptions.NoOptimization)]
            public void WriteCompilerOnlyFence(int newValue)
            {
                _value = newValue;
            }

            public void WriteUnfenced(int newValue)
            {
                _value = newValue;
            }

            public bool AtomicCompareExchange(int newValue, int comparand)
            {
                return Interlocked.CompareExchange(ref _value, newValue, comparand) == comparand;
            }

            public int AtomicExchange(int newValue)
            {
                return Interlocked.Exchange(ref _value, newValue);
            }

            public int AtomicAddAndGet(int delta)
            {
                return Interlocked.Add(ref _value, delta);
            }

            public int AtomicIncrementAndGet()
            {
                return Interlocked.Increment(ref _value);
            }

            public int AtomicDecrementAndGet()
            {
                return Interlocked.Decrement(ref _value);
            }

            public override string ToString()
            {
                var value = ReadFullFence();
                return value.ToString();
            }
        }
    }
}