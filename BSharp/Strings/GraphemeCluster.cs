using System;
using System.Globalization;

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

    private GraphemeCluster(string? value, Range range, ShouldValidate shouldValidate) {
        Must.NotBeEmpty(value);

        value = value[range];

        Value = shouldValidate switch {
            ShouldValidate.Yes => value.MustBe(it => new StringInfo(it).LengthInTextElements == 1),
            ShouldValidate.No  => value,
            _                  => throw BEnum.UnhandledSwitch(shouldValidate),
        };
    }

    private enum ShouldValidate { Yes, No }

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