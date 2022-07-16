using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public static class Reject {
    private readonly record struct TypeName(string? Value) : IHas<string?> {
        private TypeName(object? obj) : this(
            obj switch {
                Type t   => t.Name,
                string s => s,
                _        => obj?.GetType().Name,
            }
        ) { }

        public static implicit operator TypeName?(string?      name) => name.IsBlank() ? null : new TypeName(name);
        public static implicit operator TypeName?(Type?        type) => type == null ? null : new TypeName(type?.Name);
        public static                   TypeName? From(object? obj)  => obj  == null ? null : new TypeName(obj);
    }

    [Pure]
    private static string GetNotSupportedMessage(TypeName? type, string? details, string? methodName) {
        var detailStr = details.IfNotBlank(it => $": {it}");
        if (methodName.IsBlank()) {
            var typeStr = type?.Value.IfNotBlank(it => $" by {it}");
            return $"🚫 This method is not supported{typeStr}{detailStr}!";
        }
        else {
            var typeStr = type?.Value.IfNotBlank(it => $" {it}.");
            return $"🚫 The method {typeStr}{methodName.AppendIfMissing("()")} is not supported{detailStr}!";
        }
    }

    // TODO: rename to NotSupported
    [Pure]
    public static NotSupportedException Unsupported(object? typeIdentifier = default, string? details = default, [CallerMemberName] string? methodName = default) {
        return new NotSupportedException(GetNotSupportedMessage(TypeName.From(typeIdentifier), details, methodName));
    }

    [Pure]
    public static ReadOnlyException ReadOnly(object? typeIdentifier = default, [CallerMemberName] string? methodName = default) {
        var typeName = TypeName.From(typeIdentifier);
        return new ReadOnlyException(GetNotSupportedMessage(typeName, $"this {typeName} is read-only", methodName));
    }

    [Pure]
    public static NotImplementedException DefaultImplementation(object? typeIdentifier = default, [CallerMemberName] string? methodName = default) {
        return new NotImplementedException(GetNotSupportedMessage(TypeName.From(typeIdentifier), "(default implementation)", methodName));
    }

    [Pure]
    public static RejectionException BadType<T>(
        T       badObj,
        string? details = default,
        [CallerMemberName]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        var typeName = TypeName.From(badObj);
        return new RejectionException(
            typeName,
            details,
            parameterName,
            rejectedBy,
            $"[{typeName}] isn't allowed!"
        );
    }

    [Pure]
    public static RejectionException BadType(
        object            typeIdentifier,
        IEnumerable<Type> legalTypes,
        string?           details       = default,
        string?           parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        var typeName = TypeName.From(typeIdentifier);
        return new RejectionException(
            typeName,
            details,
            parameterName,
            rejectedBy,
            $"[{typeName}] is not one of the valid types: [{legalTypes.JoinString(",", "[", "]")}"
        );
    }

    [Pure]
    public static RejectionException UnhandledSwitchType<T>(
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
}