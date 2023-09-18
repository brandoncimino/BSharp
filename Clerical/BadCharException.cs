using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Memory.Enumerators;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Validated;

public class BadCharException : BrandonException {
    internal static void MarkBadChars(ReadOnlySpan<char> input, ReadOnlySpan<char> badChars, Span<char> destination) {
        destination.RequireSpace(0, input.Length);
        destination.Clear();

        foreach (var index in new IndexOfAnyEnumerator<char>(input, badChars)) {
            var badChar = input[index];
            if (char.IsWhiteSpace(badChar)) {
                badChar = '⁐';
            }

            destination[index] = badChar;
        }
    }

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

    public static BadCharException Create<TArg>(
        ReadOnlySpan<char>                             source,
        TArg                                           arg,
        [RequireStaticDelegate] Func<char, TArg, bool> badCharPredicate,
        string?                                        description    = default,
        Exception?                                     innerException = default,
        [CallerArgumentExpression("source")] string?   _source        = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        var msg = GetBaseMessage(source, arg, badCharPredicate, _source, _badCharPredicate);
        return new BadCharException(msg, description, innerException);
    }

    public static BadCharException Create(
        ReadOnlySpan<char>                           source,
        [RequireStaticDelegate] Func<char, bool>     badCharPredicate,
        string?                                      description    = default,
        Exception?                                   innerException = default,
        [CallerArgumentExpression("source")] string? _source        = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        var msg = GetBaseMessage(source, badCharPredicate, static (c, pred) => pred(c), _source, _badCharPredicate);
        return new BadCharException(msg, description);
    }

    private static string GetBaseMessage(ReadOnlySpan<char> source, ReadOnlySpan<char> badChars, string? _source, string? predicateDescription) {
        return GetBaseMessage(source, badChars.ToArray(), static (c, bad) => bad.Contains(c), _source, predicateDescription);
    }

    private static string GetBaseMessage<TArg>(
        ReadOnlySpan<char>                             source,
        TArg                                           arg,
        [RequireStaticDelegate] Func<char, TArg, bool> badCharPredicate,
        string?                                        _source,
        string?                                        _badCharPredicate
    ) {
        Span<char> line2 = stackalloc char[source.Length];

        for (int i = 0; i < source.Length; i++) {
            var c = source[i];
            line2[i] = (badCharPredicate(c, arg), char.IsWhiteSpace(c)) switch {
                (false, _)    => ' ',
                (true, true)  => '⁐',
                (true, false) => c
            };
        }

        return new StringBuilder()
               .AppendLine($"{_source} contained {_badCharPredicate}!")
               .AppendLine(source.ToString())
               .AppendLine(line2.ToString())
               .ToString();
    }

    public static void Assert<TArg>(
        ReadOnlySpan<char>                             source,
        TArg                                           arg,
        [RequireStaticDelegate] Func<char, TArg, bool> badCharPredicate,
        string?                                        description = default,
        [CallerArgumentExpression("source")] string?   _source     = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        var exc = TryAssert(source, arg, badCharPredicate, description, _source, _badCharPredicate);
        if (exc != null) {
            throw exc;
        }
    }

    public static void Assert(
        ReadOnlySpan<char>                           source,
        [RequireStaticDelegate] Func<char, bool>     badCharPredicate,
        string?                                      description = default,
        [CallerArgumentExpression("source")] string? _source     = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        var exc = TryAssert(source, badCharPredicate, static (c, del) => del(c), description, _source, _badCharPredicate);
        if (exc != null) {
            throw exc;
        }
    }

    public static void Assert(
        ReadOnlySpan<char>                           source,
        ReadOnlySpan<char>                           unwanted,
        [CallerArgumentExpression("source")] string? _source = default
    ) {
        var found = source.IndexOfAny(unwanted);
        if (found < 0) {
            return;
        }

        Span<char> markedChars = stackalloc char[source.Length];
        MarkBadChars(source, unwanted, markedChars);
        var msg = new StringBuilder()
                  .AppendLine($"Contained illegal characters!")
                  .Append(source)
                  .AppendLine()
                  .Append(markedChars);
        throw new ArgumentException(msg.ToString(), _source);
    }

    public static BadCharException? TryAssert(
        ReadOnlySpan<char>                             source,
        ReadOnlySpan<char>                             badChars,
        [CallerArgumentExpression("source")]   string? _source   = default,
        [CallerArgumentExpression("badChars")] string? _badChars = default
    ) {
        return source.IndexOfAny(badChars) >= 0 ? new BadCharException(source, badChars, _source, _badChars) : default;
    }

    public static BadCharException? TryAssert(
        ReadOnlySpan<char>                           source,
        [RequireStaticDelegate] Func<char, bool>     badCharPredicate,
        string?                                      description = default,
        [CallerArgumentExpression("source")] string? _source     = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        return TryAssert(source, badCharPredicate, static (c, del) => del(c), description, _source, _badCharPredicate);
    }

    public static BadCharException? TryAssert<TArg>(
        ReadOnlySpan<char>                             source,
        TArg                                           arg,
        [RequireStaticDelegate] Func<char, TArg, bool> badCharPredicate,
        string?                                        description = default,
        [CallerArgumentExpression("source")] string?   _source     = default,
        [CallerArgumentExpression("badCharPredicate")]
        string? _badCharPredicate = default
    ) {
        foreach (var c in source) {
            if (badCharPredicate(c, arg)) {
                return Create(source, arg, badCharPredicate, description, _source: _source, _badCharPredicate: _badCharPredicate);
            }
        }

        return default;
    }
}