using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Writing

    private static Span<T> RequireIndex<T>(this Span<T> span, int index) {
        Console.WriteLine($"validating index {index} against span {span.FormatString()}");
        if ((index >= 0 && index < span.Length) == false) {
            throw new IndexOutOfRangeException($"Index {index} is out-of-bounds for a {nameof(Span<T>)}.{nameof(Span<T>.Length)} of {span.Length}!");
        }

        return span;
    }

    /// <summary>
    /// Helps you "fill" a <see cref="Span{T}"/> with stuff.
    /// </summary>
    /// <example>
    /// Chaining <see cref="Write{T}"/> calls together lets you add entries to a <see cref="Span{T}"/> in a nice, clean way:
    /// <code><![CDATA[
    /// // Some strings we want to combine into a single Span<char>
    /// var one =   "one";
    /// var two =   "two";
    /// var three = "three";
    /// 
    /// var length = one.Length +
    ///              two.Length +
    ///              three.Length;                  // Total length: 11 
    ///
    /// 
    /// Span<char> span = stackalloc[totalLength];  // Allocate a new Span<char>
    /// var pos = 0;                                // Create a variable to hold the current position that we're writing to
    /// 
    ///                                             //    Span              pos
    /// span                                        // => [...........]     0
    ///     .Write(one,   ref pos)                  // => [one........]     3
    ///     .Write(two,   ref pos)                  // => [onetwo.....]     6
    ///     .Write(three, ref pos);                 // => [onetwothree]     11
    ///
    /// Console.WriteLine(span.ToString());         // => "onetwothree" 
    /// ]]></code>
    /// </example>
    /// <param name="span">the <see cref="Span{T}"/> being filled</param>
    /// <param name="toWrite">the stuff being added to the <paramref name="span"/></param>
    /// <param name="position">where we should start adding <paramref name="toWrite"/>. The referenced variable will be updated with the the position where we <b>finished</b> adding <paramref name="toWrite"/></param>
    /// <typeparam name="T">the <see cref="Span{T}"/> type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> Write<T>(this Span<T> span, ReadOnlySpan<T> toWrite, ref int position) {
        return span.Write(toWrite, ref position, true);
    }

    private static Span<T> Write<T>(this Span<T> span, ReadOnlySpan<T> toWrite, ref int position, bool shouldValidatePosition) {
        if (toWrite.IsEmpty) {
            return span;
        }

        if (shouldValidatePosition) {
            if (position < 0 || position >= span.Length || (position + toWrite.Length) > span.Length) {
                throw new ArgumentOutOfRangeException($"Can't write {toWrite.Length} entires to a span of size {span.Length} starting at position {position}!");
            }
        }

        foreach (var c in toWrite) {
            span[position] =  c;
            position       += 1;
        }

        return span;
    }

    /// <summary>
    /// Equivalent to <see cref="Write{T}"/> but with an optional <paramref name="joiner"/> applied if <paramref name="position"/> &gt; 0 and <see cref="source"/> is not <see cref="ReadOnlySpan{T}.Empty"/>.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <param name="joiner"></param>
    /// <param name="position"></param>
    /// <param name="shouldValidatePosition"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Span<T> WriteJoin<T>(
        this Span<T>    destination,
        ReadOnlySpan<T> source,
        ReadOnlySpan<T> joiner,
        ref int         position,
        bool            shouldValidatePosition = false
    ) {
        if (source.IsEmpty) {
            return destination;
        }

        if (position > 0) {
            destination.Write(joiner, ref position, shouldValidatePosition);
        }

        return destination.Write(source, ref position, shouldValidatePosition);
    }

    #endregion
}