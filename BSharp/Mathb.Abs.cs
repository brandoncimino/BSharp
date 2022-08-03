using System;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        #region Abs

        [Pure] public static sbyte   Abs(this sbyte   value) => Math.Abs(value);
        [Pure] public static short   Abs(this short   value) => Math.Abs(value);
        [Pure] public static int     Abs(this int     value) => Math.Abs(value);
        [Pure] public static long    Abs(this long    value) => Math.Abs(value);
        [Pure] public static double  Abs(this double  value) => Math.Abs(value);
        [Pure] public static float   Abs(this float   value) => Math.Abs(value);
        [Pure] public static decimal Abs(this decimal value) => Math.Abs(value);

        #endregion

        #region Dist

        [Pure] public static int     Dist(this int     start, int     end) => start > end ? start - end : end - start;
        [Pure] public static long    Dist(this long    start, long    end) => start > end ? start - end : end - start;
        [Pure] public static uint    Dist(this uint    start, uint    end) => start > end ? start - end : end - start;
        [Pure] public static ulong   Dist(this ulong   start, ulong   end) => start > end ? start - end : end - start;
        [Pure] public static float   Dist(this float   start, float   end) => start > end ? start - end : end - start;
        [Pure] public static double  Dist(this double  start, double  end) => start > end ? start - end : end - start;
        [Pure] public static decimal Dist(this decimal start, decimal end) => start > end ? start - end : end - start;

        #endregion
    }
}