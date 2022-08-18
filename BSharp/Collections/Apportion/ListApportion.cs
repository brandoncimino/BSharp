using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// Distributes the items in <see cref="ApportionBase{PORTION,SOURCE,SOURCE_ELEMENT}.Source"/> according to a set of relative <see cref="ApportionBase{PORTION,SOURCE,SOURCE_ELEMENT}.Weights"/>.
/// </summary>
/// <param name="Source">the items being distributed</param>
/// <param name="Weights">the relative amounts of each resulting portion</param>
/// <typeparam name="T">the type of the <see cref="ApportionBase{PORTION,SOURCE,SOURCE_ELEMENT}.Source"/> entries</typeparam>
public sealed record ListApportion<T>(IList<T> Source, IEnumerable<double> Weights) :
    ApportionBase<IList<T>, IList<T>, T>(Source, Weights) {
    public ListApportion(IEnumerable<T> source, IEnumerable<double> weights) : this(source.ToImmutableList(), weights.ToImmutableList()) { }
    public ListApportion(IEnumerable<T> source, params double[]     weights) : this(source, weights.AsEnumerable()) { }

    protected override IEnumerable<IList<T>> GetPortions() {
        IEnumerable<Range> rangeApportion = new RangeApportion(new Indexes(Source.Count), Weights);
        return rangeApportion.Select(it => Source.GetSlice(it));
    }
}