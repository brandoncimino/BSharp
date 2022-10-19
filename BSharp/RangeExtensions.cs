using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Randomization;

using JetBrains.Annotations;

namespace FowlFever.BSharp;

public static class RangeExtensions {
    /// <param name="range">a <see cref="Range"/> of values. <i>⚠ Must not contain any <see cref="Index.IsFromEnd"/> values!</i></param>
    /// <param name="value">the <see cref="int"/> that might be inside the <see cref="Range"/></param>
    /// <param name="clusivity">determines whether the <see cref="Range.Start"/> and <see cref="Range.End"/> values should be considered</param>
    /// <returns><c>true</c> if this <see cref="Range"/> contains the given <paramref name="value"/></returns>
    /// <exception cref="NotSupportedException">if the <see cref="Range.Start"/> or <see cref="Range.End"/> <see cref="Index.IsFromEnd"/></exception>
    [Experimental("Can't handle Index.IsFromEnd == true")]
    public static bool Contains(this Range range, int value, Clusivity clusivity = Clusivity.Inclusive) {
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {
            throw new NotSupportedException($"How would I even handle {nameof(Index.IsFromEnd)}?!");
        }

        return value.IsBetween(range.Start.Value, range.End.Value, clusivity);
    }

    public static IEnumerable<int> Enumerate(this Range range) {
        Must.Have(range.Start.IsFromEnd == false && range.End.IsFromEnd == false);
        return Enumerable.Range(range.Start.Value, range.End.Value);
    }

    public static IEnumerable<int> Enumerate(this Range range, int length) {
        var (off, len) = range.GetOffsetAndLength(length);
        return Enumerable.Range(off, len);
    }

    [Experimental]
    public static IEnumerable<T> EnumerateSlice<T>(this Range range, IEnumerable<T> source) {
        static IEnumerable<T> Collection(ICollection<T> src, Range rng) {
            var (off, len) = rng.GetOffsetAndLength(src.Count);
            for (int i = off; i < (off + len); i++) {
                yield return src.ElementAt(i);
            }
        }

        static IEnumerable<T> ReadOnlyCollection(IReadOnlyCollection<T> src, Range rng) {
            var (off, len) = rng.GetOffsetAndLength(src.Count);
            for (int i = off; i < (off + len); i++) {
                yield return src.ElementAt(i);
            }
        }

        static IEnumerable<T> Enumerable(IEnumerable<T> src, Range rng) {
            throw Reject.Unsupported(typeof(RangeExtensions), details: "Too hard!");
            // var (start, end) = (rng.Start, rng.End);
            // return (start.IsFromEnd, end.IsFromEnd) switch {
            // (false, false) => src.Skip(start.Value).Take(end.Value - start.Value),
            // (false, true)  => src.Skip(start.Value).SkipLast(end.Value),
            // ^3..3
            // 1  2  3  4  5
            //      [3] 2  1  0 // ^3 => 2 => {3,4,5}
            // 0  1  2 [3]      //  3 => 3
            //                             ->> From {3,4,5} we want {4}, which is skip(1)
            //                             ->> skip(1) is 
            // (true, false) => src.Reverse().Skip(end.Value).Reverse().Take(start.Value),
            // (true, true)  => src.Reverse().Skip(end.Value).Take(start.Value - end.Value).Reverse(),
            // };
        }

        return source switch {
            ICollection<T> coll       => Collection(coll, range),
            IReadOnlyCollection<T> rc => ReadOnlyCollection(rc, range),
            _                         => Enumerable(source, range),
        };
    }

    /// <summary>
    /// Tests if a collection of size <paramref name="collectionLength"/> would contain the given <see cref="Index"/>.
    /// </summary>
    /// <param name="collectionLength">the size of the theoretical collection. Corresponds to the <c>length</c> parameter of <see cref="Index.GetOffset"/>.</param>
    /// <param name="index">the <see cref="Index"/> being tested</param>
    /// <returns><c>true</c> if the <see cref="Index"/> would fall within the collection</returns>
    [Pure]
    public static bool ContainsIndex(this int collectionLength, Index index) => collectionLength.ContainsIndex(index.GetOffset(collectionLength));

    /// <inheritdoc cref="ContainsIndex(int,System.Index)"/>
    [Pure]
    public static bool ContainsIndex([NonNegativeValue] this int collectionLength, int index) => index >= 0 && index < collectionLength;

    /// <summary>
    /// Tests if a collection of size <paramref name="collectionLength"/> would contain the given <see cref="Range"/>.
    /// </summary>
    /// <param name="collectionLength">the size of a theoretical collection. Corresponds to the <c>length</c> parameter of <see cref="Range.GetOffsetAndLength"/>.</param>
    /// <param name="range">the <see cref="Range"/> being tested</param>
    /// <returns><c>true</c> if the <see cref="Range"/>'s <see cref="Range.Start"/> and <see cref="Range.End"/> would both fall within the collection</returns>
    [Pure]
    public static bool ContainsRange(this int collectionLength, Range range) => collectionLength.ContainsIndex(range.Start) && collectionLength.ContainsIndex(range.End);

    /// <summary>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct">Deconstructs</a> the <see cref="Range.Start"/> and <see cref="Range.End"/> of a <see cref="Range"/>.
    /// </summary>
    /// <param name="range">this <see cref="Range"/></param>
    /// <param name="start">set to the <see cref="Range.Start"/></param>
    /// <param name="end">set to the <see cref="Range.End"/></param>
    [Pure]
    public static void Deconstruct(this Range range, out Index start, out Index end) {
        start = range.Start;
        end   = range.End;
    }

    /// <summary>
    /// Iterates through the <see cref="IndexExtensions.SignedValue"/>s of this <see cref="Range"/>'s <see cref="Range.Start"/> and <see cref="Range.End"/>.
    /// </summary>
    /// <param name="range">this <see cref="Range"/></param>
    /// <returns></returns>
    [Pure]
    public static IndexRangeEnumerator GetEnumerator(this Range range) => new(range);

    /// <summary>
    /// Generates a random <see cref="int"/> within the <see cref="IndexExtensions.SignedValue"/>s of this <see cref="Range"/>'s <see cref="Range.Start"/> and <see cref="Range.End"/>.
    /// </summary>
    /// <param name="range">this <see cref="Range"/></param>
    /// <param name="generator">a <see cref="System.Random"/> instance. Defaults to <see cref="Brandom.Gen"/> if <c>null</c> or omitted</param>
    /// <returns>a random <see cref="int"/></returns>
    [Pure]
    public static int Random(this Range range, Random? generator = default) => generator.InRange(range);
}