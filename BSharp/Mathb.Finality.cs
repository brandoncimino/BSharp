using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region Finality

    /// <inheritdoc cref="double.IsFinite"/>
    /// <remarks>
    /// The following values are considered <b>not finite</b>:
    /// <ul>
    /// <li><see cref="double.PositiveInfinity"/></li>
    /// <li><see cref="double.NegativeInfinity"/></li>
    /// <li><see cref="double.NaN"/></li>
    /// </ul>
    /// <b><i>All</i></b> other values are considered <b>finite</b>.
    /// <p/>
    /// Note that this means that <see cref="double.NaN"/> is neither <see cref="double.IsFinite"/> <i>or</i> <see cref="double.IsInfinity"/>. 
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this double d) => double.IsFinite(d);

    /// <inheritdoc cref="float.IsFinite"/>
    /// <remarks>
    /// The following values are considered <b><i>not finite</i></b>:
    /// <ul>
    /// <li><see cref="float.PositiveInfinity"/></li>
    /// <li><see cref="float.NegativeInfinity"/></li>
    /// <li><see cref="float.NaN"/></li>
    /// </ul>
    /// <b><i>All</i></b> other values are considered <b>finite</b>.
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this float f) => float.IsFinite(f);

    /// <inheritdoc cref="double.IsInfinity"/>
    /// <remarks>
    /// 📎 <see cref="double.NaN"/> is neither <see cref="double.IsFinite">finite</see> <i>nor</i> <see cref="double.IsInfinity">infinite</see>.
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinite(this double d) => double.IsInfinity(d);

    /// <inheritdoc cref="float.IsInfinity"/>
    /// <remarks>
    /// 📎 <see cref="float.NaN"/> is neither <see cref="float.IsFinite">finite</see> <i>nor</i> <see cref="float.IsInfinity">infinite</see>.
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinite(this float f) => float.IsInfinity(f);

    /// <inheritdoc cref="double.IsPositiveInfinity"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositiveInfinity(this double d) => double.IsPositiveInfinity(d);

    /// <inheritdoc cref="float.IsPositiveInfinity"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositiveInfinity(this float f) => float.IsPositiveInfinity(f);

    /// <inheritdoc cref="double.IsNegativeInfinity"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegativeInfinity(this double d) => double.IsNegativeInfinity(d);

    /// <inheritdoc cref="float.IsNegativeInfinity"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegativeInfinity(this float f) => float.IsNegativeInfinity(f);

    #endregion

    /// <inheritdoc cref="double.IsNaN"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(this double d) => double.IsNaN(d);

    /// <inheritdoc cref="float.IsNaN"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(this float f) => float.IsNaN(f);
}