using System.Runtime.CompilerServices;

using FowlFever.BSharp.Memory;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Numbers

    /// <summary>
    /// Validates that <paramref name="actualValue"/> is <see cref="PrimitiveMath.IsPositive{T}"/> (≥ 0).
    /// </summary>
    /// <param name="actualValue">an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value</param>
    /// <param name="details"></param>
    /// <param name="parameterName"></param>
    /// <param name="rejectedBy"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RejectionException">if <paramref name="actualValue"/> is less than 0</exception>
    [NonNegativeValue]
    public static T BePositive<T>(
        T       actualValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) where T : unmanaged {
        if (PrimitiveMath.IsPositive(actualValue)) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName, rejectedBy, reason: "must be > 0");
    }

    #endregion
}