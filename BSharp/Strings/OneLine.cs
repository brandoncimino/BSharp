using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings.Enums;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a <see cref="string"/> that doesn't contain any <see cref="LineBreakChars"/>.
///
/// TODO: What about adding a <see cref="StringMirroring"/> property?
/// </summary>
public readonly record struct OneLine : IHas<string>, IEnumerable<GraphemeCluster> {
    #region "Constants"

    public static readonly OneLine Empty          = CreateRisky(GraphemeCluster.Empty);
    public static readonly OneLine Space          = CreateRisky(GraphemeCluster.Space);
    public static readonly OneLine Ellipsis       = CreateRisky(GraphemeCluster.Ellipsis);
    private const          string  LineBreakChars = "\n\r";

    #endregion

    public           string            Value   => _stringInfo.StringSource;
    public           bool              IsEmpty => VisibleLength == 0;
    public           bool              IsBlank => IsEmpty || _stringInfo.All(it => it.IsBlank);
    private readonly TextElementString _stringInfo;

    /// <inheritdoc cref="Value"/>
    [Pure]
    public override string ToString() => Value;

    #region Construction

    private static string Validate(string str) => Must.NotContainAny(str, LineBreakChars);

    private enum ShouldValidate { Yes, No }

    private OneLine(string? value, ShouldValidate shouldValidate) {
        if (string.IsNullOrEmpty(value)) {
            _stringInfo = TextElementString.Empty;
            return;
        }

        value = shouldValidate switch {
            ShouldValidate.Yes => Validate(value),
            ShouldValidate.No  => value,
            _                  => throw BEnum.UnhandledSwitch(shouldValidate)
        };

        _stringInfo = new TextElementString(value);
    }

    private OneLine(IEnumerable<GraphemeCluster> value, ShouldValidate shouldValidate) {
        if (value is OneLine line) {
            _stringInfo = line._stringInfo;
            return;
        }

        _stringInfo = value switch {
            TextElementString text => text,
            _                      => new TextElementString(value),
        };

        var _ = shouldValidate switch {
            ShouldValidate.Yes => Validate(_stringInfo.StringSource),
            ShouldValidate.No  => default,
            _                  => throw BEnum.UnhandledSwitch(shouldValidate)
        };
    }

    /// <summary>
    /// Constructs a new <see cref="OneLine"/> <b>without</b> performing validation. Intended to <b>only</b> to be called after <see cref="string.Split(char[])"/>ting a <see cref="string"/>.
    /// </summary>
    /// <param name="value">the <see cref="string"/> source</param>
    /// <returns>a new <see cref="OneLine"/></returns>
    [Pure]
    internal static OneLine CreateRisky(string value) => new(value, ShouldValidate.No);

    internal static OneLine CreateRisky(IEnumerable<GraphemeCluster> value)  => new(value, ShouldValidate.No);
    internal static OneLine CreateRisky(params GraphemeCluster[]     values) => CreateRisky(values.AsEnumerable());

    /// <summary>
    /// Constructs a new instance of <see cref="OneLine"/>, validating that the provided <see cref="string"/> doesn't contain any <see cref="LineBreakChars"/>.
    /// </summary>
    /// <param name="value">a <see cref="string"/> that doesn't contain any <see cref="LineBreakChars"/></param>
    /// <returns>a new <see cref="OneLine"/></returns>
    /// <exception cref="RejectionException">if the <see cref="string"/> contained <c>\n</c> or <c>\r</c></exception>
    [Pure]
    public static OneLine Create(string? value) => new(value, ShouldValidate.Yes);

    [Pure] public static OneLine Create(IEnumerable<GraphemeCluster> value) => new(value, ShouldValidate.Yes);
    [Pure] public static OneLine Create(OneLine                      value) => value;

    /// <inheritdoc cref="Create(string?)"/>
    [Pure]
    public static OneLine Create(IHas<string?>? value) {
        return value switch {
            OneLine line => line,
            Lines        => throw Must.Reject(value, $"An instance of {nameof(Lines)} can't be converted to a {nameof(OneLine)}!"),
            _            => Create(value.GetValueOrDefault())
        };
    }

    #endregion

    /// <summary>
    /// <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/>s together multiple <see cref="OneLine"/>s.
    /// </summary>
    /// <param name="parts">a collection of <see cref="OneLine"/>s</param>
    /// <param name="joiner">an optional <see cref="OneLine"/> interposed betwixt each <see cref="OneLine"/></param>
    /// <returns>a new <see cref="OneLine"/></returns>
    /// <remarks>
    /// Since we know that all of the components are themselves <see cref="OneLine"/>, we can use <see cref="CreateRisky(string)"/> to skip re-validating them.
    /// </remarks>
    [Pure]
    public static OneLine FlatJoin(IEnumerable<OneLine> parts, OneLine? joiner = default) {
        using var iterator = parts.GetEnumerator();

        if (iterator.MoveNext() == false) {
            return Empty;
        }

        var clusters = ImmutableList.CreateBuilder<GraphemeCluster>();
        clusters.AddRange(iterator.Current);

        while (iterator.MoveNext()) {
            if (joiner != null) {
                clusters.AddRange(joiner);
            }

            clusters.AddRange(iterator.Current);
        }

        return CreateRisky(clusters.ToImmutable());
    }

    /// <inheritdoc cref="FlatJoin(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.OneLine},System.Nullable{FowlFever.BSharp.Strings.OneLine})"/>
    [Pure]
    public static OneLine FlatJoin(params OneLine[] parts) => FlatJoin(parts.AsEnumerable());

    /// <summary>
    /// <inheritdoc cref="GraphemeClusterExtensions.VisibleLength(string?)"/>
    /// </summary>
    [Pure]
    public int VisibleLength => _stringInfo.Count;

    #region Operators

    [Pure] public static implicit operator string(OneLine             line)         => line.Value;
    [Pure] public static implicit operator Lines(OneLine              line)         => new(line);
    [Pure] public static implicit operator Indexes(OneLine            line)         => line.VisibleLength;
    [Pure] public static                   OneLine operator +(OneLine a, OneLine b) => CreateRisky(a.Value + b.Value);

    #endregion

    /// <inheritdoc cref="GraphemeClusterExtensions.ElementAt"/>
    [Pure]
    public GraphemeCluster this[int textElementIndex] => _stringInfo.ElementAt(textElementIndex);

    /// <inheritdoc cref="GraphemeClusterExtensions.SubstringByTextElements"/>
    [Pure]
    public OneLine this[Range range] => CreateRisky(_stringInfo[range]);

    /// <inheritdoc cref="StringInfo.SubstringByTextElements(int,int)"/>
    [Pure]
    public OneLine Substring(int startingTextElement, int lengthInTextElements) => CreateRisky(_stringInfo.Substring(startingTextElement, lengthInTextElements));

    public IEnumerator<GraphemeCluster> GetEnumerator() => _stringInfo.GetEnumerator();
    IEnumerator IEnumerable.            GetEnumerator() => GetEnumerator();

    [Pure]
    public OneLine Truncate(int maxLength, FillerSettings? settings = default) {
        settings = settings.Resolve();
        var truncation = Truncation.PlanTruncation(maxLength, settings);
        return FlatJoin(
            settings.TruncateTrail[truncation.LeftTrail],
            this[truncation.FinalCut],
            settings.TruncateTrail[truncation.RightTrail]
        );
    }

    [Pure]
    public OneLine Truncate(FillerSettings? settings = default) {
        settings = settings.Resolve();
        return Truncate(settings.LineLengthLimit, settings);
    }

    [Pure]
    public OneLine Align(StringAlignment alignment) {
        throw new NotImplementedException();
    }
}