using System.Collections.Generic;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// Separates a set of <see cref="Indexes"/> into individual <see cref="Range"/>s that are proportional to the given <see cref="RangeApportion.Weights"/>.
/// </summary>
/// <param name="Source">the original <see cref="Indexes"/> (i.e. an abstract representation of the entries in an <see cref="ICollection{T}"/>)</param>
/// <param name="Weights">the relative size of each of the resulting <see cref="ApportionBase{PORTION,SOURCE,SOURCE_ELEMENT}.Portions"/></param>
public record RangeApportion(Indexes Source, IEnumerable<double> Weights) : ApportionBase<Range, Indexes, int>(Source, Weights) {
    protected override IEnumerable<Range> GetPortions() {
        var amountLeft  = Source.Count;
        var weightCount = Weights.Count();
        var i           = 0;

        foreach (var w in Weights) {
            if (i == weightCount - 1) {
                yield return (Source - amountLeft)..Source.Count;
            }
            else {
                var nextFraction = w / TotalWeight;
                var splerp       = amountLeft.Terp(nextFraction);
                var soFar        = Source - amountLeft;

                yield return soFar..(soFar + splerp.taken);
                amountLeft = splerp.leftovers;
            }

            i++;
        }
    }
}