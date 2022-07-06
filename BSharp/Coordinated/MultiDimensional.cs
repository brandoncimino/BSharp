using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// Provides basic multi-dimensional access to a one-dimensional <see cref="IList{T}"/>, similar to the way that <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/multidimensional-arrays">multidimensional arrays</a> work.
/// </summary>
/// <typeparam name="T">the type of the elements</typeparam>
/// <remarks>
/// This class is meant to provide implementation support for <see cref="ICoordinated{T}"/> classes.
/// </remarks>
internal sealed class MultiDimensional<T> {
    private IList<T>            Source      { get; }
    public  ImmutableArray<int> Dimensions  { get; }
    public  int                 TotalLength { get; }

    public MultiDimensional(IList<T> source, params int[] dimensions) {
        Source      = source;
        Dimensions  = dimensions.ToImmutableArray();
        TotalLength = Dimensions.Aggregate((a, b) => a * b);
        Must.Be(Source.Count == TotalLength);
    }

    public T this[params int[] indices] {
        get => Source[GetFlattenedIndex(indices)];
        set => Source[GetFlattenedIndex(indices)] = value;
    }

    private int GetFlattenedIndex(ReadOnlySpan<int> indices) {
        Must.Equal(indices.Length, Dimensions.Length, "You must provide an index for every dimension!");
        var flattenedIndex = 0;

        var boundsSoFar = 0;
        for (int i = 0; i < indices.Length; i++) {
            var currentDimension = ^(i + 1);
            var currentIndex     = indices[currentDimension];
            var currentBound     = Dimensions[currentDimension];
            Must.BeIndexOf(currentIndex, new Indexes(currentBound));
            flattenedIndex += currentIndex * boundsSoFar;
            boundsSoFar    += currentBound;
        }

        return flattenedIndex;
    }
}