using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Keeps a <see cref="Name"/> and <see cref="Subexpression"/> together and formats them nicely with different <see cref="RegexGroupStyle"/>s.
/// </summary>
/// <param name="Name">the desired <see cref="Name"/>. Defaults to <see cref="CallerMemberNameAttribute"/></param>
/// <remarks>A "named group" is more properly known as a <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#named_matched_subexpression">named matched subexpression</a>.</remarks>
/// <example>
/// The following would match a standard US phone number in the format <c>203-481-1845</c>:
/// <code>
/// <![CDATA[
/// Name            = "phoneNumber"
/// Subexpression   = \d{3}-\d{3}-\d{4}
/// ]]></code>
/// <ul>
/// <li><see cref="Name"/> = <c>"phoneNumber";</c></li>
/// <li><see cref="Subexpression"/> = new <see cref="System.Text.RegularExpressions.Regex"/><c>(@"\d{3}-\d{3}-\d{4}");</c></li>
/// <li><see cref="Regex"/> => <c><![CDATA[(?<phoneNumber>\d{3}-\d{3}-\d{4})]]></c></li>
/// </ul>
/// <code>
/// <see cref="System.Text.RegularExpressions.Regex"/>
/// <![CDATA[
/// (?<area_code>\d{3})-\d{3}-\d{4}
/// ]]>
/// </code>
/// <see cref="Name"/>
/// <code>area_code</code>
/// </example>
[Experimental()]
public sealed record RegexGroup([RegexPattern] string Subexpression, [CallerMemberName] string? Name = default) : RegexBuilder {
    private string? _name = Name;
    /// <summary>
    /// The <see cref="System.Text.RegularExpressions.Group"/> name of this <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#named_matched_subexpression">named matched subexpression</a>.
    /// </summary>
    /// <example><c>number</c> in <c><![CDATA[(?<number>\d{3})]]></c></example>
    public string Name {
        get => _name ??= Guid.NewGuid().ToString();
        init => _name = value;
    }

    /// <summary>
    /// The <see cref="System.Text.RegularExpressions.Regex"/> that this group will match.
    /// </summary>
    /// <example>
    /// <c>\w+</c> in <c><![CDATA[(?<word>\w+)]]></c>
    /// </example>
    [RegexPattern]
    public string Subexpression { get;  init; } = Subexpression;
    public RegexGroupStyle Style { get; init; } = RegexGroupStyle.Named;

    #region Constructors

    /// <inheritdoc cref="RegexGroup"/>
    public RegexGroup([RegexPattern] ReadOnlySpan<char> subexpression, [CallerMemberName] string? name = default) : this(subexpression.ToString(), name) { }

    #endregion

    protected override string BuildPattern() {
        var captureGroup = (string)$"(?<{Name}>{Subexpression})";
        return Style switch {
            RegexGroupStyle.Named              => captureGroup,
            RegexGroupStyle.Lookahead          => $"(?={captureGroup})",
            RegexGroupStyle.NegativeLookahead  => $"(?!{captureGroup})",
            RegexGroupStyle.Lookbehind         => $"(?<={captureGroup})",
            RegexGroupStyle.NegativeLookbehind => $"(?<!{captureGroup})",
            _                                  => throw BEnum.InvalidEnumArgumentException(nameof(Style), Style),
        };
    }

    /// <param name="name"><see cref="Name"/></param>
    /// <param name="subexpression"><see cref="Subexpression"/></param>
    /// <returns>a <see cref="RegexGroupStyle.Named"/> <see cref="RegexGroup"/></returns>
    public static RegexGroup Named(string subexpression, string name) {
        return new RegexGroup(subexpression, name) {
            Style = RegexGroupStyle.Named
        };
    }

    public enum LookDirection { Ahead, Behind }

    public static RegexGroup Looker(ReadOnlySpan<char> subexpression, LookDirection direction, Polarity polarity = Polarity.Positive) {
        var style = (direction, polarity) switch {
            (LookDirection.Ahead, Polarity.Positive)  => RegexGroupStyle.Lookahead,
            (LookDirection.Ahead, Polarity.Negative)  => RegexGroupStyle.NegativeLookahead,
            (LookDirection.Behind, Polarity.Positive) => RegexGroupStyle.Lookbehind,
            (LookDirection.Behind, Polarity.Negative) => RegexGroupStyle.NegativeLookbehind,
            _                                         => throw BEnum.UnhandledSwitch((direction, polarity))
        };

        return new RegexGroup(subexpression) {
            Style = style
        };
    }

    /// <param name="subexpression"><see cref="Subexpression"/></param>
    /// <param name="polarity">whether this is a <see cref="RegexGroupStyle.Lookahead"/> or a <see cref="RegexGroupStyle.NegativeLookahead"/></param>
    /// <returns>a <see cref="RegexGroupStyle.Lookahead"/> or <see cref="RegexGroupStyle.NegativeLookahead"/> <see cref="RegexGroup"/></returns>
    public static RegexGroup Lookahead(ReadOnlySpan<char> subexpression, Polarity polarity = Polarity.Positive) => Looker(subexpression, LookDirection.Ahead, polarity);

    /// <param name="subexpression"><see cref="Subexpression"/></param>
    /// <param name="polarity">whether this is a <see cref="RegexGroupStyle.Lookbehind"/> or a <see cref="RegexGroupStyle.NegativeLookbehind"/></param>
    /// <returns>a <see cref="RegexGroupStyle.Lookbehind"/> or <see cref="RegexGroupStyle.NegativeLookbehind"/> <see cref="RegexGroup"/></returns>
    public static RegexGroup Lookbehind(ReadOnlySpan<char> subexpression, Polarity polarity) => Looker(subexpression, LookDirection.Behind, polarity);
}