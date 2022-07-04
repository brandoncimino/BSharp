using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections {
    [PublicAPI]
    public static class TupleExtensions {
        #region ToArray

        [Pure] public static T[] ToArray<T>(this (T, T)                                              tuple) => new[] { tuple.Item1, tuple.Item2 };
        [Pure] public static T[] ToArray<T>(this (T, T, T)                                           tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T)                                        tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T)                                     tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T)                                  tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T)                               tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T)                            tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T)                         tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T)                      tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T)                   tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T)                tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T)             tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T)          tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)       tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)    tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };
        [Pure] public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };

        [Pure] public static object?[] ToObjArray<T, T2>(this                                                                     (T, T2)                                                                     tuple) => new object?[] { tuple.Item1, tuple.Item2 };
        [Pure] public static object?[] ToObjArray<T, T2, T3>(this                                                                 (T, T2, T3)                                                                 tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4>(this                                                             (T, T2, T3, T4)                                                             tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5>(this                                                         (T, T2, T3, T4, T5)                                                         tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6>(this                                                     (T, T2, T3, T4, T5, T6)                                                     tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7>(this                                                 (T, T2, T3, T4, T5, T6, T7)                                                 tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8>(this                                             (T, T2, T3, T4, T5, T6, T7, T8)                                             tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9>(this                                         (T, T2, T3, T4, T5, T6, T7, T8, T9)                                         tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this                                    (T, T2, T3, T4, T5, T6, T7, T8, T9, T10)                                    tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this                               (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)                               tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this                          (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)                          tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this                     (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)                     tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this                (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)                tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this           (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)           tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this      (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16)      tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };
        [Pure] public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17) tuple) => new object?[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };

        #endregion

        #region Sort

        public static (T min, T max) Sort<T>(this (T a, T b) tuple2) {
            var ls = tuple2.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1]);
        }

        public static (T min, T, T max) Sort<T>(this (T, T, T) tuple3) {
            var ls = tuple3.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1], ls[2]);
        }

        public static (T min, T, T, T max) Sort<T>(this (T, T, T, T) tuple4) {
            var ls = tuple4.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1], ls[2], ls[3]);
        }

        public static (T min, T, T, T, T max) Sort<T>(this (T, T, T, T, T) tuple5) {
            var ls = tuple5.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1], ls[2], ls[3], ls[4]);
        }

        public static (T min, T, T, T, T, T max) Sort<T>(this (T, T, T, T, T, T) tuple6) {
            var ls = tuple6.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1], ls[2], ls[3], ls[4], ls[5]);
        }

        public static (T min, T, T, T, T, T, T max) Sort<T>(this (T, T, T, T, T, T, T) tuple7) {
            var ls = tuple7.ToArray();
            Array.Sort(ls);
            return (ls[0], ls[1], ls[2], ls[3], ls[4], ls[5], ls[6]);
        }

        #endregion

        #region "Spread Operators"

        #region Item1

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T?> Item1<T, T2>(this IEnumerable<(T, T2)> tuples) => tuples.Select(it => it.Item1);

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T?> Item1<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) => tuples.Select(it => it.Item1);

        #endregion

        #region Item2

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T2?> Item2<T, T2>(this IEnumerable<(T, T2)> tuples) => tuples.Select(it => it.Item2);

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T2?> Item2<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) => tuples.Select(it => it.Item2);

        #endregion

        #region Item3

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T3?> Item3<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) => tuples.Select(it => it.Item3);

        #endregion

        #endregion

        #region Math ⚠: These methods have been disabled beyond Tuple2s because they slowed down IntelliJ's autocompletion a _lot_

        #region Sum

        #region Sum<int>

        [Pure] public static int Sum(this (int, int) tuple) => tuple.ToArray().Sum();

        // [Pure] public static int Sum(this (int, int, int)                                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int)                                                                  tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int)                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int)                                                        tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int)                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int)                                              tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int)                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int)                                    tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int)                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int)                          tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int, int)                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int, int, int)                tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int)           tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int)      tuple) => tuple.ToArray().Sum();
        // [Pure] public static int Sum(this (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<int?>

        [Pure] public static int? Sum(this (int?, int?) tuple) => tuple.ToArray().Sum();

        // [Pure] public static int? Sum(this (int?, int?, int?)                                                                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?)                                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?)                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?)                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?)                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?)                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)             tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?)       tuple) => tuple.ToArray().Sum();
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?) tuple) => tuple.ToArray().Sum();
        //
        // [Pure] public static int? Sum(this (int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?, int?) tuple) => tuple.ToArray().Sum();

        #endregion

        #region Sum<long>

        // [Pure] public static long Sum(this (long, long)                                                                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long)                                                                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long)                                                                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long)                                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long)                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long)                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long)                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long)                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long)                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long)                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long, long)                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long, long, long)                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long, long, long, long)             tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long, long, long, long, long)       tuple) => tuple.ToArray().Sum();
        // [Pure] public static long Sum(this (long, long, long, long, long, long, long, long, long, long, long, long, long, long, long, long, long) tuple) => tuple.ToArray().Sum();

        #endregion

        #region Sum<long?>

        [Pure] public static long? Sum(this (long?, long?) tuple) => tuple.ToArray().Sum();

        // [Pure] public static long? Sum(this (long?, long?, long?)                                                                                            tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?)                                                                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?)                                                                              tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?)                                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?)                                                                tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?)                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?)                                                  tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)                                    tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)                      tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)               tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?)        tuple) => tuple.ToArray().Sum();
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?) tuple) => tuple.ToArray().Sum();
        //
        // [Pure] public static long? Sum(this (long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?, long?) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<float>

        [Pure] public static float Sum(this (float, float) tuple) => tuple.ToArray().Sum();

        // [Pure] public static float Sum(this (float, float, float)                                                                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float)                                                                                            tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float)                                                                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float)                                                                              tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float)                                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float)                                                                tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float)                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float)                                                  tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float)                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float)                                    tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float, float)                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float, float, float)                      tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float, float, float, float)               tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float)        tuple) => tuple.ToArray().Sum();
        // [Pure] public static float Sum(this (float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<float?>

        [Pure] public static float? Sum(this (float?, float?) tuple) => tuple.ToArray().Sum();

        // [Pure] public static float? Sum(this (float?, float?, float?)                                                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?)                                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?)                                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?)                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?)                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?)                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?)         tuple) => tuple.ToArray().Sum();
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?) tuple) => tuple.ToArray().Sum();
        //
        // [Pure] public static float? Sum(this (float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?, float?) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<double>

        [Pure] public static double Sum(this (double, double) tuple) => tuple.ToArray().Sum();

        // [Pure] public static double Sum(this (double, double, double)                                                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double)                                                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double)                                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double)                                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double)                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double)                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double)                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double)                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double)                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double, double)                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double, double, double)                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double, double, double, double)                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double)         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double Sum(this (double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<double?>

        [Pure] public static double? Sum(this (double?, double?) tuple) => tuple.ToArray().Sum();

        // [Pure] public static double? Sum(this (double?, double?, double?)                                                                                                                      tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?)                                                                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?)                                                                                                    tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?)                                                                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?)                                                                                  tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?)                                                                tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)                                              tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)                            tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?)          tuple) => tuple.ToArray().Sum();
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?) tuple) => tuple.ToArray().Sum();
        //
        // [Pure] public static double? Sum(this (double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?, double?) tuple) => tuple.ToArray().Sum();
        //

        #endregion

        #region Sum<decimal>

        [Pure] public static decimal Sum(this (decimal, decimal) tuple) => tuple.ToArray().Sum();

        // [Pure] public static decimal Sum(this (decimal, decimal, decimal)                                                                                                                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal)                                                                                                                      tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal)                                                                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal)                                                                                                    tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                                                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                                                                  tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                                                tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                              tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                            tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)          tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal Sum(this (decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal) tuple) => tuple.ToArray().Sum();
        //
        // #endregion
        //

        #region Sum<decimal?>

        [Pure] public static decimal? Sum(this (decimal?, decimal?) tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?)                                                                                                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?)                                                                                                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?)                                                                                                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                                                                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                                                           tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                                                 tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                                       tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                             tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                                   tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                                         tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                               tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)                     tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?)           tuple) => tuple.ToArray().Sum();
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?) tuple) => tuple.ToArray().Sum();
        //
        // [Pure] public static decimal? Sum(this (decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?, decimal?) tuple) => tuple.ToArray().Sum();

        #endregion

        #endregion

        #endregion

        #region Difference

        /// <param name="tuple">a <see cref="(T1, T2)"/></param>
        /// <returns><see cref="(T1, T2).Item2"/> - <see cref="ValueTuple{T1,T2}.Item1"/></returns>
        public static int Diff(this (int from, int to) tuple) => tuple.to - tuple.from;

        #endregion

        #region Max

        /// <summary>
        /// Returns that <b>maximum</b> values of each <see cref="Tuple{T1,T2}"/> member individually.
        /// </summary>
        /// <param name="tuples">a collection of <see cref="Tuple{T1,T2}"/>s where both items are <see cref="IComparable{T}"/></param>
        /// <typeparam name="T1">the <see cref="Type"/> of <see cref="Tuple{T1,T2}.Item1"/></typeparam>
        /// <typeparam name="T2">the <see cref="Type"/> of <see cref="Tuple{T1,T2}.Item2"/></typeparam>
        /// <returns>a <see cref="Tuple{T1,T2}"/> containing the <b>maximum</b> <typeparamref name="T1"/> and <typeparamref name="T2"/> values</returns>
        public static (T1, T2) MaxItems<T1, T2>(this IEnumerable<(T1, T2)> tuples)
            where T1 : IComparable<T1>
            where T2 : IComparable<T2> {
            return tuples.Aggregate(
                (a, b) => (
                              a.Item1.Max(b.Item1),
                              a.Item2.Max(b.Item2))
            );
        }

        #endregion

        #region Min

        /// <summary>
        /// Returns that <b>minimum</b> values of each <see cref="Tuple{T1,T2}"/> member individually.
        /// </summary>
        /// <param name="tuples">a collection of <see cref="Tuple{T1,T2}"/>s where both items are <see cref="IComparable{T}"/></param>
        /// <typeparam name="T1">the <see cref="Type"/> of <see cref="Tuple{T1,T2}.Item1"/></typeparam>
        /// <typeparam name="T2">the <see cref="Type"/> of <see cref="Tuple{T1,T2}.Item2"/></typeparam>
        /// <returns>a <see cref="Tuple{T1,T2}"/> containing the <b>minimum</b> <typeparamref name="T1"/> and <typeparamref name="T2"/> values</returns>
        public static (T1, T2) MinItems<T1, T2>(this IEnumerable<(T1, T2)> tuples)
            where T1 : IComparable<T1>
            where T2 : IComparable<T2> {
            return tuples.Aggregate(
                (a, b) => (
                              a.Item1.Max(b.Item1),
                              a.Item2.Max(b.Item2)
                          )
            );
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Applies a <see cref="Func{T,TResult}"/> to each item in this <see cref="Tuple{T1, T2}"/>.
        /// </summary>
        /// <remarks>Requires a homogenous tuple.</remarks>
        /// <param name="tuple">the original <see cref="Tuple{T1,T2}"/></param>
        /// <param name="selector">the transformation <see cref="Func{T,TResult}"/></param>
        /// <typeparam name="T">the input type</typeparam>
        /// <typeparam name="TOut">the output type</typeparam>
        /// <returns>a new <see cref="Tuple"/> of <typeparamref name="TOut"/> values</returns>
        public static (TOut, TOut) Select<T, TOut>(this (T, T) tuple, Func<T, TOut> selector) {
            return (
                       selector(tuple.Item1),
                       selector(tuple.Item2)
                   );
        }

        public static (TOut, TOut, TOut) Select<T, TOut>(this (T, T, T) tuple, Func<T, TOut> selector) {
            return (
                       selector(tuple.Item1),
                       selector(tuple.Item2),
                       selector(tuple.Item3)
                   );
        }

        public static (TOut, TOut, TOut, TOut) Select<T, TOut>(this (T, T, T, T) tuple, Func<T, TOut> selector) {
            return (
                       selector(tuple.Item1),
                       selector(tuple.Item2),
                       selector(tuple.Item3),
                       selector(tuple.Item4)
                   );
        }

        #endregion
    }
}