using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp;

public static class ValueArray {
    public static ValueArray<T> Of<T>(T                 item) where T : struct, IEquatable<T>             => item;
    public static ValueArray<T> Of<T>(T                 a, T b) where T : struct, IEquatable<T>           => ImmutableArray.Create(a, b);
    public static ValueArray<T> Of<T>(T                 a, T b, T c) where T : struct, IEquatable<T>      => ImmutableArray.Create(a, b, c);
    public static ValueArray<T> Of<T>(T                 a, T b, T c, T d) where T : struct, IEquatable<T> => ImmutableArray.Create(a, b, c, d);
    public static ValueArray<T> Of<T>(params T[]        items) where T : struct, IEquatable<T> => ImmutableArray.Create(items);
    public static ValueArray<T> Of<T>(ReadOnlySpan<T>   items) where T : struct, IEquatable<T> => items.CreateImmutableArray();
    public static ValueArray<T> Of<T>(ImmutableArray<T> items) where T : struct, IEquatable<T> => items;
    public static ValueArray<T> Of<T>(IEnumerable<T>    items) where T : struct, IEquatable<T> => items.ToImmutableArray();

    public static ValueArray<T> ToValueArray<T>(this ImmutableArray<T> items) where T : struct, IEquatable<T> => new(items);

    public static ReadOnlySpan<T> AsSpan<T>(this ValueArray<T>? array) where T : struct, IEquatable<T> => array.GetValueOrDefault().AsSpan();
}