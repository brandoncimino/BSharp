using System.Collections.Generic;

using FowlFever.BSharp.Memory;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections.Apportion;

/// <summary>
/// Methods that chop up collections of discrete items into smaller pieces.
/// </summary>
public static partial class Apportion {
    public static int[] Weighted<TWeights>(int amount, TWeights weights) where TWeights : IEnumerable<float> {
        scoped Span<float> normalWeights;
        if (weights.TryGetCount(out var count)) {
            normalWeights = stackalloc float[count];
            EnumerateInto(weights, normalWeights);
        }
        else {
            normalWeights = weights.ToArray().AsSpan();
        }

        var results = new int[count];
        NormalizeWeightsInPlace(normalWeights);
        Weighted(amount, normalWeights, results);
        return results;
    }

    public static void Weighted(int amount, ReadOnlySpan<float> weights, Span<int> output) {
        var sum       = weights.FastSum();
        var index     = 0;
        var sumVector = new Vector<float>(sum);
        int remainder = amount;

        if (Vector.IsHardwareAccelerated) {
            var distributedVector = Vector<int>.Zero;

            while (index + Vector<float>.Count <= weights.Length) {
                var weightVector  = weights.NextVector(ref index);
                var portionVector = Vector.ConvertToInt32(weightVector / sumVector * amount);
                PrimitiveMath.CopyTo(portionVector, output[index..]);
                distributedVector += portionVector;
            }

            remainder -= distributedVector.Sum();
        }

        // handle the stuff we couldn't vectorize
        for (; index < weights.Length; index++) {
            var normalWeight = weights[index] / sum;
            var portion      = unchecked((int)(normalWeight * amount));
            output[index] =  portion;
            remainder     -= portion;
        }

        // bung whatever is left into the final index
        output[^1] += remainder;
    }

    private static void EnumerateInto<T>(IEnumerable<T> source, Span<T> destination) {
        var i = 0;
        foreach (var it in source) {
            destination[i] =  it;
            i              += 1;
        }
    }

    public static int[] Weighted(int amount, IEnumerable<float> weights) {
        // if we can convert the weights to span, then this is super easy
        if (Spanq.TryGetReadOnlySpan(weights, out var weightSpan)) {
            return DistributeResults(amount, weightSpan.Length, weightSpan);
        }

        // if we know how _many_ weights we have - even if they haven't been computed yet - then we can still make things fast by allocating a temporary span to store the computed weights
        // 📎 This is probably the most common scenario - for example, you know you have 10 objects, but you haven't extracted their 'Weight' properties yet.
        if (weights.TryGetCount(out var count)) {
            Span<float> weightBuffer = stackalloc float[count];
            EnumerateInto(weights, weightBuffer);
            return DistributeResults(amount, count, weightBuffer);
        }

        // at this point, we must accept that we know very little about the weights, and so we force enumeration into an Array first and THEN try to do stuff fast
        var weightArray = weights.ToArray();
        return DistributeResults(amount, weightArray.Length, weightArray);
    }

    private static int[] DistributeResults(int amount, int count, ReadOnlySpan<float> wBuff) {
        var results = new int[count];
        Weighted(amount, wBuff, results);
        return results;
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
    public static double[] NormalizeWeights(IEnumerable<double> weights) {
        var results     = weights.ToArray();
        var totalWeight = results.Sum();
        _MakeProportional(ref results, totalWeight, default);
        return results;
    }

    private static void _MakeProportional(ref double[] weights, double total, double nonFiniteReplacement) {
        var remaining = 1d;
        for (int i = 0; i < weights.Length; i++) {
            if (i == weights.Length - 1) {
                // Dump whatever is left into the last weight, to guarantee that we have a total of 1 (even if there were rounding errors or something)
                weights[i] = remaining;
                return;
            }

            weights[i] /= total;
            remaining  -= weights[i];
        }
    }

    internal static void NormalizeWeightsInPlace<T>(Span<T> weights) where T : unmanaged {
        var sum = weights.FastSum();
        weights.FastDivideAll(sum);
    }
}