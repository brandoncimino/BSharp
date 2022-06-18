using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// Methods that chop up collections of discrete items into smaller pieces.
/// </summary>
public static class Apportion {
    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> into <paramref name="portions"/> as evenly as possible.
    /// </summary>
    /// <param name="amount">the amount to distribute</param>
    /// <param name="portions">the number of even portions</param>
    /// <returns>all of the resulting portions</returns>
    public static IEnumerable<int> Evenly(int amount, int portions) {
        return new SimpleApportion(amount, Enumerable.Repeat<double>(1, portions));
    }

    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> into portions based on their relative <paramref name="weights"/>.
    /// </summary>
    /// <param name="amount">the amount to distribute</param>
    /// <param name="weights">the relative amount allocated to each resulting portion</param>
    /// <returns>all of the resulting portions</returns>
    public static IEnumerable<int> Simply(int amount, IEnumerable<double> weights) {
        return new SimpleApportion(amount, weights);
    }

    /// <summary>
    /// Divides the entries <paramref name="stock"/> into portions based on their relative <paramref name="equities"/>.
    /// </summary>
    /// <param name="stock">the <typeparamref name="T"/> stuff to be distributed</param>
    /// <param name="equities">the relative amount of <paramref name="stock"/> that each shareholder is entitled to</param>
    /// <typeparam name="T">the type of the <paramref name="stock"/></typeparam>
    /// <returns>the distributed <paramref name="stock"/></returns>
    public static IList<IList<T>> Shares<T>(IEnumerable<T> stock, IEnumerable<double> equities) {
        return new ListApportion<T>(stock, equities);
    }

    /// <summary>
    /// Proportionally adjusts a collection of <see cref="double"/>s so that they total 1.  
    /// </summary>
    /// <param name="weights">the original relative amounts</param>
    /// <returns>a collection of <see cref="double"/>s that total 1</returns>
    [PublicAPI]
    public static IEnumerable<double> NormalizeWeights(IEnumerable<double> weights) {
        var eWeights    = weights.AsList();
        var totalWeight = eWeights.Sum();
        foreach (var it in eWeights) {
            yield return it / totalWeight;
        }
    }
}