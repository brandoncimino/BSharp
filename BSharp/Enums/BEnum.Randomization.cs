using System;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Randomization;

namespace FowlFever.BSharp.Enums;

public static partial class BEnum {
    /// <inheritdoc cref="Brandom.Enum{T}"/>
    public static T Random<T>(Random? generator = default, bool includeAliases = false) where T : Enum => GetValues<T>().Random(generator);
}