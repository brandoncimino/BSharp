using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace FowlFever.BSharp.Logging;

public static class LogLevelExtensions {
    /// <summary>
    /// Entries in this dictionary will be used for the <see cref="LogLevel"/>'s <see cref="GetLabel"/>, overriding the hard-coded defaults.
    /// <p/>
    /// To use the hard-coded defaults, <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/> the <see cref="LogLevel"/>.
    /// </summary>
    [PublicAPI]
    public static readonly IDictionary<LogLevel, string> Labels = new Dictionary<LogLevel, string>();

    /// <summary>
    /// A concise <see cref="string"/> used to represent this <see cref="LogLevel"/>.
    /// </summary>
    /// <remarks>
    /// This will return hard-coded emoji by default. These can be overridden by modifying the entries in the <see cref="Labels"/> dictionary.
    /// </remarks>
    /// <param name="logLevel">this <see cref="LogLevel"/></param>
    /// <returns>a concise <see cref="string"/> used to represent this <see cref="LogLevel"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">if an unknown <see cref="LogLevel"/> is provided</exception>
    public static string GetLabel(this LogLevel logLevel) {
        return logLevel switch {
            LogLevel.Trace       => Labels.TryGetValue(LogLevel.Trace,       out var label) ? label : "✒",
            LogLevel.Debug       => Labels.TryGetValue(LogLevel.Debug,       out var label) ? label : "🐜",
            LogLevel.Information => Labels.TryGetValue(LogLevel.Information, out var label) ? label : "ℹ",
            LogLevel.Warning     => Labels.TryGetValue(LogLevel.Warning,     out var label) ? label : "⚠",
            LogLevel.Error       => Labels.TryGetValue(LogLevel.Error,       out var label) ? label : "💣",
            LogLevel.Critical    => Labels.TryGetValue(LogLevel.Critical,    out var label) ? label : "💥",
            LogLevel.None        => Labels.TryGetValue(LogLevel.None,        out var label) ? label : "🙅",
            _                    => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}