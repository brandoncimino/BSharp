using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using BenchmarkDotNet.Engines;

using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    public const string RejectIcon = "🚮";
    public const string ReasonIcon = "🙅";
    private const int DefaultValueStringLimit = 30;
    
    public static ArgumentException Reject<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        string? rejectedBy = default,
        [CallerMemberName]
        string?    reason   = default,
        Exception? causedBy = default
    ) {
        return Reject<T, ArgumentException>(
            actualValue,
            parameterName,
            rejectedBy,
            reason,
            causedBy
        );
    }

    private static string _GetRejectionMessage_ReasonFromCallerName<T>(
        T?      actualValue,
        string? parameterName,
        string? rejectedBy,
        [CallerMemberName]
        string? validationMethod = null
    ) {
        var reason = $"must {validationMethod}";
        return _GetRejectionMessage(actualValue, parameterName, rejectedBy, reason);
    }

    private static string _GetRejectionMessage<T>(
        T?      actualValue,
        string? parameterName,
        string? rejectedBy,
        string? reason
    ) {
        reason = reason.IfBlank("<reason not specified 🤷>");
        var valueStr = GetActualValueString(actualValue, DefaultValueStringLimit);
        var lines    = new StringBuilder();
        lines.AppendLine($"{RejectIcon} {rejectedBy} rejected the parameter {parameterName}!");
        lines.AppendLine($"Method:    {rejectedBy}");
        lines.AppendLine($"Parameter: {parameterName}");
        lines.AppendLine($"Actual:    {valueStr}");
        lines.AppendLine($"Reason:    {reason}");
        return lines.ToString();
    }

    public static TExc Reject<T, TExc>(
        T?                                 actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        string? rejectedBy = default,
        [CallerMemberName]
        string? reason = default,
        Exception?              causedBy           = default
    )
        where TExc : Exception {
        var rejectionMessage = _GetRejectionMessage(actualValue, parameterName, rejectedBy, reason);
        return ExceptionUtils.ConstructException<TExc>(rejectionMessage, causedBy);
    }

    private static string GetActualValueString(object? actualValue, int maxLength) {
        var valStr = actualValue switch {
            null => Prettification.DefaultNullPlaceholder,
            string => $"\"{actualValue}\"",
            _      => actualValue.ToString(),
        };

        return valStr.Truncate(maxLength);
    }
}