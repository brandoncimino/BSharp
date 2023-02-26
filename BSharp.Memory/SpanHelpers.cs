using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Internal utilities to help <see cref="Span{T}"/> manipulation.
/// </summary>
internal static class SpanHelpers {
    public const StringSplitOptions TrimEntriesOption = (StringSplitOptions)2;

    /// <summary>
    /// Determines if <see cref="F:System.StringSplitOptions.TrimEntries"/> should apply.
    /// </summary>
    /// <remarks>
    /// TODO: Add the ability to provide a <see cref="Func{T, T}"/> predicate to <see cref="SpanSpliterator{T}"/> that will override this default behavior
    /// </remarks>
    /// <param name="entry">a <typeparamref name="T"/> instance</param>
    /// <returns><c>true</c> if we should trim the entry</returns>
    internal static bool IsTrimmable<T>(T? entry) {
        return entry switch {
            char c => char.IsWhiteSpace(c),
#if NET5_0_OR_GREATER
            System.Text.Rune r => System.Text.Rune.IsWhiteSpace(r),
#endif
            string s => string.IsNullOrWhiteSpace(s),
            null     => true,
            _        => false
        };
    }

    internal static ReadOnlySpan<T> GenericTrim<T>(this ReadOnlySpan<T> span) {
        // ReSharper disable once ConvertToLocalFunction
        Func<T, bool> trimmable = static it => IsTrimmable(it);
        return span.Skip(span.CountWhile(trimmable))
                   .Skip(span.CountWhile(trimmable, from: Spanq.From.End));
    }

    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> &lt; 0 or ≥ <see cref="length"/></exception>
    internal static int RequireIndex(
        this int                                     length,
        int                                          index,
        [CallerArgumentExpression("length")] string  _length = "length",
        [CallerArgumentExpression("index")]  string  _index  = "index",
        [CallerMemberName]                   string? _caller = default
    ) {
        if (index < 0 || index >= length) {
            throw OutOfBounds(length, index, _length, _index, _caller);
        }

        return index;
    }

    internal static ArgumentOutOfRangeException OutOfBounds(
        int                                          length,
        Index                                        index,
        [CallerArgumentExpression("length")] string  _length = "length",
        [CallerArgumentExpression("index")]  string  _index  = "index",
        [CallerMemberName]                   string? _caller = default
    ) {
        return new ArgumentOutOfRangeException(_index, index, $"🙅 {_caller}: {_index} {index} is out-of-bounds for a collection of {_length} {length}!");
    }

    internal static int RequireIndex(
        this int                                     length,
        Index                                        index,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerArgumentExpression("index")]  string? _index  = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        var off = index.GetOffset(length);

        if (off < 0 || off >= length) {
            throw new ArgumentOutOfRangeException(_index, index, $"🙅 {_caller}: {_index} {index} is out-of-bounds for {_length} {length}!");
        }

        return off;
    }

    internal static (int off, int len) RequireRange(
        this int                                     length,
        Range                                        range,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerArgumentExpression("range")]  string? _range  = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        var start = length.RequireIndex(range.Start, _length: _length, _caller: _caller);
        var end   = length.RequireIndex(range.End,   _length: _length, _caller: _caller);
        return (start, end - start);
    }

    internal static void RequireSpace(
        [NonNegativeValue] this             int     count,
        [NonNegativeValue]                  int     amountToAdd = 1,
        [NonNegativeValue]                  int     maximum     = RoMultiSpan.MaxSpans,
        [CallerArgumentExpression("count")] string? _count      = default,
        [CallerArgumentExpression("amountToAdd")]
        string? _amountToAdd = default,
        [CallerArgumentExpression("maximum")] string? _maximum = default,
        [CallerMemberName]                    string? _caller  = default
    ) {
        if (amountToAdd == 1 && count >= maximum) {
            throw new ArgumentOutOfRangeException(_amountToAdd, amountToAdd, $"🙅‍♀️ {_caller}: {_count} {count} has already hit the {_maximum} limit of {maximum}!");
        }

        if (count + amountToAdd > maximum) {
            throw new ArgumentOutOfRangeException(_amountToAdd, amountToAdd, $"🙅‍♀️ {_caller}: {_count} {count} + {_amountToAdd} {amountToAdd} would exceed the {_maximum} limit of {maximum}!");
        }
    }

    internal static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> span) => span;

    #region Lists

    /// <summary>
    /// The <see cref="FieldInfo"/> that stores the <see cref="Array"/> that backs a <see cref="List{T}"/>.
    /// </summary>
    private static readonly FieldInfo UnderlyingArrayField = typeof(List<>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingFieldException(nameof(List<object>), "_items");

    /// <param name="list">a <see cref="List{T}"/></param>
    /// <typeparam name="T">the list element type</typeparam>
    /// <returns>the <see cref="Array"/> that backs the given <see cref="List{T}"/></returns>
    private static T[] GetUnderlyingArray<T>(List<T> list) {
        var array = UnderlyingArrayField.GetValue(list);
        Debug.Assert(array != null);
        return (T[])array;
    }

    /// <summary>
    /// Retrieves the <see cref="Span{T}"/> that backs the given <see cref="List{T}"/>.
    /// </summary>
    /// <param name="list">a <see cref="List{T}"/></param>
    /// <typeparam name="T">the list element type</typeparam>
    /// <returns>the <see cref="Span{T}"/> that backs the <see cref="List{T}"/></returns>
    /// <remarks>
    /// This delegates to the .NET 6+ <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Runtime.InteropServices.CollectionsMarshal.AsSpan">CollectionsMarshal.AsSpan()</a> if it is available.
    /// </remarks>
    internal static Span<T> GetListSpan<T>(List<T> list) {
#if NET6_0_OR_GREATER
        return CollectionsMarshal.AsSpan(list);
#endif
        return GetUnderlyingArray(list);
    }

    #endregion
}