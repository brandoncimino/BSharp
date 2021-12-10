using System;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        public static sbyte  Abs(this sbyte  value) => Math.Abs(value);
        public static short  Abs(this short  value) => Math.Abs(value);
        public static int    Abs(this int    value) => Math.Abs(value);
        public static long   Abs(this long   value) => Math.Abs(value);
        public static double Abs(this double value) => Math.Abs(value);
        public static float  Abs(this float  value) => Math.Abs(value);
    }
}