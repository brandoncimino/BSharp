namespace FowlFever.BSharp.Strings.Enums;

/// <summary>
/// Indicates how <see cref="string"/>s that are too long should be handled.
/// </summary>
/// <remarks>
/// <see cref="OverflowStyle"/>s are meant to be "high-level" options describing things like "do I want this on one line?" and should not dictate
/// any specific formatting.
/// <br/>
/// See individual <see cref="OverflowStyle"/> documentations for what, exactly, they should imply.
/// </remarks>
public enum OverflowStyle {
    /// <summary>
    /// String <see cref="string.Length"/>s are allowed to exceed the given limit.
    /// </summary>
    /// <remarks>
    /// <see cref="Overflow"/> is the <c>default</c> <see cref="OverflowStyle"/>.
    /// There are <b>no</b> strict implementation requirements for an <see cref="Overflow"/> function,
    /// which may or may not modify the original <see cref="string"/> in some way.
    /// </remarks>
    Overflow,
    /// <summary>
    /// Strings will be modified so that their <see cref="string.Length"/>s do not exceed the limit. 
    /// </summary>
    /// <remarks>
    /// A <see cref="Truncate"/> function must:
    /// <ul>
    /// <li>Return a <see cref="string"/> with a <see cref="string.Length"/> that is <see cref="ComparisonOperator.LessThanOrEqualTo"/> the "limit"</li>
    /// </ul>
    /// How the <see cref="string"/> will be modified - such as whether or not a "trail" will be applied to indicate that truncation occurred - is not specified.
    /// </remarks>
    Truncate,
    /// <summary>
    /// Strings will be split into multiple segments, none of which exceed the <see cref="string.Length"/> limit.
    /// </summary>
    /// <remarks>
    /// A <see cref="Wrap"/> function must:
    /// <ul>
    /// <li>Preserve <b>all</b> of the content of the original <see cref="string"/></li>
    /// <li>Return multiple <see cref="string"/> "segments" that:
    ///     <ul>
    ///     <li>Are clearly delimited</li>
    ///     <li>Do not exceed the <see cref="string.Length"/> limit</li>
    ///     </ul>
    /// </li>
    /// </ul>
    /// "Segments" can be delimited in <b>any</b> unambiguous way, such as with line breaks or as individual <see cref="string"/>s.
    /// </remarks>
    Wrap,
}