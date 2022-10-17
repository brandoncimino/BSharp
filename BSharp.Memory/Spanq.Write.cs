using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Writing

    #region Start

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

    #endregion

    #region Write

    /// <summary>
    /// Helps you "fill" a <see cref="Span{T}"/> with stuff.
    /// </summary>
    /// <example>
    /// <see cref="Start{T}"/>, <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/>, and <see cref="Build{T,TOut}"/> can be chained together
    /// to populate <see cref="Span{T}"/> entries in a fluent way:
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
    ///     .Write(three, ref pos)                  // => [onetwothree]     11
    /// 
    ///     .Build(pos, it => it.ToString());       // => "onetwothree" 
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
    /// Sets <see cref="Span{T}.this">this[cursor]</see> to <paramref name="toAdd"/>, then advances the <paramref name="cursor"/> by 1.
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

    /// <summary>
    /// <see cref="Write{T}(System.Span{T},T,ref int)"/>s <paramref name="joiner"/> if this <see cref="Span{T}"/> isn't <see cref="Span{T}.IsEmpty"/>; then <see cref="Write{T}(System.Span{T},T,ref int)"/>s <paramref name="toWrite"/>. 
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="toWrite">the <typeparamref name="T"/> entry being written to this <paramref name="span"/></param>
    /// <param name="joiner">it this <see cref="Span{T}"/> isn't <see cref="Span{T}.IsEmpty"/>, then write this first before <paramref name="toWrite"/></param>
    /// <param name="cursor">the index in this <paramref name="span"/> where we will write things. <i>📎 Updated during the course of this method!</i></param>
    /// <typeparam name="T">the type of the entries in this <paramref name="span"/></typeparam>
    /// <returns>this <paramref name="span"/>, for method chaining</returns>
    public static Span<T> WriteJoin<T>(
        this Span<T> span,
        T            toWrite,
        T            joiner,
        ref int      cursor
    ) {
        if (cursor > 0) {
            span.Write(joiner, ref cursor);
        }

        return span.Write(toWrite, ref cursor);
    }

    #endregion

    #region Specialized

    /// <summary>
    /// <see cref="Write{T}(System.Span{T},T,ref int)"/>s <paramref name="toWrite"/> if it isn't the most recent entry written to the <paramref name="span"/> (i.e. <paramref name="cursor"/> - 1)
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="toWrite">the <typeparamref name="T"/> value being written to the <paramref name="span"/></param>
    /// <param name="cursor">the index in this <paramref name="span"/> where we will write stuff</param>
    /// <typeparam name="T">the span entry type</typeparam>
    /// <returns>this <see cref="Span{T}"/>, for method chaining</returns>
    public static Span<T> WriteIfMissing<T>(this Span<T> span, T toWrite, ref int cursor) where T : IEquatable<T> {
        if (cursor > 0 && span[cursor - 1].Equals(toWrite)) {
            return span;
        }

        return span.Write(toWrite, ref cursor);
    }

    /// <inheritdoc cref="WriteIfMissing{T}(System.Span{T},T,ref int)"/>
    public static Span<T> WriteIfMissing<T>(this Span<T> span, ReadOnlySpan<T> toWrite, ref int cursor) where T : IEquatable<T> {
        return span[..cursor].EndsWith(toWrite) ? span : span.Write(toWrite, ref cursor);
    }

    #endregion

    #region End

    private static InvalidOperationException SpanNotFinishedException<T>(Span<T> span, int cursor) {
        return new InvalidOperationException($"The {nameof(cursor)} {cursor} is not at the end of the {span.SpanType()}[📏{span.Length}]!");
    }

    /// <summary>
    /// Converts this <see cref="Span{T}"/> to a <typeparamref name="TOut"/>, asserting that the <paramref name="cursor"/> is at the end of the <see cref="Span{T}"/>. 
    /// </summary>
    /// <example>
    /// <inheritdoc cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)"/>
    /// </example>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="cursor">the <i>exclusive</i> index where we finished <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)">writing</see> to the <see cref="Span{T}"/></param>
    /// <param name="finisher">transforms the <see cref="Span{T}"/> to a <see cref="TOut"/></param>
    /// <typeparam name="T">the <see cref="Span{T}"/> entry type</typeparam>
    /// <typeparam name="TOut">the result type</typeparam>
    /// <returns>the <see cref="SpanFunc{TIn,TOut}"/> result</returns>
    /// <exception cref="InvalidOperationException">if the <paramref name="cursor"/> isn't equal to the <see cref="Span{T}.Length"/></exception>
    public static TOut Build<T, TOut>(this Span<T> span, in int cursor, SpanFunc<T, TOut> finisher) {
        if (cursor != span.Length) {
            throw SpanNotFinishedException(span, cursor);
        }

        return finisher(span);
    }

    /// <summary>
    /// <inheritdoc cref="Build{T,TOut}"/>
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="cursor">the <i>exclusive</i> index where we finished <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)">writing</see> to the <see cref="Span{T}"/></param>
    /// <param name="arg">an additional value sent to the <paramref name="finisher"/></param>
    /// <param name="finisher">transforms the <see cref="Span{T}"/> to a <typeparamref name="TOut"/></param>
    /// <typeparam name="T">the <see cref="Span{T}"/> entry type</typeparam>
    /// <typeparam name="TArg">the additional <paramref name="arg"/> type</typeparam>
    /// <typeparam name="TOut">the result type</typeparam>
    /// <returns>the <see cref="SpanFunc{TIn,TArg,TOut}"/> result</returns>
    /// <exception cref="InvalidOperationException">if the <paramref name="cursor"/> isn't equal to the <see cref="Span{T}.Length"/></exception>
    public static TOut Build<T, TArg, TOut>(this Span<T> span, in int cursor, TArg arg, SpanFunc<T, TArg, TOut> finisher) {
        if (cursor != span.Length - 1) {
            throw SpanNotFinishedException(span, cursor);
        }

        return finisher(span, arg);
    }

    /// <summary>
    /// Converts this <see cref="Span{T}"/> to a <see cref="string"/>, asserting that the <paramref name="cursor"/> is at the end of the <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="cursor">the <i>exclusive</i> index where we finished <see cref="Write{T}(System.Span{T},System.ReadOnlySpan{T},ref int)">writing</see> to the <see cref="Span{T}"/></param>
    /// <returns>a new <see cref="string"/></returns>
    public static string BuildString(this Span<char> span, in int cursor) {
        return span.Build(in cursor, static it => it.ToString());
    }

    #endregion

    #endregion
}