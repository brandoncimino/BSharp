using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Partition

    /// <summary>
    /// Separates everything <b>before</b> and <b>after</b> <paramref name="splitterIndex"/>, <b><i>dropping <paramref name="splitterIndex"/></i></b>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitterIndex">the element that separates the before and after partitions</param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns><c>(before, after)</c> as a <see cref="RoSpanTuple{TA,TB}"/></returns>
    public static RoSpanTuple<T, T> Partition<T>(this ReadOnlySpan<T> span, int splitterIndex) => new(span[..splitterIndex], span[(splitterIndex + 1)..]);

    /// <inheritdoc cref="Partition{T}(System.ReadOnlySpan{T},int)"/>
    public static RoSpanTuple<T, T> Partition<T>(this ReadOnlySpan<T> span, Index splitterIndex) => span.Partition(splitterIndex.GetOffset(span.Length));

    /// <summary>
    /// Separates everything <b>before</b> and <b>after</b> a <see cref="Range"/> <i>(dropping the <see cref="Range"/> itself)</i>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitterRange">the <see cref="Range"/> used to separate the before and after parts</param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns><c>(before, after)</c> as a <see cref="RoSpanTuple{TA,TB}"/></returns>
    public static RoSpanTuple<T, T> Partition<T>(this ReadOnlySpan<T> span, Range splitterRange) => new(
        span[..splitterRange.Start],
        span[splitterRange.End..]
    );

    /// <summary>
    /// <inheritdoc cref="Partition{T}(System.ReadOnlySpan{T},Range)"/> 
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="index">the start of the range used to split the <paramref name="span"/></param>
    /// <param name="length">the size of the range used to split the <paramref name="span"/></param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns><c>(before, after)</c> as a <see cref="RoSpanTuple{TA,TB}"/></returns>
    public static RoSpanTuple<T, T> Partition<T>(this ReadOnlySpan<T> span, int index, int length) => new(
        span[..index],
        span[(index + length)..]
    );

    #endregion

    #region TryPartition

    #region Index-based

    /// <summary>
    /// <i>Attempts</i> to separate everything <b>before</b> and <b>after</b> <paramref name="splitterIndex"/>, <b><i>dropping <paramref name="splitterIndex"/></i></b>.
    /// <p/>
    /// If <paramref name="splitterIndex"/> is out-of-bounds, <c>false</c> is returned, and <paramref name="before"/> and <paramref name="after"/> will both be <see cref="ReadOnlySpan{T}.Empty"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitterIndex">the index that indicates the transition between <paramref name="before"/> and <paramref name="after"/></param>
    /// <param name="before">set to the contents of <paramref name="span"/> located <b>before</b> <paramref name="splitterIndex"/></param>
    /// <param name="after">set to the contents of <paramref name="span"/> located <b>after</b> <paramref name="splitterIndex"/> <i>(📎 <paramref name="splitterIndex"/> is <b>not</b> included!)</i></param>
    /// <typeparam name="T">the <paramref name="span"/> entry type</typeparam>
    /// <returns>whether or not the <paramref name="splitterIndex"/> fell within the <paramref name="span"/></returns>
    public static bool TryPartition<T>(this ReadOnlySpan<T> span, int splitterIndex, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) {
        if (splitterIndex < 0) {
            before = default;
            after  = default;
            return false;
        }

        before = span[..splitterIndex];
        after  = span[(splitterIndex + 1)..];
        return true;
    }

    /// <inheritdoc cref="TryPartition{T}(System.ReadOnlySpan{T},int,out System.ReadOnlySpan{T},out System.ReadOnlySpan{T})"/>
    public static bool TryPartition<T>(this ReadOnlySpan<T> span, Index splitterIndex, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) {
        return span.TryPartition(splitterIndex.GetOffset(span.Length), out before, out after);
    }

    public static bool TryPartition<T>(
        this ReadOnlySpan<T> span,
        int                  index,
        int                  length,
        out ReadOnlySpan<T>  before,
        out ReadOnlySpan<T>  after
    ) where T : IEquatable<T> {
        if (span.Length.ContainsIndex(index) && span.Length.ContainsIndex(index + length)) {
            before = span[..index];
            after  = span[(index + length)..];
            return true;
        }

        before = default;
        after  = default;
        return false;
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, Range)"/>
    public static bool TryPartition<T>(this ReadOnlySpan<T> span, Range range, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) where T : IEquatable<T> {
        if (span.Length.ContainsRange(range)) {
            before = span[..range.Start];
            after  = span[range.End..];
            return true;
        }

        before = default;
        after  = default;
        return false;
    }

    #endregion

    /// <summary>
    /// Splits everything before and after the <b>first</b> occurence of <paramref name="splitter"/>.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="splitter"/> isn't found, both <paramref name="before"/> and <paramref name="after"/> will be <see cref="ReadOnlySpan{T}.Empty"/>.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitter">the item to split the <paramref name="span"/> on</param>
    /// <param name="before">the entries before the <paramref name="splitter"/></param>
    /// <param name="after">the entries after the <paramref name="splitter"/></param>
    /// <typeparam name="T">the <paramref name="span"/> entry type</typeparam>
    /// <returns>whether or not the <paramref name="splitter"/> was found in the <paramref name="span"/></returns>
    public static bool TryPartition<T>(this ReadOnlySpan<T> span, T splitter, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) where T : IEquatable<T> {
        return span.TryPartition(span.IndexOf(splitter), out before, out after);
    }

    /// <summary>
    /// Splits everything before and after the <b>last</b> occurence of <paramref name="splitter"/>.
    /// </summary>
    /// <inheritdoc cref="TryPartition{T}(System.ReadOnlySpan{T},int,out System.ReadOnlySpan{T},out System.ReadOnlySpan{T})"/>
    public static bool TryPartitionLast<T>(this ReadOnlySpan<T> span, T splitter, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) where T : IEquatable<T> {
        return span.TryPartition(span.LastIndexOf(splitter), out before, out after);
    }

    /// <summary>
    /// <i>Attempts</i> to separate everything before and after the <b><i>first</i></b> occurence of <paramref name="splitSequence"/>, returning <c>true</c> if the <paramref name="splitSequence"/>
    /// is found.
    /// </summary>
    /// <remarks>
    /// If <paramref name="splitSequence"/> <i>isn't</i> found, <paramref name="before"/> and <paramref name="after"/> will be set to <see cref="ReadOnlySpan{T}.Empty"/>.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitSequence">the sequence to split the <paramref name="span"/> by</param>
    /// <param name="before">the elements before the <paramref name="splitSequence"/></param>
    /// <param name="after">the elements after the <paramref name="splitSequence"/></param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns><c>true</c> if the <paramref name="splitSequence"/> was found in the <paramref name="span"/></returns>
    public static bool TryPartition<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) where T : IEquatable<T> {
        return span.TryPartition(span.IndexOf(splitSequence), splitSequence.Length, out before, out after);
    }

    /// <summary>
    /// <i>Attempts</i> to separate everything before and after the <b><i>last</i></b> occurence of <paramref name="splitSequence"/>, return <c>true</c> if the <paramref name="splitSequence"/> is found.
    /// </summary>
    /// <inheritdoc cref="TryPartition{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T},out System.ReadOnlySpan{T},out System.ReadOnlySpan{T})"/>
    public static bool TryPartitionLast<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after) where T : IEquatable<T> {
        return span.TryPartition(span.LastIndexOf(splitSequence), splitSequence.Length, out before, out after);
    }

    #endregion
}