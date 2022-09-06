using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Writing

    #region Write

    /// <summary>
    /// <see cref="ReadOnlySpan{T}.CopyTo">Copies</see> <paramref name="toWrite"/> to <paramref name="span"/>, updating <paramref name="cursor"/> to the index in <paramref name="span"/> where we finished writing.
    /// </summary>
    /// <remarks>
    /// This method can be used as the first in a chain of <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/> calls to avoid having to separately declare the <paramref name="cursor"/> variable.
    /// </remarks>
    /// <inheritdoc cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/>
    public static Span<T> Start<T>(this Span<T> span, ReadOnlySpan<T> toWrite, out int cursor) {
        toWrite.CopyTo(span);
        cursor = toWrite.Length;
        return span;
    }

    /// <summary>
    /// Helps you "fill" a <see cref="Span{T}"/> with stuff.
    /// </summary>
    /// <example>
    /// Chaining <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/> calls together lets you add entries to a <see cref="Span{T}"/> in a nice, clean way:
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
    /// Sets <c>this[cursor]</c> to <paramref name="toAdd"/>, then advances the <paramref name="cursor"/> by 1.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="toAdd">the <typeparamref name="T"/></param>
    /// <param name="cursor">the index where <paramref name="toAdd"/> will be placed</param>
    /// <typeparam name="T">the <see cref="Span{T}"/> element type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> Write<T>(this Span<T> span, T toAdd, ref int cursor) {
        span[cursor] =  toAdd;
        cursor       += 1;
        return span;
    }

    #endregion

    #region WriteJoin

    /// <summary>
    /// Similar to <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/>, but inserts a <paramref name="joiner"/> if <paramref name="cursor"/> &gt; 0
    /// and <paramref name="toWrite"/> isn't <see cref="ReadOnlySpan{T}.IsEmpty"/>.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="toWrite">the <see cref="ReadOnlySpan{T}"/> that we want to copy to this <paramref name="span"/></param>
    /// <param name="joiner">written to <paramref name="span"/> if we aren't at the the start of <paramref name="span"/> (i.e. <paramref name="cursor"/> is 0) and <paramref name="toWrite"/> isn't <see cref="ReadOnlySpan{T}.IsEmpty"/></param>
    /// <param name="cursor">the index in <paramref name="span"/> that we will start writing</param>
    /// <typeparam name="T">the <see cref="Span{T}"/> element type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> WriteJoin<T>(
        this Span<T>    span,
        ReadOnlySpan<T> toWrite,
        ReadOnlySpan<T> joiner,
        ref int         cursor
    ) {
        return span.WriteJoin(toWrite, joiner, ref cursor, true);
    }

    private static Span<T> WriteJoin<T>(
        this Span<T>    span,
        ReadOnlySpan<T> toWrite,
        ReadOnlySpan<T> joiner,
        ref int         cursor,
        bool            shouldValidatePosition
    ) {
        if (toWrite.IsEmpty) {
            return span;
        }

        if (cursor > 0) {
            span.Write(joiner, ref cursor, shouldValidatePosition);
        }

        return span.Write(toWrite, ref cursor, shouldValidatePosition);
    }

    /// <summary>
    /// Similar to <see cref="Write{T}(System.Span{T},T,ref int)"/>, but first writes <paramref name="joiner"/> if <paramref name="cursor"/> is &gt; 0.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="toWrite">the <typeparamref name="T"/> instance to be written</param>
    /// <param name="joiner">written before <paramref name="toWrite"/> if <paramref name="cursor"/> &gt; 0</param>
    /// <param name="cursor">the index in <paramref name="span"/> where we will write things</param>
    /// <typeparam name="T">the <see cref="Span{T}"/> element type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> WriteJoin<T>(
        this Span<T>    span,
        T               toWrite,
        ReadOnlySpan<T> joiner,
        ref int         cursor
    ) {
        if (cursor > 0) {
            span.Write(joiner, ref cursor);
        }

        return span.Write(toWrite, ref cursor);
    }

    #endregion

    #endregion
}