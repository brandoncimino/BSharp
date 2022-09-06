using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// A more expressive <see cref = "ArgumentException" /> used by <see cref = "Must" />.
/// </summary>
public class RejectionException : ArgumentException {
    private const           string Shrug      = "🤷";
    private const           string ReasonIcon = "🙅";
    private const           string RejectIcon = "🚮";
    private static readonly string Indent     = new(' ', StringInfo.ParseCombiningCharacters(RejectIcon).Length);
    private const           string NullIcon   = "⛔";
    private const           string NoReason   = $"<reason not specified {Shrug}>";
    private const           string NoRejector = $"<somebody {Shrug}>";

    private const int DefaultValueStringLengthLimit = 30;
    private       int ValueStringLengthLimit { get; init; } = DefaultValueStringLengthLimit;

    public string? ActualValueString {
        get => TruncateString(Data[nameof(ActualValueString)]?.ToString(), ValueStringLengthLimit) ?? NullIcon;
        init => Data[nameof(ActualValueString)] = value;
    }

    public string? RejectedBy {
        get => Data[nameof(RejectedBy)]?.ToString() ?? Source;
        init => Data[nameof(RejectedBy)] = value;
    }

    public string? Reason {
        get => Data[nameof(Reason)]?.ToString();
        init => Data[nameof(Reason)] = value;
    }

    public string? Details {
        get => Data[nameof(Details)]?.ToString();
        init => Data[nameof(Details)] = value;
    }

    private string Preamble => $"`{RejectedBy ?? NoRejector}` rejected `{ParamName}`";

    public override string Message {
        get {
            static IEnumerable<string> _AddLine(IEnumerable<string> lines, string? value, string? label = default) {
                if (string.IsNullOrWhiteSpace(value)) {
                    return lines;
                }

                lines = lines.Append(label != null ? $"{label}: {value}" : value);

                return lines;
            }

            var lines = Enumerable.Empty<string>();

            lines = _AddLine(lines, Preamble);
            lines = _AddLine(lines, Details);
            lines = _AddLine(lines, Reason,            "Reason");
            lines = _AddLine(lines, ActualValueString, "Actual");

            lines = lines.Select(
                (ln, i) => {
                    var indent = i == 0 ? RejectIcon : Indent;
                    return $"{indent} {ln}";
                }
            );

            return string.Join("\n", lines);
        }
    }

    public RejectionException(
        string?                    details        = default,
        Exception?                 innerException = default,
        string?                    reason         = default,
        [CallerMemberName] string? _caller        = default
    ) : base(null, innerException) {
        Details    = details;
        RejectedBy = _caller;
        Reason     = reason;
    }

    public RejectionException(
        object? actualValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? _actualValue = default,
        [CallerMemberName] string? rejectedBy     = default,
        string?                    reason         = default,
        Exception?                 innerException = default
    ) : base(null, _actualValue, innerException) {
        Details           = details;
        ActualValueString = actualValue.OrNullPlaceholder();
        RejectedBy        = rejectedBy;
        Reason            = reason;
    }

    [Pure]
    [return: NotNullIfNotNull("valStr")]
    private static string? TruncateString(string? valStr, int maxLength) {
        if (valStr == null) {
            return null;
        }

        if (valStr.Length <= maxLength) {
            return valStr;
        }

        const string ellipsis    = "…";
        var          truncLength = maxLength - ellipsis.Length;
        return valStr[..truncLength] + ellipsis;
    }
}