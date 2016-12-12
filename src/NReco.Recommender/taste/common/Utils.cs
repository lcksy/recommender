using System;

namespace NReco.CF.Taste.Common
{
    public static class Utils
    {
        public static int GetArrayHashCode(Array arr)
        {
            int arrHash = arr.Length;
            for (int i = 0; i < arr.Length; ++i)
            {
                arrHash = arrHash ^ arr.GetValue(i).GetHashCode();
            }
            return arrHash;
        }

        public static int GetArrayDeepHashCode(Array arr)
        {
            int arrHash = arr.Length;
            for (int i = 0; i < arr.Length; ++i)
            {
                var val = arr.GetValue(i);
                var valHashCode = val is Array ? GetArrayDeepHashCode((Array)val) : val.GetHashCode();
                arrHash = arrHash ^ valHashCode;
            }
            return arrHash;
        }

        public static bool ArrayDeepEquals(Array arr1, Array arr2)
        {
            if (arr1.Length != arr2.Length || arr1.GetType() != arr2.GetType())
                return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                var v1 = arr1.GetValue(i);
                var v2 = arr2.GetValue(i);
                if (v1 is Array && v2 is Array)
                { 
                    if (!ArrayDeepEquals((Array)v1, (Array)v2))
                        return false;
                    else
                        continue;
                }

                if (v1 == null && v2 == null)
                    continue;

                if (v1 != null)
                { 
                    if (!v1.Equals(v2))
                        return false;
                }

                if (v2 != null)
                { 
                    if (!v2.Equals(v1))
                        return false;
                }
            }
            return true;
        }
    }
}