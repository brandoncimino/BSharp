using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;

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
        [CallerArgumentExpression("source")]   string? sourceExp = default,
        [CallerArgumentExpression("badChars")] string? badExp    = default
    ) : this(GetBaseMessage(source, badChars, sourceExp, badExp), message) { }

    private static string GetBaseMessage(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, string? sourceExp, string? badExp) {
        Span<char> line2 = stackalloc char[source.Length];

        for (int i = 0; i < source.Length; i++) {
            line2[i] = badChars.Contains(source[i]) ? source[i] : ' ';
        }

        return new StringBuilder()
               .AppendLine($"{sourceExp} contained {badExp}!")
               .AppendLine(source.ToString())
               .AppendLine(line2.ToString())
               .ToString();
    }

    public static void Assert(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, [CallerArgumentExpression("source")] string? sourceExp = default, [CallerArgumentExpression("badChars")] string? badExp = default) {
        if (source.ContainsAny(badChars)) {
            throw new BadCharException(source, badChars, sourceExp, badExp);
        }
    }

    public static void Assert(ReadOnlySpan<char> source, ImmutableArray<char> badChars, [CallerArgumentExpression("source")] string? sourceExp = default, [CallerArgumentExpression("badChars")] string? badExp = default) => Assert(source, badChars.AsSpan(), sourceExp, badExp);
}