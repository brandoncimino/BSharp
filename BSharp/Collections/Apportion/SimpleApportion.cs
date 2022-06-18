using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// Separates an <see cref="int"/> into multiple parts proportional to each of the <see cref="ApportionBase{PORTION,SOURCE,SOURCE_ELEMENT}.Weights"/>.
/// </summary>
public sealed record SimpleApportion : ApportionBase<int, Indexes, int> {
    public SimpleApportion(Indexes amountToDistribute, params double[]     weights) : this(amountToDistribute, weights.ToImmutableList()) { }
    public SimpleApportion(Indexes source,             IEnumerable<double> weights) : base(source, weights) { }

    #region Overrides of ApportionBase<int,Indexes,int>

    protected override IEnumerable<int> GetPortions() {
        var ranges = new RangeApportion(Source, Weights);
        return ranges.Select(it => it.End.Value - it.Start.Value);
    }

    #endregion
}