using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Collections

    #region BeIndexOf

    public static int BeIndexOf(
        int         actualIndex,
        ICollection possibleCollection,
        string?     details = default,
        [CallerArgumentExpression("actualIndex")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return Be(
            actualIndex,
            possibleCollection.ContainsIndex,
            details,
            parameterName,
            rejectedBy,
            $"must be a valid index of a {possibleCollection.GetType().Name} of size {possibleCollection.Count}"
        );
    }

    public static Index BeIndexOf(
        Index       actualIndex,
        ICollection possibleCollection,
        string?     details = default,
        [CallerArgumentExpression("actualIndex")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return Be(
            actualIndex,
            possibleCollection.ContainsIndex,
            details,
            parameterName,
            rejectedBy,
            $"must be a valid index of a {possibleCollection.GetType().Name} of size {possibleCollection.Count}"
        );
    }

    #endregion

    #region Contain (Index)

    public static T ContainIndex<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T actualValue,
        Index   requiredIndex,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => it.ContainsIndex(requiredIndex),
            details,
            parameterName,
            rejectedBy,
            $"must contain the index [{requiredIndex}] (actual size: {actualValue.Count})"
        );
    }

    public static TCollection ContainIndex<TElements, TCollection>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        TCollection actualValue,
        Index   requiredIndex,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where TCollection : IReadOnlyCollection<TElements> {
        return Be(
            actualValue,
            it => it.ContainsIndex(requiredIndex),
            details,
            parameterName,
            rejectedBy,
            $"must contain the index [{requiredIndex}] (actual size: {actualValue.Count})"
        );
    }

    #endregion

    #region Contain (Range)

    public static T ContainRange<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T actualValue,
        Range   range,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => {
                range.GetOffsetAndLength(it.Count);
                return true;
            },
            details,
            parameterName,
            rejectedBy,
            $"must contain the range {range}"
        );
    }

    #endregion

    #region Contain (entry)

    public static T ContainAny<T, T2>(
        T               actualValue,
        IEnumerable<T2> desiredValues,
        string?         details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : IEnumerable<T2> {
        return Be(
            actualValue,
            it => it.ContainsAny(desiredValues),
            details,
            parameterName,
            rejectedBy
        );
    }

    public static T NotContainAny<T, T2>(
        [InstantHandle]
        T actualValues,
        [InstantHandle]
        IEnumerable<T2> badValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : IEnumerable<T2> {
        if (actualValues.IsEmpty()) {
            return actualValues;
        }

        badValues = badValues.AsList();
        if (!actualValues.ContainsAny(badValues)) {
            return actualValues;
        }

        var badNuts = actualValues.Union(badValues.AsEnumerable());
        throw Reject(
            actualValue: actualValues,
            details,
            parameterName: parameterName,
            rejectedBy: rejectedBy,
            reason: $"contained the invalid {typeof(T2)}: {badNuts}"
        );
    }

    public static T ContainAll<T, T2>(
        [InstantHandle]
        T actualValues,
        [InstantHandle]
        IEnumerable<T2> requiredValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : IEnumerable<T2> {
        requiredValues = requiredValues.AsList();
        if (actualValues.ContainsAll(requiredValues)) {
            return actualValues;
        }

        var missingNuts = actualValues.Except(requiredValues);
        throw Reject(
            actualValue: actualValues,
            details: details,
            parameterName: parameterName,
            rejectedBy: rejectedBy,
            reason: $"was missing the required {typeof(T2)} values {missingNuts}"
        );
    }

    #endregion

    public static COLLECTION NotContainNull<COLLECTION, ENTRY>(
        [NotNull] COLLECTION? actualValue,
        string?               details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where COLLECTION : IEnumerable<ENTRY?>? {
        if (actualValue == null) {
            throw Reject(actualValue, details, parameterName, rejectedBy, $"the {typeof(COLLECTION).Name} itself is null!");
        }

        if (actualValue.Any(it => it == null)) {
            throw Reject(actualValue, details, parameterName, rejectedBy);
        }

        return actualValue;
    }

    #region NotBeEmpty

    public static SOURCE NotBeEmpty<SOURCE>(
        [InstantHandle]
        SOURCE? actualValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where SOURCE : ICollection {
        return Be(actualValues, it => it?.Count > 0, details, parameterName, rejectedBy);
    }

    public static IEnumerable<T> NotBeEmpty<T>(
        [InstantHandle]
        IEnumerable<T>? actualValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return Be(actualValues, it => it.IsNotEmpty(), details, parameterName, rejectedBy);
    }

    #endregion

    #endregion

    #region Tuplizing

    public static T HaveSize<T, T2>(
        [InstantHandle]
        T? actualValues,
        [NonNegativeValue]
        int requiredSize,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : ICollection<T2> {
        if (actualValues?.Count != requiredSize) {
            throw Reject(actualValues, details, parameterName, rejectedBy, $"must contain EXACTLY {requiredSize} items (actual size: {actualValues?.Count.ToString() ?? "⛔"})!");
        }

        return actualValues;
    }

    /// <summary>
    /// Similarly to <see cref="Enumerable.Single{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>, this guarantees that the <paramref name="actualValues"/> contains <paramref name="requiredCount"/> entries.
    ///
    /// <b>📎 NOTE:</b> This method will not throw an error until the returned <see cref="IEnumerable{T}"/> is actually enumerated!
    /// 
    /// </summary>
    /// <param name="actualValues">the <see cref="IEnumerable{T}"/> being validated</param>
    /// <param name="requiredCount">the desired number of <typeparamref name="T"/> entries</param>
    /// <param name="details">optional additional information</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the items in <paramref name="actualValues"/></typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/></returns>
    /// <exception cref="RejectionException">if, <b>when enumerated</b>, the sequence doesn't contain <b>exactly</b> <paramref name="requiredCount"/> elements</exception>
    public static IEnumerable<T> TakeExactly<T>(
        this IEnumerable<T> actualValues,
        [NonNegativeValue]
        int requiredCount,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        using var iter = actualValues.GetEnumerator();
        for (int i = 0; i < requiredCount; i++) {
            if (iter.MoveNext()) {
                yield return iter.Current;
            }
            else {
                throw Reject(actualValues.ToString(), details, parameterName, rejectedBy, $"must contain EXACTLY {requiredCount} items (actual size: {i})");
            }
        }

        if (iter.MoveNext()) {
            throw Reject(actualValues.ToString(), details, parameterName, rejectedBy, $"must contain EXACTLY {requiredCount} items (actual size: > {requiredCount})");
        }
    }

    public static (T, T) Have2<T>(
        [InstantHandle]
        IEnumerable<T>? actualValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        (T, T) result;
        var    has2 = HaveSize<IList<T>, T>(actualValues?.AsList(), 2, details, parameterName, rejectedBy);
        return (has2[0], has2[1]);
    }

    public static (T, T, T) Have3<T>(
        [InstantHandle]
        IEnumerable<T>? actualValues,
        string? details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        var has3 = HaveSize<IList<T>, T>(actualValues?.AsList(), 3, details, parameterName, rejectedBy);

        return (has3[0], has3[1], has3[2]);
    }

    #endregion
}