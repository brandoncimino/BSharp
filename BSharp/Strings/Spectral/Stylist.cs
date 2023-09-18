using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Enums;

using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// An alternative to <see cref="Spectre.Console.Style"/> with several improvements:
/// <ul>
/// <li>Is a <c>record</c> type, enabling <c>with</c> expressions</li>
/// <li>Is a <c>struct</c>, enabling the <c>default</c> keyword instead of <see cref="Style.Plain"/></li>
/// <li>Has <see cref="System.Nullable{T}"/> members, avoiding ambiguous <see cref="Style.Combine"/> behavior with <see cref="Color.Default"/> and <see cref="Spectre.Console.Decoration.None"/></li>
/// <li>Can be implicitly cast from <see cref="Spectre.Console.Color"/>, <see cref="Spectre.Console.Decoration"/>, and <see cref="ConsoleColor"/></li> 
/// </ul>
/// </summary>
/// <param name="Foreground">the foreground <see cref="Color"/></param>
/// <param name="Background">the background <see cref="Color"/></param>
/// <param name="Decoration">a combination of <see cref="Spectre.Console.Decoration"/>s, like as <see cref="Spectre.Console.Decoration.Bold"/> or <see cref="Spectre.Console.Decoration.Underline"/></param>
public readonly record struct Stylist(Color? Foreground = default, Color? Background = default, Decoration? Decoration = default) {
    private readonly object? _isDefault = new();
    public           bool    IsDefault => _isDefault == null;

    public Stylist(Decoration decoration) : this(Decoration: decoration) { }

    public Stylist(Color? foreground, Decoration? decoration) : this(foreground, default, decoration) { }

    public Stylist(Style? style) : this(
        DefaultToNull(style?.Foreground),
        DefaultToNull(style?.Background),
        NoneToNull(style?.Decoration)
    ) { }

    private static Color?      DefaultToNull(Color?   color)      => color      == Color.Default ? null : color;
    private static Decoration? NoneToNull(Decoration? decoration) => decoration == Spectre.Console.Decoration.None ? null : decoration;

    #region Conversions

    public Style ToStyle() => new(Foreground, Background, Decoration);

    public static implicit operator Stylist(Color                color)      => new(color);
    public static implicit operator Stylist(Decoration           decoration) => new(decoration);
    public static implicit operator Stylist(Style?               style)      => new(style);
    public static implicit operator Stylist(ConsoleColor         color)      => new(color);
    public static implicit operator Stylist(System.Drawing.Color color)      => new(FromSystemDrawingColor(color));
    public static implicit operator Style(Stylist                stylist)    => stylist.ToStyle();

    private static Color FromSystemDrawingColor(System.Drawing.Color color) {
        return new Color(color.R, color.G, color.B);
    }

    #endregion

    #region Markup strings

    /// <inheritdoc cref="Style.ToMarkup"/>
    public string ToMarkup() => ToStyle().ToMarkup();

    /// <inheritdoc cref="Spectral.ApplyMarkup(Spectre.Console.Style?,string?)"/>
    [return: NotNullIfNotNull("text")]
    public string? ApplyMarkup(string? text) => ToStyle().ApplyMarkup(text);

    #endregion

    #region Ansi Strings

    private bool IsEmpty => IsDefault || (Foreground.HasValue == false && Background.HasValue == false && Decoration.HasValue == false);

    /// <summary>
    /// Invokes the <paramref name="selector"/> against this <see cref="Nullable{T}"/> if it <see cref="Nullable{T}.HasValue"/>.
    /// </summary>
    /// <param name="nullable">this <see cref="Nullable{T}"/></param>
    /// <param name="selector">a <see cref="Func{T,TResult}"/> that produces an <see cref="IEnumerable{T}"/> from a non-null <typeparamref name="T"/></param>
    /// <typeparam name="T">the <see cref="Nullable{T}"/> value type</typeparam>
    /// <typeparam name="T2">the <paramref name="selector"/> output element type</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> of <typeparamref name="T2"/> entries</returns>
    public static IEnumerable<T2> SelectMany<T, T2>(T? nullable, Func<T, IEnumerable<T2>> selector) where T : struct {
        return nullable.HasValue == false ? Enumerable.Empty<T2>() : selector(nullable.Value);
    }

    private IEnumerable<byte> EnumerateForeground(bool on) {
        return (on
                    ? Foreground?.Enumerate_Foreground_On()
                    : Foreground?.Enumerate_Foreground_Off()
               )
               ?? Enumerable.Empty<byte>();
    }

    private IEnumerable<byte> EnumerateBackground(bool on) {
        return (on
                    ? Background?.Enumerate_Background_On()
                    : Background?.Enumerate_Background_Off()
               )
               ?? Enumerable.Empty<byte>();
    }

    private IEnumerable<byte> EnumerateDecoration(bool on) {
        return (on
                    ? Decoration?.EnumerateBytes_On()
                    : Decoration?.EnumerateBytes_Off()
               )
               ?? Enumerable.Empty<byte>();
    }

    private IEnumerable<byte> EnumerateBytes(bool on) {
        return EnumerateForeground(on)
               .Concat(EnumerateBackground(on))
               .Concat(EnumerateDecoration(on));
    }

    public string AnsiString(bool on) => IsEmpty ? "" : Ansi.RenderSequence(EnumerateBytes(on));
    public string AnsiOn()            => AnsiString(true);
    public string AnsiOff()           => AnsiString(false);

    public string ApplyAnsi(string str) {
        return $"{AnsiOn()}{str}{AnsiOff()}";
    }

    #endregion

    #region Merging

    public enum CombinePreference { Self, Other }

    /// <summary>
    /// Combines <c>this</c> with <paramref name="other"/>, choosing which one takes precedence.
    /// </summary>
    /// <param name="other">a different <see cref="Stylist"/></param>
    /// <param name="preference">which <see cref="Stylist"/> we prefer when they both have a value set</param>
    /// <returns>a <see cref="Stylist"/> combining the non-<c>default</c> values from this and <paramref name="other"/></returns>
    public Stylist Merge(Stylist other, CombinePreference preference) {
        if (this == default) {
            return other;
        }

        if (other == default) {
            return this;
        }

        if (this == other) {
            return this;
        }

        return new Stylist(
            Merge(Foreground, other.Foreground, preference),
            Merge(Background, other.Background, preference),
            Merge(Decoration, other.Decoration, preference)
        );
    }

    private static T? Merge<T>(T? self, T? other, CombinePreference preference) {
        return (self, other) switch {
            (null, _) => other,
            (_, null) => self,
            (_, _) => preference switch {
                CombinePreference.Self  => self,
                CombinePreference.Other => other,
                _                       => throw BEnum.UnhandledSwitch(preference),
            }
        };
    }

    /// <summary>
    /// Replaces <c>null</c> properties from <c>this</c> with ones from <paramref name="other"/>.
    /// </summary>
    /// <remarks>
    /// For a similar method that prefers the values in <paramref name="other"/> over ones from <c>this</c>, see <see cref="UpdateWith"/>.
    /// </remarks>
    /// <param name="other">another <see cref="Stylist"/></param>
    /// <returns>a new <see cref="Stylist"/></returns>
    /// <seealso cref="UpdateWith"/>
    public Stylist WithFallback(Stylist other) => Merge(other, CombinePreference.Self);

    /// <summary>
    /// Replaces properties from <c>this</c> with non-<c>null</c> ones from <paramref name="other"/>.
    /// </summary>
    /// <remarks>
    /// This method works similarly to the original <see cref="Style.Combine"/>.
    /// <p/>
    /// For a similar method that prefers the values in <c>this</c> over ones from <paramref name="other"/>, see <see cref="WithFallback"/>.
    /// </remarks>
    /// <param name="other">another <see cref="Stylist"/></param>
    /// <returns>a new <see cref="Stylist"/></returns>
    /// <seealso cref="WithFallback"/>
    public Stylist UpdateWith(Stylist other) => Merge(other, CombinePreference.Other);

    public Stylist IfDefault(Stylist other) => IsDefault ? other : this;

    public Stylist IfDefault(Stylist other, params Stylist[] more) {
        var res = IfDefault(other);
        if (res.IsDefault == false) {
            return res;
        }

        foreach (var it in more) {
            res.IfDefault(it);
            if (res.IsDefault == false) {
                return res;
            }
        }

        return res;
    }

    /// <summary>
    /// <b>Adds</b> the provided <see cref="Spectre.Console.Decoration"/> to this <see cref="Stylist"/>'s <see cref="Decoration"/>.
    /// </summary>
    /// <param name="decoration">an <b>additional</b> <see cref="Spectre.Console.Decoration"/> to be combined with the current <see cref="Decoration"/></param>
    /// <returns>a new <see cref="Stylist"/></returns>
    public Stylist Decorate(Decoration decoration) => this with {
        Decoration = Decoration.GetValueOrDefault() | decoration
    };

    #endregion

    public override string ToString() => ToMarkup();
}