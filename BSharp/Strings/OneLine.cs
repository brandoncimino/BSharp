using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
public readonly record struct OneLine : IHas<string>, IEnumerable<GraphemeCluster>, IEquivalent<string> {
    #region "Constants"

    public static readonly OneLine Empty          = CreateRisky("");
    public static readonly OneLine Space          = CreateRisky(GraphemeCluster.Space);
    public static readonly OneLine Ellipsis       = CreateRisky(GraphemeCluster.Ellipsis);
    public static readonly OneLine Hyphen         = CreateRisky(GraphemeCluster.Hyphen);
    private const          string  LineBreakChars = "\n\r";

    #endregion

    public string Value => _stringInfo?.StringSource ?? "";
    [MemberNotNullWhen(false, nameof(_stringInfo))]
    public bool IsEmpty => Length == 0;
    public bool IsBlank => IsEmpty || _stringInfo.All(it => it.IsBlank);
    /// <summary>
    /// ⚠ Default values in struct fields are completely ignored unless you explicitly call <c>new OneLine()</c> (as opposed to the <c>default</c> keyword).
    /// This means that <i>all</i> reference types in struct fields can be null.
    /// <p/>
    /// We should use the <see cref="System.Diagnostics.CodeAnalysis.MaybeNullAttribute"/> to treat the fields as nullable on <i><b>read</b></i> while still preventing us
    /// from <i><b>setting</b></i> them to <c>null</c>.
    ///
    /// <ul>See:
    /// <li><a href="https://stackoverflow.com/a/72843696/18494923">How to tell C# that a struct's non-nullable fields may actually be null for nullability-analysis purposes?</a></li>
    /// <li><a href="https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references#known-pitfalls">Nullable references # Known pitfalls</a></li>
    /// <li><a href="https://stackoverflow.com/questions/58425298/why-dont-i-get-a-warning-about-possible-dereference-of-a-null-in-c-sharp-8-with">Why don't I get a warning about possible dereference of a null in C# 8 with a class member of a struct?</a></li>
    /// </ul>
    /// </summary>
    /// <remarks>
    /// TODO: Add these notes to a documentation file.
    /// </remarks>
    [MaybeNull]
    private readonly TextElementString _stringInfo;

    #region Equals

    public bool Equals(string?              other) => this.ValueEquals(other);
    public bool Equals(IHas<string?>?       other) => this.ValueEquals(other);
    public bool Equals(IEquatable<string?>? other) => this.ValueEquals(other);

    #endregion

    #region Comparison

    public int CompareTo(string?               other) => Comparer<string>.Default.Compare(Value, other!);
    public int CompareTo(IHas<string?>?        other) => Comparer<string>.Default.Compare(Value, other.OrDefault()!);
    public int CompareTo(IComparable<string?>? other) => other?.CompareTo(Value) ?? 1;

    #endregion

    /// <inheritdoc cref="Value"/>
    [Pure]
    public override string ToString() => Value;

    #region Construction

    private static string Validate(string? str) => str.MustNotBeNull()
                                                      .MustNotBe(it => it.ContainsAny(LineBreakChars));

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

        _stringInfo = new TextElementString(value!);
    }

    private OneLine(IEnumerable<GraphemeCluster> value, ShouldValidate shouldValidate) {
        if (value is OneLine line) {
            _stringInfo = line._stringInfo ?? TextElementString.Empty;
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

    [Pure] internal static OneLine CreateRisky(IEnumerable<GraphemeCluster> value)  => new(value, ShouldValidate.No);
    [Pure] internal static OneLine CreateRisky(params GraphemeCluster[]     values) => CreateRisky(values.AsEnumerable());

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
            _            => Create(value.OrDefault())
        };
    }

    #endregion

    /// <summary>
    /// <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/>s together multiple <see cref="OneLine"/>s.
    /// </summary>
    /// <param name="parts">a collection of <see cref="OneLine"/>s</param>
    /// <param name="joiner">an optional <see cref="OneLine"/> interposed betwixt each <see cref="OneLine"/></param>
    /// <param name="prefix"></param>
    /// <param name="suffix"></param>
    /// <returns>a new <see cref="OneLine"/></returns>
    /// <remarks>
    /// Since we know that all of the components are themselves <see cref="OneLine"/>, we can use <see cref="CreateRisky(string)"/> to skip re-validating them.
    /// </remarks>
    [Pure]
    public static OneLine FlatJoin(IEnumerable<OneLine> parts, OneLine? joiner = default, OneLine? prefix = default, OneLine? suffix = default) {
        using var iterator = parts.GetEnumerator();

        if (iterator.MoveNext() == false) {
            return Empty;
        }

        var clusters = ImmutableList.CreateBuilder<GraphemeCluster>();

        if (prefix != null) {
            clusters.AddRange(prefix);
        }

        clusters.AddRange(iterator.Current);

        while (iterator.MoveNext()) {
            if (joiner != null) {
                clusters.AddRange(joiner);
            }

            clusters.AddRange(iterator.Current);
        }

        if (suffix != null) {
            clusters.AddRange(suffix);
        }

        return CreateRisky(clusters.ToImmutable());
    }

    /// <inheritdoc cref="FlatJoin(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.OneLine},System.Nullable{FowlFever.BSharp.Strings.OneLine},System.Nullable{FowlFever.BSharp.Strings.OneLine},System.Nullable{FowlFever.BSharp.Strings.OneLine})"/>
    [Pure]
    public static OneLine FlatJoin(params OneLine[] parts) => FlatJoin(parts.AsEnumerable());

    /// <summary>
    /// <inheritdoc cref="GraphemeClusterExtensions.VisibleLength(string?)"/>
    /// </summary>
    [Pure]
    public int Length => _stringInfo?.Count ?? 0;

    #region Operators

    [Pure] public static implicit operator string(OneLine             line)         => line.Value;
    [Pure] public static implicit operator Lines(OneLine              line)         => new(line);
    [Pure] public static implicit operator Indexes(OneLine            line)         => line.Length;
    [Pure] public static                   OneLine operator +(OneLine a, OneLine b) => CreateRisky(a.Value + b.Value);

    #endregion

    /// <summary>
    /// Retrieves the <see cref="GraphemeCluster"/> at the given <paramref name="textElementIndex"/>.
    /// </summary>
    /// <param name="textElementIndex">the index of the <see cref="GraphemeCluster"/></param>
    [Pure]
    public GraphemeCluster this[int textElementIndex] =>
        IsEmpty
            ? throw new IndexOutOfRangeException(
                  $"Index {textElementIndex} is out-of-bounds because this {nameof(OneLine)} is empty!"
              )
            : _stringInfo[textElementIndex];

    /// <summary>
    /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#implicit-range-support">Implicit Range support</a>
    /// </summary>
    public OneLine Slice(int start, int length) => CreateRisky((_stringInfo ?? TextElementString.Empty).Slice(start, length));

    public IEnumerator<GraphemeCluster> GetEnumerator() => _stringInfo.MustNotBeNull().GetEnumerator();
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

    /// <summary>
    /// Cuts this <see cref="OneLine"/> down to the given <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">settings that inform the <see cref="Truncation"/></param>
    /// <returns>a new <see cref="OneLine"/> with a <see cref="Length"/> <![CDATA[<=]]> <see cref="FillerSettings.LineLengthLimit"/></returns>
    [Pure]
    public OneLine Truncate(FillerSettings settings) {
        return Truncate(settings.LineLengthLimit, settings);
    }

    /// <inheritdoc cref="GraphemeClusterExtensions.RepeatToLength(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.GraphemeCluster},int)"/>
    [Pure]
    public OneLine RepeatToLength(int desiredLength) => CreateRisky(Must.NotBeEmpty(_stringInfo).RepeatToLength(desiredLength));

    /// <summary>
    /// <inheritdoc cref="Fill(FowlFever.BSharp.Strings.FillerSettings?)"/>
    /// </summary>
    /// <param name="desiredLength">see <see cref="FillerSettings.LineLengthLimit"/></param>
    /// <param name="padString">see <see cref="FillerSettings.PadString"/></param>
    /// <param name="settings">additional <see cref="FillerSettings"/></param>
    /// <returns>a <see cref="OneLine"/> with a <see cref="Length"/> of at least <paramref name="desiredLength"/></returns>
    [Pure]
    public OneLine Fill(int desiredLength, OneLine padString, FillerSettings? settings = default) {
        return Fill(settings.Resolve() with { LineLengthLimit = desiredLength, PadString = padString });
    }

    /// <summary>
    /// Adds <see cref="FillerSettings.PadString"/> to this <see cref="Value"/> in order to reach <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">controls how the fill should be performed</param>
    /// <returns><see cref="OneLine"/> with a <see cref="Length"/> of <b><i>at least</i></b> <see cref="FillerSettings.LineLengthLimit"/></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an  unknown <see cref="StringAlignment"/> is provided</exception>
    /// <remarks>
    /// This method is purely additive, meaning that the output will always contain <b><i>at least</i></b> the original <see cref="Value"/>.
    /// If the <see cref="Length"/> is already greater than <see cref="FillerSettings.LineLengthLimit"/>, then no changes will be applied.
    /// <p/>
    /// The "destructive" aka "subtractive" equivalent is <see cref="Truncate(int,FowlFever.BSharp.Strings.FillerSettings?)"/>.
    /// </remarks>
    [Pure]
    public OneLine Fill(FillerSettings settings) {
        var fillerLength = settings.LineLengthLimit - Length;

        if (fillerLength <= 0) {
            return this;
        }

        var filler = settings.PadString.IsEmpty ? this : settings.PadString;

        (int left, int right) fillAmounts = settings.Alignment switch {
            StringAlignment.Left   => (0, fillerLength),
            StringAlignment.Right  => (fillerLength, 0),
            StringAlignment.Center => fillerLength.Bisect(settings.LeftSideRounding),
            _                      => throw BEnum.UnhandledSwitch(settings.Alignment),
        };

        var rightFill = filler.RepeatToLength(fillAmounts.right);
        var leftFill  = fillAmounts.left == fillAmounts.right ? rightFill : filler.RepeatToLength(fillAmounts.left);
        leftFill = settings.MirrorPadding.ApplyTo(leftFill);
        return OneLine.FlatJoin(leftFill, this, rightFill);
    }

    /// <summary>
    /// Either <see cref="Fill(int,FowlFever.BSharp.Strings.OneLine,FowlFever.BSharp.Strings.FillerSettings?)"/> or <see cref="Truncate(int,FowlFever.BSharp.Strings.FillerSettings?)"/>s
    /// this <see cref="OneLine"/> so that it reaches <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">determines how the alignment should be performed</param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="FillerSettings.OverflowStyle"/> is provided</exception>
    [Pure]
    public OneLine Fit(FillerSettings? settings) {
        settings = settings.Resolve();

        var comparisonResult = Length.ComparedWith(settings.LineLengthLimit);
        return comparisonResult switch {
            ComparisonResult.LessThan    => Fill(settings),
            ComparisonResult.EqualTo     => this,
            ComparisonResult.GreaterThan => HandleOverflow(this, settings),
            _                            => throw BEnum.UnhandledSwitch(comparisonResult),
        };

        static OneLine HandleOverflow(OneLine og, FillerSettings settings) {
            return settings.OverflowStyle switch {
                OverflowStyle.Overflow => og,
                OverflowStyle.Truncate => og.Truncate(settings),
                OverflowStyle.Wrap     => throw BEnum.NotSupported(settings.OverflowStyle, $"Because this returns a {nameof(OneLine)}, {nameof(OverflowStyle)}.{OverflowStyle.Wrap} the result would be equivalent to {OverflowStyle.Truncate}."),
                _                      => throw BEnum.UnhandledSwitch(settings.OverflowStyle),
            };
        }
    }
}