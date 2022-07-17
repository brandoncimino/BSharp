using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings.Enums;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a <see cref="string"/> that doesn't contain any <see cref="LineBreakChars"/>.
///
/// TODO: What about adding a <see cref="StringMirroring"/> property?
/// TODO: Ensure proper serializability!
/// </summary>
public readonly partial record struct OneLine : IHas<string>, IEnumerable<GraphemeCluster>, IEquivalent<string> {
    #region "Constants"

    public static readonly OneLine Empty          = CreateRisky("");
    public static readonly OneLine Space          = CreateRisky(GraphemeCluster.Space);
    public static readonly OneLine Ellipsis       = CreateRisky(GraphemeCluster.Ellipsis);
    public static readonly OneLine Hyphen         = CreateRisky(GraphemeCluster.Hyphen);
    internal const         string  LineBreakChars = "\n\r";

    #endregion

    public string              Value      => _stringInfo?.StringSource ?? "";
    string IEquivalent<string>.Equivalent => _stringInfo?.StringSource ?? "";
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

    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc cref="Value"/>
    [Pure]
    public override string ToString() => Value;

    #region Construction

    private static string Validate(string? str) => str.MustNotBeNull()
                                                      .MustNotBe(it => it.ContainsAny(LineBreakChars));

    public OneLine(string? value) : this(value, ShouldValidate.Yes) { }

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
    //TODO: rename to `Of`
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

    [Pure] public static implicit operator string(OneLine  line) => line.Value;
    [Pure] public static implicit operator Lines(OneLine   line) => new(line);
    [Pure] public static implicit operator Indexes(OneLine line) => line.Length;

    /// <summary>
    /// <ul>
    /// <li>Structs can be implicit cast to their <see cref="Nullable"/> counterparts, so this covers all combinations of <see cref="OneLine"/> and <see cref="Nullable">OneLine?</see></li>
    /// <li>Coalescing multiple <c>null</c>s into <see cref="Empty"/> aligns with how <c>null</c> <see cref="string"/>s work.</li>
    /// </ul>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [Pure]
    public static OneLine operator +(OneLine? a, OneLine? b) => (a, b) switch {
        (null, null) => default,
        (_, null)    => a.Value,
        (null, _)    => b.Value,
        (_, _)       => CreateRisky(a.Value.Value + b.Value.Value),
    };

    public static OneLine operator *(OneLine? root, int repetitions) => root switch {
        { IsEmpty: false } => Enumerable.Repeat(root.Value, repetitions).JoinOneLine(),
        _                  => default,
    };

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

    public IEnumerator<GraphemeCluster> GetEnumerator() => _stringInfo?.GetEnumerator() ?? Enumerable.Empty<GraphemeCluster>().GetEnumerator();
    IEnumerator IEnumerable.            GetEnumerator() => GetEnumerator();
}