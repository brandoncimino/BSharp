using System;
using System.Text.RegularExpressions;

using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// A base class to construct builder-style <c>record</c>s for <see cref="System.Text.RegularExpressions.Regex"/> patterns. 
/// </summary>
public abstract record RegexBuilder : IHas<Regex> {
    private string? _pattern;
    /// <summary>
    /// The <see cref="string"/> representation of the final <see cref="Regex"/>.
    /// </summary>
    public string Pattern => _pattern ??= BuildPattern();

    private readonly TimeSpan _matchTimeout = DefaultMatchTimeout;

    /// <inheritdoc cref="System.Text.RegularExpressions.Regex.MatchTimeout"/>
    public TimeSpan MatchTimeout {
        get => _matchTimeout;
        init {
            _matchTimeout = value;
            Expire();
        }
    }

    private readonly RegexOptions _options = RegexOptions.None;
    /// <inheritdoc cref="System.Text.RegularExpressions.Regex.Options"/>
    public RegexOptions Options {
        get => _options;
        init {
            _options = value;
            Expire();
        }
    }
    private readonly bool _rightToLeft = false;
    /// <inheritdoc cref="System.Text.RegularExpressions.Regex.RightToLeft"/>
    public bool RightToLeft {
        get => _rightToLeft;
        init {
            _rightToLeft = value;
            Expire();
        }
    }

    private void Expire() {
        _regex   = null;
        _pattern = null;
    }

    private static readonly TimeSpan DefaultMatchTimeout = new Regex("").MatchTimeout;

    Regex IHas<Regex>.Value => Regex;

    private Regex? _regex;
    /// <summary>
    /// The final <see cref="System.Text.RegularExpressions.Regex"/> for this <see cref="RegexBuilder"/>.
    /// </summary>
    public Regex Regex => _regex ??= new Regex(Pattern, Options, MatchTimeout);

    protected abstract string BuildPattern();

    public sealed override string ToString() => Pattern;

    public static implicit operator Regex(RegexBuilder builder) => builder.Regex;
}