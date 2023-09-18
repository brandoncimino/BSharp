using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Memory.Enumerators;

namespace FowlFever.Clerical;

internal static class BadCharException {
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

    public static void Validate(
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
}