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

    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> &lt; 0 or ≥ <see cref="length"/></exception>
    internal static int RequireIndex(
        this int                                     length,
        int                                          index,
        [CallerArgumentExpression("length")] string  _length = "length",
        [CallerArgumentExpression("index")]  string  _index  = "index",
        [CallerMemberName]                   string? _caller = default
    ) {
        Debug.Assert(length >= 0, "Expected a non-negative length!");
        if ((uint)index >= (uint)length) {
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
        // TODO: This can probably be optimized with `uint` nonsense
        if (amountToAdd == 1 && count >= maximum) {
            throw new ArgumentOutOfRangeException(_amountToAdd, amountToAdd, $"🙅‍♀️ {_caller}: {_count} {count} has already hit the {_maximum} limit of {maximum}!");
        }

        if (count + amountToAdd > maximum) {
            throw new ArgumentOutOfRangeException(_amountToAdd, amountToAdd, $"🙅‍♀️ {_caller}: {_count} {count} + {_amountToAdd} {amountToAdd} would exceed the {_maximum} limit of {maximum}!");
        }
    }

    /// <inheritdoc cref="Span{T}.op_Implicit(System.Span{T})"/>
    internal static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> span) => span;

    #region Creating spans

    /// <inheritdoc cref="Unsafe.AsRef{T}(System.Void*)"/>
    /// <remarks>
    /// This method is equivalent to <see cref="Unsafe.AsRef{T}(Void*)"/>, but uses an <see cref="nint"/> instead of a <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code#pointer-types">void*</a>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ref T AsRef<T>(nint pointer) {
#if NET7_0_OR_GREATER
        return ref Unsafe.AsRef<T>(pointer.ToPointer());
#else
        // TODO: Check if this actually works in .NET 6!
        return ref Unsafe.AsRef<T>((void*)pointer);
#endif
    }

    /// <summary>
    /// Similar to <see cref="MemoryMarshal.CreateReadOnlySpan{T}"/>, but uses an <see cref="nint"/> to the start of the span instead of a <c>ref</c> <typeparamref name="T"/>.
    /// </summary>
    /// <param name="pointerToFirstElement">an <see cref="nint"/> that points to the <see cref="MemoryMarshal.GetReference{T}(System.ReadOnlySpan{T})"/></param>
    /// <param name="elementCount">the desired <see cref="ReadOnlySpan{T}.Length"/></param>
    /// <typeparam name="T">the element type</typeparam>
    /// <returns>a new <see cref="ReadOnlySpan{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(nint pointerToFirstElement, int elementCount) {
        return MemoryMarshal.CreateReadOnlySpan(ref AsRef<T>(pointerToFirstElement), elementCount);
    }

    /// <summary>
    /// Similar to <see cref="MemoryMarshal.CreateReadOnlySpan{T}"/>, but uses an <see cref="nint"/> to the start of the span instead of a <c>ref</c> <typeparamref name="T"/>.
    /// </summary>
    /// <param name="pointerToFirstElement">an <see cref="nint"/> that points to the <see cref="MemoryMarshal.GetReference{T}(System.ReadOnlySpan{T})"/></param>
    /// <param name="elementCount">the desired <see cref="ReadOnlySpan{T}.Length"/></param>
    /// <typeparam name="T">the element type</typeparam>
    /// <returns>a new <see cref="ReadOnlySpan{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> CreateSpan<T>(nint pointerToFirstElement, int elementCount) {
        return MemoryMarshal.CreateSpan(ref AsRef<T>(pointerToFirstElement), elementCount);
    }

    #endregion

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