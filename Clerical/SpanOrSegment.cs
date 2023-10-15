using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Represents either a <see cref="StringSegment"/> or a <see cref="ReadOnlySpan{T}"/>. Used to unify the implementations of <see cref="IParsable{TSelf}"/> and <see cref="ISpanParsable{TSelf}"/>.
/// </summary>
internal readonly ref struct SpanOrSegment {
    public readonly StringSegment?     Segment;
    public readonly ReadOnlySpan<char> Span;

    public char this[int index] => Segment?[index] ?? Span[index];

    internal SpanOrSegment(StringSegment segment) {
        Segment = segment;
        Span    = default;
    }

    internal SpanOrSegment(ReadOnlySpan<char> span) {
        Segment = null;
        Span    = span;
    }

    public int Length => Segment?.Length ?? Span.Length;

    public SpanOrSegment Slice(int start, int length) =>
        Segment switch {
            { } segment => new SpanOrSegment(segment.Subsegment(start, length)),
            _           => new SpanOrSegment(Span.Slice(start, length))
        };

    public SpanOrSegment Trim() => Segment?.Trim() ?? Span.Trim();

    public string? TryGetStringWithoutAllocating() {
        if (Segment == null) {
            return null;
        }

        var seg = Segment.Value;
        if (seg.HasValue == false) {
            return "";
        }

        if (seg.Length == seg.Buffer.Length) {
            return seg.Buffer;
        }

        return null;
    }

    public                          StringSegment      ToStringSegment()      => Segment ?? Span.ToString();
    public                          ReadOnlySpan<char> AsSpan()               => Segment ?? Span;
    public static implicit operator ReadOnlySpan<char>(SpanOrSegment self)    => self.AsSpan();
    public static implicit operator SpanOrSegment(ReadOnlySpan<char> span)    => new(span);
    public static implicit operator SpanOrSegment(StringSegment      segment) => new(segment);
    public static implicit operator SpanOrSegment(string?            s)       => new(s);
}