using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

public static class AggregationExtensions {
    public static (TSource, TSource) DoubleAggro<TSource>(
        this IEnumerable<TSource>       source,
        Func<TSource, TSource, TSource> aggro_1,
        Func<TSource, TSource, TSource> aggro_2
    ) {
        var multi = source.MultiAggro(aggro_1, aggro_2);
        return Must.Have2(multi);
    }

    public static TSource[] MultiAggro<TSource>(
        this   IEnumerable<TSource>              source,
        params Func<TSource, TSource, TSource>[] aggregators
    ) {
        TSource[]? soFar = null;

        foreach (var next in source) {
            if (soFar == null) {
                soFar = Enumerable.Repeat(next, aggregators.Length).ToArray();
                continue;
            }

            for (int i = 0; i < soFar.Length; i++) {
                soFar[i] = aggregators[i](soFar[i], next);
            }
        }

        return soFar.OrEmpty();
    }
}