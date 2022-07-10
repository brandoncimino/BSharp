using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.BSharp.Strings.Prettifiers;

internal static partial class InnerPretty {
#if NETSTANDARD2_0
    [Pure] public static string Tuple2<T1, T2>((T1, T2)                                                   tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple3<T, T2, T3>((T, T2, T3)                                             tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple4<T, T2, T3, T4>((T, T2, T3, T4)                                     tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple5<T, T2, T3, T4, T5>((T, T2, T3, T4, T5)                             tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple6<T, T2, T3, T4, T5, T6>((T, T2, T3, T4, T5, T6)                     tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple7<T, T2, T3, T4, T5, T6, T7>((T, T2, T3, T4, T5, T6, T7)             tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    [Pure] public static string Tuple8Plus<T, T2, T3, T4, T5, T6, T7, T8>((T, T2, T3, T4, T5, T6, T7, T8) tuple, PrettificationSettings? settings = default) => tuple.ToObjArray().PrettifyTupleArray(settings);
    
    private static string PrettifyTupleArray(this IEnumerable<object?> array, PrettificationSettings? settings) {
        settings ??= PrettificationSettings.Default;
        return array
               .Select(it => it.Prettify(settings))
               .JoinString(", ")
               .Circumfix("(", ")");
    }
#else
    internal static string PrettifyTuple(this ITuple tuple, PrettificationSettings settings) {
        return tuple.EachItem()
                    .Select(it => it.Prettify(settings))
                    .JoinString(", ")
                    .Circumfix("(", ")");
    }
#endif
}