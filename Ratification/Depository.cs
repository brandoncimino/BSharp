using System.Collections.Immutable;

using FowlFever.BSharp.Optional;

using Implementors;

namespace Ratified;

/// <summary>
/// A specialized collection of multiple <see cref="IRatifier{T}"/>s.
/// <p/>
/// Analogous to <see cref="FowlFever.BSharp.Optional.RapSheet"/>.
/// </summary>
/// <typeparam name="T">the type being ratified</typeparam>
public sealed record Depository<T> : IRatifier<T>, IHasImmutableList<IRatifier<T>>
    where T : notnull {
    public readonly ImmutableArray<IRatifier<T>> Ratifiers;

    public Depository(IEnumerable<IRatifier<T>> ratifiers) {
        Ratifiers = ratifiers.ToImmutableArray();
    }

    public Depository(IRatifier<T> first, IRatifier<T> second, params IRatifier<T>[] more) {
        Ratifiers = more.Prepend(first)
                        .Prepend(second)
                        .ToImmutableArray();
    }

    IImmutableList<IRatifier<T>> IHasImmutableList<IRatifier<T>>.AsImmutableList => Ratifiers;

    public string Description => string.Join('\n', Ratifiers);

    IFailable IRatifier<T>._TryRatify(T? target, string? targetName, string? requiredBy) {
        var results = Ratifiers.Select(it => it.TryRatify(target, targetName, requiredBy));
        return new RapSheet(results);
    }
}