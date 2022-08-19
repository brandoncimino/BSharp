using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Splits a <see cref="ReadOnlySpan{T}"/> into 2 parts.
/// </summary>
/// <typeparam name="T">the entry type</typeparam>
public readonly ref struct SpanPartition<T>
    where T : IEquatable<T> {
    /// <summary>
    /// The items before the splitter, if <see cref="IsSuccessful"/>
    /// </summary>
    public ReadOnlySpan<T> Before { get; }
    /// <summary>
    /// The items after the splitter, if <see cref="IsSuccessful"/>
    /// </summary>
    public ReadOnlySpan<T> After { get; }
    /// <summary>
    /// Whether the partition operation was successful or not - for example, whether a given splitter was found
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Splits by a sub-sequence.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// var result = new SpanPartition("ab--cd", "--");
    ///     
    /// IsSuccessful:   true
    /// Before:         [a, b]
    /// After:          [c, d]
    /// ]]></code>
    /// </example>
    /// <param name="source"></param>
    /// <param name="splitter"></param>
    public SpanPartition(ReadOnlySpan<T> source, ReadOnlySpan<T> splitter) {
        var splitIndex = source.IndexOf(splitter);

        if (splitIndex < 0) {
            IsSuccessful = false;
            Before       = default;
            After        = default;
        }
        else {
            IsSuccessful = true;
            Before       = source[..splitIndex];
            After        = source[(splitIndex + splitter.Length)..];
        }
    }

    /// <summary>
    /// Splits by a specific item.
    /// </summary>
    /// <example>
    /// var result = new SpanPartition("ab--cd", '-');
    ///     
    /// IsSuccessful:   true
    /// Before:         [a, b]
    /// After:          [-, c, d]
    /// </example>
    /// <param name="source"></param>
    /// <param name="splitter"></param>
    public SpanPartition(ReadOnlySpan<T> source, T splitter) {
        var splitIndex = source.IndexOf(splitter);

        if (splitIndex < 0) {
            IsSuccessful = false;
            Before       = default;
            After        = default;
        }
        else {
            IsSuccessful = true;
            Before       = source[..splitIndex];
            After        = source[(splitIndex + 1)..];
        }
    }

    /// <summary>
    /// Splits by an arbitrary <see cref="Range"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="splitter"></param>
    public SpanPartition(ReadOnlySpan<T> source, Range splitter) {
        if (source.Contains(splitter) == false) {
            IsSuccessful = false;
            Before       = default;
            After        = default;
            return;
        }

        var (off, len) = splitter.GetOffsetAndLength(source.Length);

        IsSuccessful = true;
        Before       = source[..off];
        After        = source[(off + len)..];
    }
}