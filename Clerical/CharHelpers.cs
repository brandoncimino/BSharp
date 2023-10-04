using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

using FowlFever.BSharp.Memory;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

internal static class CharHelpers {
    private static bool ContainsAsciiLetterUpper(Vector<ushort> charVector) {
        Debug.Assert(Vector.IsHardwareAccelerated, $"Should have already checked for {nameof(Vector.IsHardwareAccelerated)}");

        return Vector.GreaterThanOrEqualAny(charVector, new Vector<ushort>('A'))
               && Vector.LessThanOrEqualAny(charVector, new Vector<ushort>('Z'));
    }

    public static bool ContainsAsciiLetterUpper(ReadOnlySpan<char> span) {
        var index = 0;

        if (Vector.IsHardwareAccelerated) {
            var vectorizableSpan = MemoryMarshal.Cast<char, ushort>(span);

            while (index + Vector<ushort>.Count <= vectorizableSpan.Length) {
                var spanSlice = vectorizableSpan[index..];
                index += Vector<ushort>.Count;
                var vectorSlice = VectorMath.CreateVector(spanSlice);
                if (ContainsAsciiLetterUpper(vectorSlice)) {
                    return true;
                }
            }
        }

        for (; index < span.Length; index++) {
            if (IsAsciiLetterUpper(span[index])) {
                return true;
            }
        }

        return false;
    }

    #region Methods stolen from .NET 7

    /// <inheritdoc cref="char.IsBetween"/>
    /// <remarks>
    /// Taken from .NET 7's <a href="https://learn.microsoft.com/en-us/dotnet/api/system.char.isbetween?view=net-7.0">Char.IsBetween</a>.
    /// </remarks>
    /// <seealso cref="char.IsBetween"/>
    private static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
#if NET7_0_OR_GREATER
        char.IsBetween(c, minInclusive, maxInclusive);
#else
        (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
#endif

    /// <inheritdoc cref="char.IsAsciiLetterLower"/>
    /// <remarks>
    /// This determines whether the character is in the range 'a' through 'z', inclusive.
    /// <p/>
    /// Taken from .NET 7's <a href="https://learn.microsoft.com/en-us/dotnet/api/system.char.isasciiletterupper?view=net-7.0">Char.IsAsciiLetterUpper</a>.
    /// </remarks>
    public static bool IsAsciiLetterUpper(char c) =>
#if NET7_0_OR_GREATER
        char.IsAsciiLetterUpper(c);
#else
        IsBetween(c, 'A', 'Z');
#endif

    public static StringSegment AsStringSegment(this ReadOnlySpan<char> span, string? source) {
        return source.AsSpan().Overlaps(span, out var offset) ? new StringSegment(source!, offset, span.Length) : default;
    }

    #endregion
}