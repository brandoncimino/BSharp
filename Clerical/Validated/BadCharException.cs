using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical.Validated;

public class BadCharException : BrandonException {
    protected override string BaseMessage { get; }

    protected BadCharException(string baseMessage, string? message = default, Exception? innerException = null) : base(message, innerException) {
        BaseMessage = baseMessage;
    }

    protected BadCharException(SerializationInfo info, StreamingContext context, string baseMessage) : base(info, context) {
        BaseMessage = baseMessage;
    }

    public BadCharException(
        ReadOnlySpan<char>                             source,
        ReadOnlySpan<char>                             badChars,
        string?                                        message   = default,
        [CallerArgumentExpression("source")]   string? _source   = default,
        [CallerArgumentExpression("badChars")] string? _badChars = default
    ) : this(GetBaseMessage(source, badChars, _source, _badChars), message) { }

    private static string GetBaseMessage(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, string? _source, string? _badChars) {
        Span<char> line2 = stackalloc char[source.Length];

        for (int i = 0; i < source.Length; i++) {
            line2[i] = badChars.Contains(source[i]) ? source[i] : ' ';
        }

        return new StringBuilder()
               .AppendLine($"{_source} contained {_badChars}!")
               .AppendLine(source.ToString())
               .AppendLine(line2.ToString())
               .ToString();
    }

    public static void Assert(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, [CallerArgumentExpression("source")] string? _source = default, [CallerArgumentExpression("badChars")] string? _badChars = default) {
        if (source.ContainsAny(badChars)) {
            throw new BadCharException(source, badChars, _source, _badChars);
        }
    }

    public static void Assert(ReadOnlySpan<char> source, ImmutableArray<char> badChars, [CallerArgumentExpression("source")] string? _source = default, [CallerArgumentExpression("badChars")] string? _badChars = default) {
        Assert(source, badChars.AsSpan(), _source, _badChars);
    }

    public static BadCharException? TryAssert(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, [CallerArgumentExpression("source")] string? _source = default, [CallerArgumentExpression("badChars")] string? _badChars = default) {
        return source.ContainsAny(badChars) ? new BadCharException(source, badChars, _source, _badChars) : default;
    }
}