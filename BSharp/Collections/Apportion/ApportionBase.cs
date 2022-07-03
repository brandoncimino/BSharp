using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// A remarkably over-complicated class that provides the base implementation for the different <see cref="Apportion"/> types.
/// </summary>
/// <typeparam name="PORTION">the type of each of the output <see cref="Portions"/></typeparam>
/// <typeparam name="SOURCE">the type of the original <see cref="Source"/></typeparam>
/// <typeparam name="SOURCE_ELEMENT">the type of each element in the original <see cref="Source"/></typeparam>
public abstract record ApportionBase<PORTION, SOURCE, SOURCE_ELEMENT> : WrappedImmutableCollection<PORTION, ImmutableList<PORTION>>, IEnumerable
    where SOURCE : IList<SOURCE_ELEMENT> {
    public                 SOURCE                  Source  { get; }
    public                 IEnumerable<double>     Weights { get; }
    private                ImmutableList<PORTION>? _portions;
    public                 ImmutableList<PORTION>  Portions => _portions ??= GetPortions().ToImmutableList();
    public sealed override ImmutableList<PORTION>  Value    => Portions;

    private double? _totalWeight;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected ApportionBase(SOURCE source, IEnumerable<double> weights) {
        Source  = source;
        Weights = weights;
    }

    public double TotalWeight => _totalWeight ??= Weights.Sum();

    protected abstract IEnumerable<PORTION> GetPortions();
    public virtual     PORTION              GetPortion(int index) => Portions[index];

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override ImmutableList<PORTION> Slice(int start, int end) => Value.Slice(start, end);
}