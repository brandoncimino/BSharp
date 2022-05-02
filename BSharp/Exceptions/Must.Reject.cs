using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Reflection;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    [DoesNotReturn]
    public static ArgumentException Reject<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        string?    reason   = default,
        Exception? causedBy = default
    ) {
        return RejectArgument.Because(actualValue, parameterName, rejectedBy, reason);
    }
}