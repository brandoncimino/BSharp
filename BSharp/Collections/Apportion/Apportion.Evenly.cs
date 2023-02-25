using System.Collections.Generic;

namespace FowlFever.BSharp.Collections.Apportion;

public static partial class Apportion {
    #region Evenly

    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> into <paramref name="portions"/> as evenly as possible.
    /// </summary>
    /// <param name="amount">the amount to distribute</param>
    /// <param name="portions">the number of even portions</param>
    /// <param name="strategy">determines how the remainder should be distributed among the portions
    /// <i>(📎 Defaults to <see cref="DefaultStrategy"/>)</i></param>
    /// <returns>all of the resulting portions</returns>
    /// <remarks>
    /// <ul>
    /// <li>
    /// If fairness is more important than efficiency, you can use <see cref="Evenly(int,int,Random)"/> instead, which will distribute the remainder <see cref="Random"/>ly.
    /// </li>
    /// <li>
    /// If efficiency is paramount, you can use <see cref="Evenly(int,Span{int}, DistributionStrategy)"/>, which will distribute the results into a pre-allocated <see cref="Span{T}"/>.
    /// </li>
    /// </ul>
    /// </remarks>
    /// <seealso cref="Evenly(int,int,DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,Span{int},DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,Span{int},Random)"/>
    [Pure]
    public static IEnumerable<int> Evenly(int amount, int portions, DistributionStrategy strategy = DistributionStrategy.Default) {
        var (part, leftover) = amount.DivRem(portions);

        if (leftover == 0) {
            return Enumerable.Repeat(part, portions);
        }

        var results = new int[portions];
        Array.Fill(results, part);
        DistributeRemainder(results, leftover, strategy);
        return results;
    }

    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> into <paramref name="portions"/> as evenly as possible, with the remainder being distributed <see cref="Random"/>ly.
    /// </summary>
    /// <param name="amount">the amount to distribute</param>
    /// <param name="portions">the number of even portions</param>
    /// <param name="remainderDistribution">used to determine which portions get any extras</param>
    /// <returns>all of the resulting portions</returns>
    /// <seealso cref="Evenly(int,int,DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,Span{int},DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,Span{int},Random)"/>
    [Pure]
    public static IEnumerable<int> Evenly(int amount, int portions, Random remainderDistribution) {
        var (part, remainder) = amount.DivRem(portions);

        if (remainder == 0) {
            return Enumerable.Repeat(part, portions);
        }

        var results = new int[portions];
        Array.Fill(results, part);
        DistributeRemainder_Randomly(results, remainder, remainderDistribution);
        return results;
    }

    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> as evenly as possible into each of the given <paramref name="portions"/>.
    /// </summary>
    /// <param name="amount">the amount that will be distributed amongst the <paramref name="portions"/></param>
    /// <param name="portions">the existing values that will each be increased by some of the <paramref name="amount"/></param>
    /// <param name="strategy">determines how the remainder should be distributed if the amount can't be divided evenly</param>
    /// <seealso cref="Evenly(int,int,DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,int,Random)"/>
    /// <seealso cref="Evenly(int,Span{int},Random)"/>
    public static void Evenly(int amount, Span<int> portions, DistributionStrategy strategy = DistributionStrategy.Default) {
        var (part, leftover) = amount.DivRem(portions.Length);
        portions.Fill(part);
        if (leftover != 0) {
            DistributeRemainder(portions, leftover, strategy);
        }
    }

    /// <summary>
    /// Divides an <see cref="int"/> <paramref name="amount"/> as evenly as possible into each of the given <paramref name="portions"/>, with the remainder being distributed <see cref="Random"/>ly.
    /// </summary>
    /// <param name="amount">the amount that will be distributed amongst the <paramref name="portions"/></param>
    /// <param name="portions">the existing values that will each be increased by some of the <paramref name="amount"/></param>
    /// <param name="remainderDistribution">decides which portions get the remainder</param>
    /// <seealso cref="Evenly(int,int,DistributionStrategy)"/>
    /// <seealso cref="Evenly(int,int,Random)"/>
    /// <seealso cref="Evenly(int,Span{int},DistributionStrategy)"/>
    public static void Evenly(int amount, Span<int> portions, Random remainderDistribution) {
        var (part, leftover) = amount.DivRem(portions.Length);
        portions.Fill(part);
        if (leftover != 0) {
            DistributeRemainder_Randomly(portions, leftover, remainderDistribution);
        }
    }

    #endregion
}