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
            $"must contain the index [{requiredIndex}] (actual size: {actualValue.Count}"
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
            $"must contain the index [{requiredIndex}] (actual size: {actualValue.Count}"
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

    #endregion
}