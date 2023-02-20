using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Exceptions;

public static partial class Reject {
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
        T                          badObj,
        string?                    details       = default,
        [CallerMemberName] string? parameterName = default,
        [CallerMemberName] string? rejectedBy    = default
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
        object                     typeIdentifier,
        IEnumerable<Type>          legalTypes,
        string?                    details       = default,
        string?                    parameterName = default,
        [CallerMemberName] string? rejectedBy    = default
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
        [CallerMemberName] string? rejectedBy = default
    ) {
        return new RejectionException(
            actualValue,
            parameterName,
            rejectedBy,
            reason: $"Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!"
        );
    }

    /// <summary>
    /// Indicates that this line of code should have been, logically, impossible to reach.
    /// </summary>
    /// <param name="details">optional additional details</param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="_filePath">see <see cref="CallerFilePathAttribute"/></param>
    /// <param name="_lineNo">see <see cref="CallerLineNumberAttribute"/></param>
    /// <returns>a new <see cref="RejectionException"/></returns>
    [Pure]
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Values are being passed-through to matching caller info parameters.")]
    public static UnreachableCodeException Unreachable(
        string?                    details   = default,
        [CallerMemberName] string? _caller   = default,
        [CallerFilePath]   string? _filePath = default,
        [CallerLineNumber] int?    _lineNo   = default
    ) {
        return new UnreachableCodeException(details, _caller, _filePath, _lineNo);
    }

    public static RejectionException IndexOutOfRange(
        Index                                       index,
        int                                         collectionSize,
        string?                                     details = default,
        [CallerArgumentExpression("index")] string? _index  = default,
        [CallerMemberName]                  string? _caller = default
    ) {
        return new RejectionException(
            index,
            details,
            _index,
            _caller,
            $"index {LabelWithExpression(index, _index)} is out-of-bounds for a collection of size {collectionSize}!"
        );
    }

    private static string LabelWithExpression<T>(T value, string? expression) {
        var valueString = value?.ToString()?.Trim() ?? "null";

        if (expression == null || expression.Trim() == valueString) {
            return valueString;
        }

        return $"[{expression}: {valueString}]";
    }
}