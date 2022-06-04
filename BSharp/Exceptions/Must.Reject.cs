using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    [Pure]
    public static RejectionException Reject<T>(
        T?      actualValue,
        string? details,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        string?    rejectedBy = default,
        string?    reason     = default,
        Exception? causedBy   = default,
        [CallerMemberName]
        string? caller = default
    ) {
        return new RejectionException(
            actualValue,
            details,
            parameterName,
            rejectedBy,
            reason ?? $"must {caller}",
            causedBy
        );
    }

    [Pure]
    public static RejectionException RejectUnhandledSwitchType<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return new RejectionException(
            actualValue,
            parameterName,
            rejectedBy,
            reason: $"Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!"
        );
    }

    [Pure]
    public static RejectionException RejectWrongType<T>(
        T?   actualValue,
        Type desiredType,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return new RejectionException(
            actualValue,
            parameterName,
            rejectedBy,
            $"Value of type {actualValue?.GetType() ?? typeof(T)} was not {desiredType}!"
        );
    }

    /// <summary>
    /// Indicates that this line of code should have been, logically, impossible to reach.
    /// </summary>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="file">see <see cref="CallerFilePathAttribute"/></param>
    /// <param name="lineNo">see <see cref="CallerLineNumberAttribute"/></param>
    /// <returns>a new <see cref="RejectionException"/></returns>
    [Pure]
    public static RejectionException RejectUnreachable(
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerFilePath]
        string? file = default,
        [CallerLineNumber]
        int? lineNo = default
    ) {
        return new RejectionException($"{file}:line {lineNo}", rejectedBy: rejectedBy, reason: "this code should be unreachable!");
    }
}