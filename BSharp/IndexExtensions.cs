using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp;

public static class IndexExtensions {
    /// <param name="index">this <see cref="Index"/></param>
    /// <returns>the "direction" of an <see cref="Index"/>, i.e. <see cref="Index.IsFromEnd"/> ⇒ -1</returns>
    [Pure]
    [ValueRange(-1)]
    [ValueRange(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this Index index) => index.IsFromEnd ? -1 : 1;

    /// <param name="index">this <see cref="Index"/></param>
    /// <returns>the <see cref="Index.Value"/> of this <see cref="Index"/>, treating <see cref="Index.IsFromEnd"/> as negative values <i>(i.e. <c>^1</c> ⇒ -1)</i></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SignedValue(this Index index) => index.Value * index.Sign();

    /// <summary>
    /// Similar to <see cref="Index.GetOffset"/>, but clamps the resulting value to 0 and the <paramref name="length"/>.
    /// </summary>
    /// <param name="index">this <see cref="Index"/></param>
    /// <param name="length">the number of items in the prospective collection</param>
    /// <returns>the <see cref="Index.GetOffset"/>, clamped to be a value within the <paramref name="length"/></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [NonNegativeValue]
    public static int GetOffsetClamped(this Index index, [NonNegativeValue] int length) {
        return Math.Clamp(index.GetOffset(length), 0, length);
    }
}