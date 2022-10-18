using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static class IndexExtensions {
    /// <param name="index">this <see cref="Index"/></param>
    /// <returns>the "direction" of an <see cref="Index"/>, i.e. <see cref="Index.IsFromEnd"/> ⇒ -1</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this Index index) => index.IsFromEnd ? -1 : 1;

    /// <param name="index">this <see cref="Index"/></param>
    /// <returns>the <see cref="Index.Value"/> of this <see cref="Index"/>, treating <see cref="Index.IsFromEnd"/> as negative values <i>(i.e. <c>^1</c> ⇒ -1)</i></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SignedValue(this Index index) => index.Value * index.Sign();
}