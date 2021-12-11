using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections {
    [PublicAPI]
    public static class TupleExtensions {
        #region ToArray

        [Pure]
        public static T?[] ToArray<T>(this (T, T) tuple) => new[] { tuple.Item1, tuple.Item2 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };

        [Pure]
        public static T?[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };


        [Pure]
        public static object?[] ToObjArray<T, T2>(this (T, T2) tuple) => new object[] { tuple.Item1, tuple.Item2 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3>(this (T, T2, T3) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4>(this (T, T2, T3, T4) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5>(this (T, T2, T3, T4, T5) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6>(this (T, T2, T3, T4, T5, T6) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7>(this (T, T2, T3, T4, T5, T6, T7) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8>(this (T, T2, T3, T4, T5, T6, T7, T8) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9>(this (T, T2, T3, T4, T5, T6, T7, T8, T9) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };

        [Pure]
        public static object?[] ToObjArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };

        #endregion

        #region Sort

        public static (T min, T max) Sort<T>(this (T a, T b) tuple2) where T : IComparable<T> {
            var ls = tuple2.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1]);
        }

        public static (T, T, T) Sort<T>(this (T, T, T) tuple3) where T : IComparable<T> {
            var ls = tuple3.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1], ls[2]);
        }

        public static (T, T, T, T) Sort<T>(this (T, T, T, T) tuple4) where T : IComparable<T> {
            var ls = tuple4.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1], ls[2], ls[3]);
        }

        public static (T, T, T, T, T) Sort<T>(this (T, T, T, T, T) tuple5) where T : IComparable<T> {
            var ls = tuple5.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1], ls[2], ls[3], ls[4]);
        }

        public static (T, T, T, T, T, T) Sort<T>(this (T, T, T, T, T, T) tuple6) where T : IComparable<T> {
            var ls = tuple6.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1], ls[2], ls[3], ls[4], ls[5]);
        }

        public static (T, T, T, T, T, T, T) Sort<T>(this (T, T, T, T, T, T, T) tuple7) where T : IComparable<T> {
            var ls = tuple7.ToArray().ToList();
            ls.Sort();
            return (ls[0], ls[1], ls[2], ls[3], ls[4], ls[5], ls[6]);
        }

        #endregion

        #region "Spread Operators"

        #region Item1

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T?> Item1<T, T2>(this IEnumerable<(T, T2)> tuples) {
            return tuples.Select(it => it.Item1);
        }


        [Pure]
        [LinqTunnel]
        public static IEnumerable<T?> Item1<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) {
            return tuples.Select(it => it.Item1);
        }

        #endregion

        #region Item2

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T2?> Item2<T, T2>(this IEnumerable<(T, T2)> tuples) {
            return tuples.Select(it => it.Item2);
        }


        [Pure]
        [LinqTunnel]
        public static IEnumerable<T2?> Item2<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) {
            return tuples.Select(it => it.Item2);
        }

        #endregion

        #region Item3

        [Pure]
        [LinqTunnel]
        public static IEnumerable<T3?> Item3<T, T2, T3>(this IEnumerable<(T, T2, T3)> tuples) {
            return tuples.Select(it => it.Item3);
        }

        #endregion

        #endregion

        #region Sum ⚠: These methods have been disabled because they slowed down IntelliJ's autocompletion a _lot_

        //
        // #region Sum<int>
        //
        // [Pure] public static int Sum(this (int, int)                                                                            tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<int?>
        //
        // [Pure] public static int? Sum(this (int?, int?)                                                                                     tuple) => tuple.ToArray().Sum();
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
        //
        // #endregion
        //
        // #region Sum<long>
        //
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
        //
        // #endregion
        //
        // #region Sum<long?>
        //
        // [Pure] public static long? Sum(this (long?, long?)                                                                                                   tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<float>
        //
        // [Pure] public static float Sum(this (float, float)                                                                                                          tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<float?>
        //
        // [Pure] public static float? Sum(this (float?, float?)                                                                                                                 tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<double>
        //
        // [Pure] public static double Sum(this (double, double)                                                                                                                         tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<double?>
        //
        // [Pure] public static double? Sum(this (double?, double?)                                                                                                                               tuple) => tuple.ToArray().Sum();
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
        // #endregion
        //
        // #region Sum<decimal>
        //
        // [Pure] public static decimal Sum(this (decimal, decimal)                                                                                                                                        tuple) => tuple.ToArray().Sum();
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
        // #region Sum<decimal?>
        //
        // [Pure] public static decimal? Sum(this (decimal?, decimal?)                                                                                                                                             tuple) => tuple.ToArray().Sum();
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
        //
        // #endregion
        //

        #endregion
    }
}