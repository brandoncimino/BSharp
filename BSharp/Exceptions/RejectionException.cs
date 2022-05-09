using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// A more expressive <see cref = "ArgumentException" /> used by <see cref = "Must" />.
/// </summary>
public class RejectionException : ArgumentException {
    private const string Shrug                         = "🤷";
    private const string ReasonIcon                    = "🙅";
    private const string RejectIcon                    = "🚮";
    private const string NullIcon                      = "⛔";
    private const string NoReason                      = $"<reason not specified {Shrug}>";
    private const string NoRejector                    = $"<somebody {Shrug}>";
    private const int    DefaultValueStringLengthLimit = 30;

    public object? ActualValue { get; }

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

    public int ValueStringLengthLimit { get; init; } = DefaultValueStringLengthLimit;

    public string Preamble => $"{RejectIcon} `{RejectedBy ?? NoRejector}` rejected `{ParamName}`";

    public override string Message => $"{Preamble}: {Reason ?? NoReason}\nActual: {ActualValueString}";

    public RejectionException(
        object? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        string?    reason         = default,
        Exception? innerException = default
    ) : base(null, parameterName, innerException) {
        ActualValue       = actualValue;
        ActualValueString = actualValue?.ToString();
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