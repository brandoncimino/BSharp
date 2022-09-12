using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Attempts to divide a <see cref="ReadOnlySpan{T}"/> into 2 parts, returning those parts as <paramref name="before"/> and <paramref name="after"/>.
/// </summary>
/// <remarks>
/// When a partitioner returns <c>false</c>, both <paramref name="before"/> and <paramref name="after"/> should be <see cref="ReadOnlySpan{T}.Empty"/>
/// <i>(or at least, <see cref="Invoke">invokers</see> should assume that they are)</i>.
/// <br/>
/// No other assumptions can be made about the contents of <paramref name="before"/> and <paramref name="after"/>.
/// For example, a partitioner performing <see cref="string.Split(char,int,System.StringSplitOptions)"/>-style operations will generally drop the splitters themselves,
/// or possibly <see cref="StringSplitOptions.RemoveEmptyEntries"/>.
/// </remarks>
/// <typeparam name="T">the span element type</typeparam>
/// <typeparam name="TArg">the type of <paramref name="arg"/>'s elements</typeparam>
/// <param name="source">the original <see cref="ReadOnlySpan{T}"/></param>
/// <param name="arg">additional stuff passed into the function</param>
/// <param name="before">the first chunk of <paramref name="source"/></param>
/// <param name="after">the second chunk of <paramref name="source"/></param>
/// <returns>whether or not the partition was a success, and therefore <paramref name="before"/> and <paramref name="after"/> contain meaningful results</returns>
[Obsolete($"Experimental: supports the experimental {nameof(SpanPartitioner<T, TArg>)}")]
public delegate bool BiRoSpanPartitioner<T, TArg>(ReadOnlySpan<T> source, ReadOnlySpan<TArg> arg, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after);

/// <summary>
/// Repeatedly applies a <see cref="BiRoSpanPartitioner{T,TSpanArg}">partitioning function</see> to a <see cref="ReadOnlySpan{T}"/>, enumerating the results.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TArg"></typeparam>
[Obsolete("Experimental: needs better communication around how it works, and full testing")]
public ref struct SpanPartitioner<T, TArg> {
    private readonly ReadOnlySpan<TArg>           _args;
    private readonly BiRoSpanPartitioner<T, TArg> _partitioner;
    public readonly  int                          PartitionLimit { get; init; } = int.MaxValue;
    public readonly  StringSplitOptions           Options        { get; init; } = StringSplitOptions.None;

    private ReadOnlySpan<T> _remaining;
    private ReadOnlySpan<T> _current = default;
    private bool            _isActive;
    private int             _partitionCount = 0;

    public readonly ReadOnlySpan<T> Current => _current;

    public SpanPartitioner<T, TArg> GetEnumerator() => this;

    public SpanPartitioner(
        ReadOnlySpan<T>                                      source,
        ReadOnlySpan<TArg>                                   args,
        [RequireStaticDelegate] BiRoSpanPartitioner<T, TArg> partitioner
    ) {
        _isActive    = source.IsEmpty == false;
        _remaining   = source;
        _partitioner = partitioner;
        _args        = args;
    }

    public bool MoveNext() {
        if (_isActive == false) {
            return false;
        }

        _partitionCount += 1;

        if (
            _partitionCount < PartitionLimit &&
            _partitioner(_remaining, _args, out var before, out var after)
        ) {
            _current   = before;
            _remaining = after;
        }
        else {
            Finish();
        }

        //check _options
        if (Options.HasFlag(SpanSplitHelpers.TrimEntriesOption)) {
            _current = _current.GenericTrim();
        }

        if (Options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && _current.IsEmpty) {
            return MoveNext();
        }

        return true;
    }

    private void Finish() {
        _current   = _remaining;
        _remaining = default;
        _isActive  = false;
    }
}