using System.Globalization;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Strings;

public readonly record struct GraphemeCluster : IHas<string> {
    public string Value { get; } = "";

    private GraphemeCluster(string? value, bool validate) {
        if (value == null) {
            Value = "";
        }

        Value = validate ? value!.MustBe(it => new StringInfo(it).LengthInTextElements == 1) : value!;
    }
}