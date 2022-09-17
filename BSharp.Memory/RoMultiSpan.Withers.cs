using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    /// <summary>
    /// Creates a new <see cref="RoMultiSpan{T}"/>, <b>replacing</b> the <see cref="ReadOnlySpan{T}"/> at index <paramref name="spanIndex"/>.
    ///<p/>
    /// 📎 Spans cannot be added or removed using this method. To do that, use <see cref="Add"/>, <see cref="RemoveAt"/>, etc.
    /// </summary>
    /// <param name="spanIndex">the index of the span to be replaced</param>
    /// <param name="span">the <see cref="ReadOnlySpan{T}"/> to be placed at <paramref name="spanIndex"/></param>
    /// <returns>a new <see cref="RoMultiSpan{T}"/></returns>
    public RoMultiSpan<T> WithSpan(int spanIndex, ReadOnlySpan<T> span) {
        SpanCount.RequireIndex(spanIndex);

        return spanIndex switch {
            0 => this with { _a = span },
            1 => this with { _b = span },
            2 => this with { _c = span },
            3 => this with { _d = span },
            4 => this with { _e = span },
            5 => this with { _f = span },
            6 => this with { _g = span },
            7 => this with { _h = span },
            _ => throw new ArgumentOutOfRangeException(nameof(spanIndex))
        };
    }

    /// <inheritdoc cref="WithSpan(int,System.ReadOnlySpan{T})"/>
    public RoMultiSpan<T> WithSpan(Index spanIndex, ReadOnlySpan<T> span) => WithSpan(spanIndex.GetOffset(SpanCount), span);

    /// <summary>
    /// <c>init</c>-able access to the <see cref="ReadOnlySpan{T}"/> at index <see cref="Index.Start"/> (<c>0</c>).
    /// </summary>
    public ReadOnlySpan<T> First {
        get => this[0];
        init => _a = value;
    }

    /// <summary>
    /// <c>init</c>-able access to the <see cref="ReadOnlySpan{T}"/> at index <c>^1</c>.
    /// </summary>
    public ReadOnlySpan<T> Last {
        get => this[^1];
        init => this[^1] = value;
    }

    public RoMultiSpan<T> Skip_naive(int amountToSkip) {
        var builder = ToBuilder();

        for (int i = 0; i < SpanCount; i++) {
            builder[i]   =  this[i].Skip(amountToSkip);
            amountToSkip -= this[i].Length;
            if (amountToSkip <= 0) {
                break;
            }
        }

        return builder.Build();
    }

    public RoMultiSpan<T> Skip(int amountToSkip) {
        var builder = CreateBuilder();

        foreach (var span in this) {
            if (span.Length > amountToSkip) {
                builder.Add(span.Skip(amountToSkip));
            }

            amountToSkip -= span.Length;
        }

        return builder.Build();
    }

    public RoMultiSpan<T> SkipLast(int amountToSkip) {
        var builder = CreateBuilder();

        for (int i = SpanCount - 1; i >= 0; i--) {
            var span = this[i];
            if (span.Length > amountToSkip) {
                builder.SafeSet(i, span.SkipLast(amountToSkip));
            }

            amountToSkip -= span.Length;
        }

        return builder.Build();
    }
}