using System;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Keeps a <see cref="Name"/> and <see cref="Subexpression"/> together and formats them nicely with different <see cref="RegexGroupStyle"/>s.
    /// </summary>
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
    [PublicAPI]
    public record RegexGroup(RegexGroupStyle Style, string Subexpression, string? Name = default) {
        /// <summary>
        /// The <see cref="System.Text.RegularExpressions.Group.Name"/> of this <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#named_matched_subexpression">named matched subexpression</a>.
        /// </summary>
        /// <remarks>
        /// When a <see cref="RegexGroup"/> is constructed,
        /// <see cref="RegexGroupStyle.Named"/> groups without a <see cref="Name"/> will still be captured, but will be assigned a numbered group in a <see cref="GroupCollection"/>.
        /// <p/>
        /// Other <see cref="RegexGroupStyle"/>s are <b>only</b> captured if they have a <see cref="Name"/> (though they are still matched).
        /// </remarks>
        /// <example><c>number</c> in <c><![CDATA[(?<number>\d{3})]]></c></example>
        public string Name {
            get {
                if (ExplicitName.IsNotBlank()) {
                    return ExplicitName!;
                }

                return RequiresExplicitName(Style) ? throw new InvalidOperationException() : _autoName.Value;
            }
        }

        private Lazy<string> _autoName = new Lazy<string>(() => Guid.NewGuid().ToString());

        public string? ExplicitName { get; } = Name;

        /// <summary>
        /// The <see cref="System.Text.RegularExpressions.Regex"/> that this group will match.
        /// </summary>
        /// <example>
        /// <c>\w+</c> in <c><![CDATA[(?<word>\w+)]]></c>
        /// </example>
        [RegexPattern]
        public string Subexpression { get; init; } = Subexpression;

        /// <summary>
        /// The <see cref="string"/> representation of the final <see cref="Regex"/>.
        /// </summary>
        public string Pattern => _BuildPattern();

        public RegexOptions? Options { get; init; }

        private Regex? _rx;
        /// <summary>
        /// The final <see cref="System.Text.RegularExpressions.Regex"/> for this <see cref="RegexGroup"/>.
        /// </summary>
        public Regex Regex => _rx ?? Options.IfPresentOrElse(
                                  it => new Regex(Pattern, it),
                                  () => new Regex(Pattern)
                              );

        public RegexGroup(string name, string subexpression) : this(RegexGroupStyle.Named, subexpression, name) { }

        public RegexGroup(string name, Regex subexpression) : this(name, subexpression.ToString()) {
            Options = subexpression.Options;
        }

        public RegexGroup(RegexGroupStyle style, Regex subexpression, string? name = default) : this(style, subexpression.ToString(), name) {
            Options = subexpression.Options;
        }

        private static bool RequiresExplicitName(RegexGroupStyle style) => style != RegexGroupStyle.Named;

        private bool   _ShouldCaptureWithoutName() => Style == RegexGroupStyle.Named;
        private string _WithName()                 => $"(?<{Name}>{Subexpression})";
        private string _SansName()                 => _ShouldCaptureWithoutName() ? $"({Subexpression})" : Subexpression;
        private string _CaptureGroup()             => Name == null ? _SansName() : _WithName();

        private string _CapGroup() {
            if (!RequiresExplicitName(Style) || ExplicitName.IsNotBlank()) {
                return _WithName();
            }
            else {
                return _SansName();
            }
        }

        private string _BuildPattern() {
            // var captureGroup = _CaptureGroup();
            var captureGroup = _CapGroup();
            return Style switch {
                RegexGroupStyle.Named              => captureGroup,
                RegexGroupStyle.Lookahead          => $"(?={captureGroup})",
                RegexGroupStyle.NegativeLookahead  => $"(?!{captureGroup})",
                RegexGroupStyle.Lookbehind         => $"(?<={captureGroup})",
                RegexGroupStyle.NegativeLookbehind => $"(?<!{captureGroup})",
                _                                  => throw BEnum.InvalidEnumArgumentException(nameof(Style), Style)
            };
        }

        public override string ToString() {
            return Pattern;
        }

        /// <param name="name"><see cref="Name"/></param>
        /// <param name="subexpression"><see cref="Subexpression"/></param>
        /// <returns>a <see cref="RegexGroupStyle.Named"/> <see cref="RegexGroup"/></returns>
        public static RegexGroup Named(string name, string subexpression) {
            return new RegexGroup(RegexGroupStyle.Named, subexpression, name);
        }

        /// <param name="subexpression"><see cref="Subexpression"/></param>
        /// <param name="isPositive">if <c>false</c>, <see cref="RegexGroupStyle.NegativeLookahead"/> is used instead of <see cref="RegexGroupStyle.Lookahead"/></param>
        /// <returns>a <see cref="RegexGroupStyle.Lookahead"/> or <see cref="RegexGroupStyle.NegativeLookahead"/> <see cref="RegexGroup"/></returns>
        public static RegexGroup Lookahead(string subexpression, bool isPositive = true) {
            return new RegexGroup(isPositive ? RegexGroupStyle.Lookahead : RegexGroupStyle.NegativeLookahead, subexpression);
        }

        /// <param name="subexpression"><see cref="Subexpression"/></param>
        /// <param name="isPositive">if <c>false</c>, <see cref="RegexGroupStyle.NegativeLookbehind"/> is used instead of <see cref="RegexGroupStyle.Lookbehind"/></param>
        /// <returns>a <see cref="RegexGroupStyle.Lookbehind"/> or <see cref="RegexGroupStyle.NegativeLookbehind"/> <see cref="RegexGroup"/></returns>
        public static RegexGroup Lookbehind(string subexpression, bool isPositive = true) {
            return new RegexGroup(isPositive ? RegexGroupStyle.Lookbehind : RegexGroupStyle.NegativeLookbehind, subexpression);
        }
    }
}