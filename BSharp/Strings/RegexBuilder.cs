using System.Text.RegularExpressions;

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

    private Regex?    _value;
    Regex IHas<Regex>.Value => Regex;

    /// <summary>
    /// The final <see cref="System.Text.RegularExpressions.Regex"/> for this <see cref="RegexBuilder"/>.
    /// </summary>
    public Regex Regex => _value ??= Options.HasValue
                                         ? new Regex(Pattern, Options.Value)
                                         : new Regex(Pattern);

    public RegexOptions? Options { get; init; }

    protected abstract string BuildPattern();

    public sealed override string ToString() => Pattern;

    public static implicit operator Regex(RegexBuilder builder) => builder.Regex;
}