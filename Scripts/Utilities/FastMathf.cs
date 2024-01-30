using System;
using UnityEngine;

namespace Utilities
{
    public static class FastMathf
    {
        // Uses bitwise operations for fast power estimation, but at the cost of some error.
        // This can produce a minor stipling effect when rotating color hues but the result is usually quite close to that produced using the built-in power calculation method.
        public static float Pow(float x, float power)
        {
            int tmp = (int)(BitConverter.DoubleToInt64Bits(x) >> 32);
            int tmp2 = (int)(power * (tmp - 1072632447) + 1072632447);

            return Convert.ToSingle(BitConverter.Int64BitsToDouble(((long)tmp2) << 32));
        }
    }
}
