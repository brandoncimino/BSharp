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
    /// 
    ///                                             //    Span              pos
    /// span                                        // => [...........]     not yet declared
    ///     .Start(one,   out var pos)              // => [one........]     3
    ///     .Write(two,   ref pos)                  // => [onetwo.....]     6
    ///     .Write(three, ref pos);                 // => [onetwothree]     11
    ///
    /// Console.WriteLine(span.ToString());         // => "onetwothree" 
    /// ]]></code>
    /// </example>
    /// <param name="span">the <see cref="Span{T}"/> being filled</param>
    /// <param name="toWrite">the stuff being added to the <paramref name="span"/></param>
    /// <param name="cursor">where we should start adding <paramref name="toWrite"/>. The referenced variable will be updated with the the position where we <b>finished</b> adding <paramref name="toWrite"/></param>
    /// <typeparam name="T">the <see cref="Span{T}"/> type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> Write<T>(this Span<T> span, ReadOnlySpan<T> toWrite, ref int cursor) {
        return span.Write(toWrite, ref cursor, true);
    }

    /// <summary>
    /// <see cref="ReadOnlySpan{T}.CopyTo">Copies</see> <paramref name="toWrite"/> to <paramref name="span"/>, updating <paramref name="cursor"/> to the index in <paramref name="span"/> where we finished writing.
    /// </summary>
    /// <remarks>
    /// This method can be used as the first in a chain of <see cref="Write{T}"/> calls to avoid having to separately declare the <paramref name="cursor"/> variable.
    /// </remarks>
    /// <inheritdoc cref="Write{T}"/>
    public static Span<T> Start<T>(this Span<T> span, ReadOnlySpan<T> toWrite, out int cursor) {
        toWrite.CopyTo(span);
        cursor = toWrite.Length;
        return span;
    }

    private static Span<T> Write<T>(this Span<T> span, ReadOnlySpan<T> toWrite, ref int cursor, bool shouldValidatePosition) {
        if (toWrite.IsEmpty) {
            return span;
        }

        if (shouldValidatePosition) {
            if (cursor < 0 || cursor >= span.Length || (cursor + toWrite.Length) > span.Length) {
                throw new ArgumentOutOfRangeException($"Can't write {toWrite.Length} entires to a span of size {span.Length} starting at position {cursor}!");
            }
        }

        foreach (var c in toWrite) {
            span[cursor] =  c;
            cursor       += 1;
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