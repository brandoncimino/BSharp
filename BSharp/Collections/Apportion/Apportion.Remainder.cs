using System.Diagnostics;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp.Collections.Apportion;

public static partial class Apportion {
    // These methods handle the distribution of remainders when an apportion couldn't be done perfectly.

    #region Remainder Distribution

    /// <summary>
    /// The <see cref="DistributionStrategy"/> used when <see cref="DistributionStrategy.Default"/> has been specified.
    /// </summary>
    public static DistributionStrategy DefaultStrategy { get; set; } = DistributionStrategy.FromOutsideSpaced;

    public enum DistributionStrategy {
        /// <summary>
        /// Functions will use whatever <see cref="FowlFever.BSharp.Collections.Apportion.Apportion.DistributionStrategy"/> is currently set as the <see cref="Apportion.DefaultStrategy"/>.
        /// </summary>
        Default,
        /// <summary>
        /// The remainder will be distributed into every-other-outer-most-index, starting from the left.
        /// </summary>
        /// <example>
        /// For 7 items:
        /// <code><![CDATA[
        /// rem. 1 => [1, 0, 0, 0, 0, 0, 0]
        /// rem. 2 => [1, 0, 0, 0, 0, 0, 1]
        /// rem. 3 => [1, 0, 1, 0, 0, 0, 1]
        /// rem. 4 => [1, 0, 1, 0, 1, 0, 1]
        /// rem. 5 => [1, 1, 1, 0, 1, 0, 1]
        /// rem. 6 => [1, 1, 1, 0, 1, 1, 1]
        /// ]]></code>
        /// </example>
        FromOutsideSpaced,
        /// <summary>
        /// Starting from the left, the remainder will be distributed into every other index.
        /// If we still have more after that, the remainder will fill in the rest of the spaces, starting from the right.
        /// </summary>
        /// <example>
        /// For 5 items:
        /// <code><![CDATA[
        /// rem. 1 => [1, 0, 0, 0, 0]
        /// rem. 2 => [1, 0, 1, 0, 0]
        /// rem. 3 => [1, 0, 1, 0, 1]
        /// rem. 4 => [1, 0, 1, 1, 1]
        /// ]]></code>
        /// For 6 items:
        /// rem. 1 => [1, 0, 0, 0, 0, 0]
        /// rem. 2 => [1, 0, 1, 0, 0, 0]
        /// rem. 3 => [1, 0, 1, 0, 1, 0]
        /// rem. 4 => [1, 0, 1, 0, 1, 1]
        /// rem. 5 => [1, 0, 1, 1, 1, 1]
        /// </example>
        FromLeftSpaced,
        /// <summary>
        /// Starting from the left, the remainder will be distributed into each index.
        /// </summary>
        /// <example>
        /// For 5 items:
        /// <code><![CDATA[
        /// rem. 1 => [1, 0, 0, 0, 0]
        /// rem. 2 => [1, 1, 0, 0, 0]
        /// rem. 3 => [1, 1, 1, 0, 0]
        /// rem. 4 => [1, 1, 1, 1, 0]
        /// ]]></code>
        /// </example>
        FromLeft,
        /// <summary>
        /// Starting from the right, the remainder will be distributed into each index.
        /// </summary>
        /// <example>
        /// For 5 items:
        /// <code><![CDATA[
        /// rem. 1 => [0, 0, 0, 0, 1]
        /// rem. 2 => [0, 0, 0, 1, 1]
        /// rem. 3 => [0, 0, 1, 1, 1]
        /// rem. 4 => [0, 1, 1, 1, 1]
        /// ]]></code>
        /// </example>
        FromRight,
        /// <summary>
        /// The remainder will be distributed into the outer-most index, alternating sides, starting with the left.
        /// </summary>
        /// <example>
        /// For 5 items:
        /// <code><![CDATA[
        /// rem. 1 => [1, 0, 0, 0, 0]
        /// rem. 2 => [1, 0, 0, 0, 1]
        /// rem. 3 => [1, 1, 0, 0, 1]
        /// rem. 4 => [1, 1, 0, 1, 1]
        /// ]]></code>
        /// </example>
        FromOutside,
        /// <summary>
        /// The remainder will be distributed into the inner-most index, alternating sides, starting with the left.
        /// </summary>
        FromCenter,
    }

    internal static void DistributeRemainder(Span<int> portions, int remainder, DistributionStrategy strategy) {
        Debug.Assert(remainder       < portions.Length);
        Debug.Assert(remainder       > 0);
        Debug.Assert(portions.Length > 1);

        if (strategy == DistributionStrategy.Default) {
            strategy = DefaultStrategy;
        }

        switch (strategy) {
            case DistributionStrategy.Default: throw new UnreachableCodeException();
            case DistributionStrategy.FromLeft:
                DistributeRemainder_FromLeft(portions, remainder);
                break;
            case DistributionStrategy.FromLeftSpaced:
                DistributeRemainder_FromLeftSpaced(portions, remainder);
                break;
            case DistributionStrategy.FromRight:
                DistributeRemainder_FromRight(portions, remainder);
                break;
            case DistributionStrategy.FromOutside:
                DistributeRemainder_FromOutside(portions, remainder);
                break;
            case DistributionStrategy.FromCenter:
                DistributeRemainder_FromCenter(portions, remainder);
                break;
            case DistributionStrategy.FromOutsideSpaced:
                DistributeRemainder_FromOutsideSpaced(portions, remainder);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }
    }

    internal static void DistributeRemainder_FromLeft(Span<int> portions, int remainder) {
        var getsRemainder = portions[..remainder];
        getsRemainder.FastAddAll(1);
    }

    #region Alternating Vectors

    private static Vector<T> CreateAlternatingVector<T>(bool firstIsOne) where T : unmanaged {
        var ar = new T[Vector<T>.Count];

        var (even, odd) = firstIsOne switch {
            true  => (PrimitiveMath.One<T>(), PrimitiveMath.Zero<T>()),
            false => (PrimitiveMath.Zero<T>(), PrimitiveMath.One<T>())
        };

        for (int i = 0; i < ar.Length; i++) {
            var isEven = i % 2 == 0;
            ar[i] = isEven ? even : odd;
        }

        return new Vector<T>(ar);
    }

    /// <summary>
    /// A <see cref="Vector{T}"/> with 1s in the <b>even</b> indices and 0s in the <b>odd</b> indices, i.e. <c>[1,0,1,0...]</c>
    /// </summary>
    /// <remarks>
    /// 📎 The sum of a <see cref="CreateAlternatingVector{T}"/> is always <see cref="Vector{T}.Count"/> / 2.
    /// Since <see cref="Vector{T}.Count"/> is always an even number, we don't have to worry about fractions in the result.
    /// </remarks>
    internal static readonly Vector<int> Alternating_10 = CreateAlternatingVector<int>(true);
    /// <summary>
    /// A <see cref="Vector{T}"/> with 1s in  <b>odd</b> indices and 0 <b>even</b> indices, i.e. <c>[0,1,0,1...]</c>
    /// </summary>
    /// <inheritdoc cref="Alternating_10"/>
    internal static readonly Vector<int> Alternating_01 = CreateAlternatingVector<int>(false);

    #endregion

    internal static void DistributeRemainder_FromOutsideSpaced(Span<int> portions, int remainder) {
        Debug.Assert(remainder       > 0);
        Debug.Assert(remainder       < portions.Length);
        Debug.Assert(portions.Length > 1);

        // handle some known quick paths
        switch (remainder) {
            case 1:
                portions[0] += 1;
                return;
            case 2:
                // because Length > remainder, Length >= 3
                portions[0]  += 1;
                portions[^1] += 1;
                return;
            default: {
                if (remainder == portions.Length - 1) {
                    IncrementAllButOne(portions, portions.Length / 2);
                    return;
                }

                break;
            }
        }

        var leftRemainder  = remainder / 2;
        var rightRemainder = remainder - leftRemainder;

        var (left, right) = portions.TakeLeftovers(portions.Length / 2);
        DistributeRemainder_SpacedSimple_Left(left, leftRemainder);
        DistributeRemainder_SpacedSimple_Right(right, rightRemainder);
    }

    internal static void DistributeRemainder_SpacedSimple_Right(Span<int> portions, int remainder) {
        var (evenPattern, oddPattern) = portions.Length % 2 == 0 ? (Alternating_10, Alternating_01) : (Alternating_01, Alternating_10);
        var evenRemainder = Math.Min(remainder, portions.Length / 2);
        portions[^(evenRemainder * 2)..].FastAddPattern(evenPattern);

        // calculate how many odds we need
        var oddRemainder = remainder - (portions.Length / 2);
        if (oddRemainder < 0) {
            return;
        }

        // add in the odds
        var oddSpanLength = oddRemainder * 2;
        var oddSpan       = portions[^oddSpanLength..];
        oddSpan.FastAddPattern(oddPattern);
    }

    internal static void DistributeRemainder_SpacedSimple_Left(Span<int> portions, int remainder) {
        var evenRemainder = Math.Min(remainder, portions.Length / 2);
        portions[..(evenRemainder                               * 2)].FastAddPattern(Alternating_10);

        //calculate how many odds we need
        var oddRemainder = remainder - (portions.Length / 2);
        if (oddRemainder <= 0) {
            return;
        }

        // add in the odds
        var oddSpanLength = oddRemainder * 2;
        var oddSpan       = portions[..oddSpanLength];
        oddSpan.FastAddPattern(Alternating_01);
    }

    /// <summary>
    /// This could technically be slightly more efficient, by figuring out how many full vectors it needs vs. how many total-spread vectors and then the one last vector, but that complicates the method considerably.
    /// I'm fairly satisfied with the balance between efficiency and convolutedness in this version.
    /// </summary>
    internal static void DistributeRemainder_FromLeftSpaced(Span<int> portions, int remainder) {
        Debug.Assert(remainder > 0 && remainder < portions.Length);

        var evenRemainder = Math.Min(remainder, portions.Length / 2);
        portions[..(evenRemainder                               * 2)].FastAddPattern(Alternating_01);

        // calculate how many odds we need
        var oddRemainder = remainder - (portions.Length / 2);

        if (oddRemainder <= 0) {
            return;
        }

        // add in the odds
        var oddSpanLength = oddRemainder * 2;
        var oddSpan       = portions[^oddSpanLength..];
        // decide which pattern to use
        var oddPattern = portions.Length % 2 == 0 ? Alternating_10 : Alternating_01;
        oddSpan.FastAddPattern(oddPattern);
    }

    internal static void DistributeRemainder_FromRight(Span<int> portions, int remainder) {
        Debug.Assert(remainder < portions.Length);
        Debug.Assert(remainder > 0);

        var getsRemainder = portions[^remainder..];
        getsRemainder.FastAddAll(1);
    }

    internal static void DistributeRemainder_FromOutside(Span<int> portions, int remainder) {
        Debug.Assert(remainder < portions.Length);
        Debug.Assert(remainder > 0);

        var left  = remainder / 2;
        var right = remainder - left;

        DistributeRemainder_FromRight(portions, right);
        DistributeRemainder_FromLeft(portions, left);
    }

    internal static void DistributeRemainder_FromCenter(Span<int> portions, int remainder) {
        Debug.Assert(remainder < portions.Length);
        Debug.Assert(remainder > 0);

        var left  = remainder / 2;
        var right = remainder - left;

        var leftHalf  = portions[..(portions.Length / 2)];
        var rightHalf = portions[leftHalf.Length..];
        DistributeRemainder_FromRight(leftHalf, left);
        DistributeRemainder_FromLeft(rightHalf, right);
    }

    internal static void DistributeRemainder_Randomly(Span<int> portions, int remainder, Random generator) {
        Debug.Assert(remainder < portions.Length);
        Debug.Assert(remainder > 0);

        if (remainder == 1) {
            var toAdd = generator.Next(portions.Length);
            portions[toAdd] -= 1;
            return;
        }

        if (remainder == portions.Length - 1) {
            int notIncrementedIndex = generator.Next(portions.Length);
            IncrementAllButOne(portions, notIncrementedIndex);
            return;
        }

        // 1. Create a span that we will zip together with the portions
        Span<int> additionAmounts = stackalloc int[portions.Length];

        // 2. Populate the first `remainder` elements with 1
        additionAmounts[..remainder].Fill(1);

        // 3. Shuffle the 0s and 1s
        additionAmounts.Shuffle(generator);

        // 4. Zip those amounts into the portions
        portions.FastAddEach(additionAmounts);
    }

    private static void IncrementAllButOne(Span<int> portions, int notIncrementedIndex) {
        portions.FastAddAll(1);
        portions[notIncrementedIndex] -= 1;
    }

    #endregion
}