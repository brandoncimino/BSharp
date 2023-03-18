using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Builds regex <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions">character classes</a>, which match a given set of characters.
/// </summary>
/// <remarks>
/// Does not currently support character ranges, i.e. "lowercase letters" which would be the regex <c>[a-z]</c>.
/// </remarks>
/// <example>
/// <ul>
/// <li><c>a</c>, <c>b</c>, or <c>c</c> → <c>[abc]</c> → <c>new RegexCharacterSet(new []{'a','b','c'})</c></li>
/// </ul>
/// </example>
/// <param name="Inclusions"></param>
/// <param name="Exclusions"></param>
[Obsolete($"Should be avoided in favor of [GeneratedRegex]")]
internal sealed record RegexCharacterSet(IEnumerable<char>? Inclusions = default, IEnumerable<char>? Exclusions = default) : RegexBuilder {
    private readonly IImmutableSet<string> _inclusions = EscapeChars(Inclusions).ToImmutableHashSet();
    private readonly IImmutableSet<string> _exclusions = EscapeChars(Exclusions).ToImmutableHashSet();

    private enum RegexCharacterSetStyle { Inclusive, Exclusive, Subtractive }

    private bool HasInclusions => _inclusions.IsNotEmpty();
    private bool HasExclusions => _exclusions.IsNotEmpty();

    private RegexCharacterSetStyle Style => this switch {
        { HasInclusions: true, HasExclusions: true }  => RegexCharacterSetStyle.Subtractive,
        { HasInclusions: true, HasExclusions: false } => RegexCharacterSetStyle.Inclusive,
        { HasInclusions: false, HasExclusions: true } => RegexCharacterSetStyle.Exclusive,
        _                                             => throw new ArgumentException($"Must include 1+ {nameof(Inclusions)} or {nameof(Exclusions)}!"),
    };

    private static IEnumerable<string> EscapeChars(IEnumerable<char>? chars) {
        return chars.OrEmpty()
                    .Select(it => it.ToString())
                    .Select(Regex.Escape);
    }

    protected override string BuildPattern() {
        return Style switch {
            RegexCharacterSetStyle.Inclusive   => $"[{EscapeChars(Inclusions)}]",
            RegexCharacterSetStyle.Exclusive   => $"[^{EscapeChars(Exclusions)}]",
            RegexCharacterSetStyle.Subtractive => $"[{EscapeChars(Inclusions)}-[{EscapeChars(Exclusions)}]]",
            _                                  => throw BEnum.UnhandledSwitch(Style),
        };
    }
}