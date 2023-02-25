using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

using Microsoft.Extensions.Logging;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Styling for <see cref="ILogger"/> stuff like <see cref="LogLevel"/>s.
/// </summary>
public record LogPalette {
    /// <summary>
    /// Styling for a specific <see cref="LogLevel"/>.
    /// </summary>
    public readonly record struct LevelPalette {
        public string?                 Label         { get; init; }
        public Stylist?                MyStyle       { get; init; }
        public Func<Palette, Stylist>? FallbackStyle { get; init; }

        private Stylist? GetFallback(Palette? palette) {
            if (FallbackStyle == null || palette == null) {
                return null;
            }

            return FallbackStyle(palette.Value);
        }

        public Stylist GetStyle(Palette? palette = default) {
            return MyStyle ?? GetFallback(palette) ?? default;
        }
    }

    public LogPalette(IEnumerable<KeyValuePair<LogLevel, LevelPalette>> levels) {
        Levels = levels.ToImmutableDictionary();
    }

    public IImmutableDictionary<LogLevel, LevelPalette> Levels { get; init; } = ImmutableDictionary<LogLevel, LevelPalette>.Empty;

    private LevelPalette Get(LogLevel level) {
        return Levels.ContainsKey(level) ? Levels[level] : default;
    }

    public LogPalette Set(LogLevel level, LevelPalette style) {
        return this with {
            Levels = Levels.SetItem(level, style)
        };
    }

    public LogPalette With(LogLevel level, Stylist style) {
        return this with { Levels = Levels.Update(level, style, static (levelPalette, stylist) => levelPalette with { MyStyle = stylist }) };
    }

    public LogPalette With(LogLevel level, Func<Palette, Stylist> fallback) {
        return this with { Levels = Levels.Update(level, fallback, static (lp, fallback) => lp with { FallbackStyle = fallback }) };
    }

    public LevelPalette this[LogLevel level] => Get(level);

    internal static readonly ImmutableDictionary<LogLevel, LevelPalette> HardcodedLevels = new Dictionary<LogLevel, LevelPalette>() {
        [LogLevel.Trace]       = new() { Label = "✒", FallbackStyle  = static p => p.Severity.Unimportant },
        [LogLevel.Debug]       = new() { Label = "🐜", FallbackStyle = static p => p.Severity.Unimportant },
        [LogLevel.Information] = new() { Label = "ℹ", FallbackStyle  = static p => p.LastResort },
        [LogLevel.Warning]     = new() { Label = "⚠", FallbackStyle  = static p => p.Severity.Warning },
        [LogLevel.Error]       = new() { Label = "💣", FallbackStyle = static p => p.Severity.Bad },
        [LogLevel.Critical]    = new() { Label = "🤯", FallbackStyle = static p => p.Severity.Emergency },
    }.ToImmutableDictionary();

    public static readonly LogPalette Hardcoded = new(HardcodedLevels);
}