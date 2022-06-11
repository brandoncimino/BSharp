using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings;

internal sealed record RegexCharacterSet(IEnumerable<char>? Inclusions = default, IEnumerable<char>? Exclusions = default) : RegexBuilder {
    private ImmutableArray<string> _inclusions = EscapeChars(Inclusions).ToImmutableArray();
    private ImmutableArray<string> _exclusions = EscapeChars(Exclusions).ToImmutableArray();

    private enum RegexCharacterSetStyle { Inclusive, Exclusive, Subtractive }

    private bool HasInclusions => _inclusions.IsDefaultOrEmpty == false;
    private bool HasExclusions => _exclusions.IsDefaultOrEmpty == false;

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