using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

using FowlFever.BSharp.Collections;

using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using JetBrains.Annotations;

using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Collections

    #region Contain (Index)

    public static T ContainIndex<T>(
        [NotNull]
        T   actualValue,
        [NonNegativeValue]
        int requiredIndex,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => it.Count > requiredIndex,
            parameterName,
            methodName,
            $"must contain the index {requiredIndex} (actual size: {actualValue.Count})"
        );
    }

    public static T ContainIndex<T>(
        [NotNull]
        T actualValue,
        Index           requiredIndex,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => requiredIndex.GetOffset(it.Count) > 0,
            parameterName,
            methodName,
            $"must contain the index [{requiredIndex}]"
        );
    }

    #endregion

    #region Contain (Range)

    public static T ContainRange<T>(
        [NotNull]
        T     actualValue,
        Range range,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => {
                range.GetOffsetAndLength(it.Count);
                return true;
            },
            parameterName,
            methodName,
            $"must contain the range {range}"
        );
    }

    #endregion

    #region Contain (entry)

    public static T ContainAny<T, T2>(
        T               actualValue,
        IEnumerable<T2> desiredValues,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) where T : IEnumerable<T2> {
        return Be(
            actualValue,
            it => it.ContainsAny(desiredValues),
            parameterName,
            methodName
        );
    }

    public static T NotContainAny<T, T2>(
        [InstantHandle]
        T               actualValues,
        [InstantHandle]
        IEnumerable<T2> badValues,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) where T : IEnumerable<T2> {
        badValues = badValues.AsList();
        if (!actualValues.ContainsAny(badValues)) {
            return actualValues;
        }

        var badNuts = actualValues.Union(badValues.AsEnumerable());
        throw Reject(
            actualValues,
            parameterName,
            methodName,
            $"contained the invalid {typeof(T2)}: {badNuts}"
        );
    }

    public static T ContainAll<T, T2>(
        [InstantHandle]
        T actualValues,
        [InstantHandle]
        IEnumerable<T2> requiredValues,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) where T : IEnumerable<T2> {
        requiredValues = requiredValues.AsList();
        if (actualValues.ContainsAll(requiredValues)) {
            return actualValues;
        }

        var missingNuts = actualValues.Except(requiredValues);
        throw Reject(actualValues, parameterName, methodName, $"was missing the required {typeof(T2)} values {missingNuts}");
    }
    
    #endregion
    
    #endregion
}