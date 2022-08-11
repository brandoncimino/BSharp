using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

public readonly record struct GraphemeCluster : IHas<string> {
    #region "Constants"

    public static readonly GraphemeCluster Space      = CreateRisky(@" ");
    public static readonly GraphemeCluster Ellipsis   = CreateRisky(@"…");
    public static readonly GraphemeCluster Hyphen     = CreateRisky(@"-");
    public static readonly GraphemeCluster Slash      = CreateRisky(@"/");
    public static readonly GraphemeCluster OtherSlash = CreateRisky(@"\");

    #endregion

    public          string Value      { get; }
    public          bool   IsBlank    => Value.IsBlank();
    public override string ToString() => Value;

    internal static ReadOnlySpan<char> Validate(ReadOnlySpan<char> value) {
        Must.Have(value.IsEmpty, false);
        var enumerator = value.EnumerateTextElements();
        Must.Have(enumerator.MoveNext() && enumerator.MoveNext(), false);
        return value;
    }

    internal static string Validate(string? value) {
        Validate(value.AsSpan());
        return value!;
    }

    internal GraphemeCluster(string? fullString, Range clusterRange, ShouldValidate shouldValidate) {
        Must.NotBeEmpty(fullString);

        fullString = fullString[clusterRange];

        Value = shouldValidate switch {
            ShouldValidate.Yes => Validate(fullString),
            ShouldValidate.No  => fullString,
            _                  => throw BEnum.UnhandledSwitch(shouldValidate),
        };
    }

    public GraphemeCluster(string fullString, Range clusterRange) : this(fullString, clusterRange, ShouldValidate.Yes) { }

    internal GraphemeCluster(ReadOnlySpan<char> span, ShouldValidate shouldValidate) {
        Value = shouldValidate switch {
            ShouldValidate.Yes => Validate(span).ToString(),
            ShouldValidate.No  => span.ToString(),
            _                  => throw BEnum.UnhandledSwitch(shouldValidate)
        };
    }

    public GraphemeCluster(ReadOnlySpan<char> span) : this(span, ShouldValidate.Yes) { }

    internal enum ShouldValidate { Yes, No }

    public static GraphemeCluster Create(string?      value)              => new(value, Range.All, ShouldValidate.Yes);
    public static GraphemeCluster Create(string?      value, Range range) => new(value, range, ShouldValidate.Yes);
    public static GraphemeCluster CreateRisky(string? value)              => new(value, Range.All, ShouldValidate.No);
    public static GraphemeCluster CreateRisky(string? value, Range range) => new(value, range, ShouldValidate.No);

    public static GraphemeCluster? TryCreate(string? value) => TryCreate(value, Range.All);

    public static GraphemeCluster? TryCreate(string? value, Range range) {
        if (value.IsEmpty()) {
            return default(GraphemeCluster);
        }

        value = value[range];

        return value.VisibleLength() == 1 ? CreateRisky(value) : null;
    }
}