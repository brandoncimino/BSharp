using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp;

public static class Brandon {
    public static KeyValuePair<string, object?> Info(object? value, [CallerArgumentExpression("value")] string? expression = default) {
        return new KeyValuePair<string, object?>(expression ?? "🤷‍", value);
    }

    public static void Print(object? value, [CallerArgumentExpression("value")] string? expression = default) {
        var info = value is KeyValuePair<string, object?> kvp ? kvp : Info(value, expression);
        Console.WriteLine($"{info.Key.ForceToLength(50)} {info.Value.OrNullPlaceholder()}");
    }
}