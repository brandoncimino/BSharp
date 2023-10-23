using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FowlFever.Clerical;

[Flags]
public enum ClericalStyles {
    /// <summary>
    /// The <b>strictest</b> parsing - analogous to <see cref="NumberStyles.None"/>
    /// </summary>
    None = 0,
    /// <summary>
    /// Inputs will be <see cref="string.TrimStart()"/>ed.
    /// </summary>
    AllowLeadingWhiteSpace = 1,
    /// <summary>
    /// Inputs will be <see cref="string.TrimEnd()"/>ed.
    /// </summary>
    AllowTrailingWhiteSpace = 2,
    /// <summary>
    /// Inputs will have <b>up to one</b> leading <see cref="Clerk.IsDirectorySeparator"/> trimmed.
    /// </summary>
    AllowLeadingDirectorySeparator = 4,
    /// <summary>
    /// Inputs will have <b>up to one</b> trailing <see cref="Clerk.IsDirectorySeparator"/> trimmed.
    /// </summary>
    AllowTrailingDirectorySeparator = 8,
    /// <summary>
    /// If enabled, then things that are <b>ARE NOT</b> <see cref="FileExtension"/>s are allowed to contain periods.
    /// </summary>
    /// <example>
    /// Under most circumstances, a period appearing anywhere outside of a <see cref="FileExtension"/> is an error. However, there are some valid use cases:
    /// <ul>
    /// <li>Some applications use "hidden" configuration folders that contain periods, such as <c>.aws</c> in <a href="https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-files.html#cli-configure-files-where">~/.aws/config</a>.</li>
    /// <li>Files with multiple extensions, such as Resharper's <a href="https://www.jetbrains.com/help/resharper/Sharing_Configuration_Options.html#solution-team-shared-layer">{solution name}.sln.DotSettings</a>.</li>
    /// </ul>
    /// </example>
    AllowPeriodsOutsideExtensions = 16,
    /// <summary>
    /// Allows <see cref="FileExtension"/>s to be specified without periods, e.g. as <c>"json"</c> instead of <c>".json"</c>.
    /// </summary>
    AllowExtensionsWithoutPeriods = 32,
    AllowUpperCaseExtensions        = 64,
    AllowEmptyPathParts             = 128,
    AllowBookendWhiteSpace          = AllowLeadingWhiteSpace         | AllowTrailingWhiteSpace,
    AllowBookendDirectorySeparators = AllowLeadingDirectorySeparator | AllowTrailingDirectorySeparator,
    Any                             = int.MaxValue
}

internal static class ClericalStylesExtensions {
    private const ClericalStyles BookendStyles = ClericalStyles.AllowBookendWhiteSpace | ClericalStyles.AllowBookendDirectorySeparators;

    public static string? TryConsumeBookendTrimming(this ref ClericalStyles styles, ref SpanOrSegment s) {
        s      = styles.ApplyBookendTrimming(s);
        styles = styles & ~BookendStyles;
        return CheckForBookends(s);
    }

    /// <summary>
    /// Applies all of the <see cref="BookendStyles"/> present in these <paramref name="styles"/> to the input <paramref name="s"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="ClericalStyles.AllowBookendWhiteSpace"/> is applied before <see cref="ClericalStyles.AllowBookendDirectorySeparators"/>, and each is applied exactly once.
    /// This matches the behavior of <see cref="NumberStyles.AllowTrailingWhite"/> and <see cref="NumberStyles.AllowDecimalPoint"/>, for example:
    /// <code><![CDATA[
    /// int.Parse("1.",  NumberStyles.Any); // => 1
    /// int.Parse("1. ", NumberStyles.Any); // => 1
    /// int.Parse("1 .", NumberStyles.Any); // => ❌ FormatException 
    /// ]]></code> 
    /// </remarks>
    /// <param name="styles">these <see cref="ClericalStyles"/></param>
    /// <param name="s">the input being parsed</param>
    /// <returns>the input after all appropriate <see cref="ClericalStyles"/> have been applied</returns>
    public static SpanOrSegment ApplyBookendTrimming(this ClericalStyles styles, SpanOrSegment s) {
        // Optimize for the 2 most common use cases: "trim everything" and "trim nothing".
        return (styles & BookendStyles) switch {
            0             => s,
            BookendStyles => ConsumeAll(s),
            _             => ConsumeSome(styles, s)
        };

        static SpanOrSegment ConsumeAll(SpanOrSegment s) {
            s = s.Trim();
            s = ParseHelpers.TrimBookendDirectorySeparators(s);
            return s;
        }

        [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
        static SpanOrSegment ConsumeSome(ClericalStyles styles, SpanOrSegment s) {
            if (s.IsEmpty) {
                return s;
            }

            s = TrimWhite(styles, s);
            s = TrimSeparators(styles, s);
            return s;

            static SpanOrSegment TrimWhite(ClericalStyles styles, SpanOrSegment s) {
                return (styles & ClericalStyles.AllowBookendWhiteSpace) switch {
                    0                                      => s,
                    ClericalStyles.AllowLeadingWhiteSpace  => s.TrimStart(),
                    ClericalStyles.AllowTrailingWhiteSpace => s.TrimEnd(),
                    _                                      => s.Trim()
                };
            }

            static SpanOrSegment TrimSeparators(ClericalStyles styles, SpanOrSegment s) {
                return (styles & ClericalStyles.AllowBookendDirectorySeparators) switch {
                    0                                              => s,
                    ClericalStyles.AllowLeadingDirectorySeparator  => ParseHelpers.TrimLeadingDirectorySeparator(s),
                    ClericalStyles.AllowTrailingDirectorySeparator => ParseHelpers.TrimTrailingDirectorySeparator(s),
                    _                                              => ParseHelpers.TrimBookendDirectorySeparators(s)
                };
            }
        }
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static string? CheckForBookends(SpanOrSegment s) {
        return s switch {
            []                        => null,
            [var c]                   => CheckChar(c),
            [var first, .., var last] => CheckChar(first) ?? CheckChar(last)
        };

        static string? CheckChar(char c) {
            if (c is '/' or '\\') {
                // TODO: Add some indication that `ClericalStyles` have already been processed
                return "Cannot start or end with a directory separator!";
            }

            if (char.IsWhiteSpace(c)) {
                return "Cannot start or end with whitespace!";
            }

            return null;
        }
    }

    /// <summary>
    /// Removes any of the <paramref name="unsupportedStyles"/> from these <see cref="ClericalStyles"/>.
    /// Prints a message in <see cref="Debug"/> mode if any of the flags were present.  
    /// </summary>
    /// <param name="clericalStyles">these <see cref="ClericalStyles"/></param>
    /// <param name="unsupportedStyles">the <see cref="ClericalStyles"/> that this <paramref name="typeName"/> doesn't support</param>
    /// <param name="typeName">the type we are trying to parse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveUnsupportedStyles(this ref ClericalStyles clericalStyles, ClericalStyles unsupportedStyles, string typeName) {
        Debug.WriteLineIf((clericalStyles & unsupportedStyles) != 0, $"The type {typeName} doesn't support the {nameof(ClericalStyles)} values: {unsupportedStyles}, so they will be ignored.");
        clericalStyles &= ~unsupportedStyles;
    }
}