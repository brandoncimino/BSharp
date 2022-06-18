using System;
using System.Data;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public static class Reject {
    public readonly record struct TypeName(string? Value) : IHas<string?> {
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

    public static NotSupportedException Unsupported(object? type = default, string? details = default, [CallerMemberName] string? methodName = default) {
        return new NotSupportedException(GetNotSupportedMessage(TypeName.From(type), details, methodName));
    }

    public static ReadOnlyException ReadOnly(object? type = default, string? details = default, [CallerMemberName] string? methodName = default) {
        return new ReadOnlyException(GetNotSupportedMessage(TypeName.From(type), details, methodName));
    }
}