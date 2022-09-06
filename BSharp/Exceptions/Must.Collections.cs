using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;
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
        [CallerMemberName] string? rejectedBy = default
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
        [CallerMemberName] string? rejectedBy = default
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
        [NotNull] T actualValue,
        Index       index,
        string?     details = default,
        [CallerArgumentExpression("actualValue")]
        string? _index = default,
        [CallerMemberName] string? _caller = default
    )
        where T : ICollection {
        return Be(
            actualValue,
            it => it.ContainsIndex(index),
            details,
            _index,
            _caller,
            $"must contain the index [{index}] (actual size: {actualValue.Count})"
        );
    }

    public static TCollection ContainIndex<TElements, TCollection>(
        [NotNull] TCollection actualValue,
        Index                 index,
        string?               details = default,
        [CallerArgumentExpression("actualValue")]
        string? _index = default,
        [CallerMemberName] string? _caller = default
    )
        where TCollection : IReadOnlyCollection<TElements> {
        return Be(
            actualValue,
            it => it.ContainsIndex(index),
            details,
            _index,
            _caller,
            $"must contain the index [{index}] (actual size: {actualValue.Count})"
        );
    }

    /// <summary>
    /// Requires <paramref name="index"/> to fall inside of a collection of size <paramref name="collectionLength"/>.
    /// </summary>
    /// <param name="collectionLength">the number of elements in the theoretical collection</param>
    /// <param name="index">the <see cref="Index"/> that must fall inside the collection</param>
    /// <param name="details">optional additional details</param>
    /// <param name="_index">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <returns>the <see cref="Index.GetOffset"/> calculated using <paramref name="index"/> and <paramref name="collectionLength"/></returns>
    /// <exception cref="RejectionException">if <see cref="Index.GetOffset"/> results in a value outside of <paramref name="collectionLength"/></exception>
    public static int ContainIndex(
        int     collectionLength,
        Index   index,
        string? details = default,
        [CallerArgumentExpression("collectionLength")]
        string? _index = default,
        [CallerMemberName] string? _caller = default
    ) {
        var offset = index.GetOffset(collectionLength);
        if (offset < 0 || offset >= collectionLength) {
            throw Exceptions.Reject.IndexOutOfRange(index, collectionLength, details, _index, _caller);
        }

        return offset;
    }

    #endregion

    #region Contain (Range)

    public static T ContainRange<T>(
        [NotNull] T actualValue,
        Range       range,
        string?     details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
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
        [CallerMemberName] string? rejectedBy = default
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
        [InstantHandle] T               actualValues,
        [InstantHandle] IEnumerable<T2> badValues,
        string?                         details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
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
        [InstantHandle] T               actualValues,
        [InstantHandle] IEnumerable<T2> requiredValues,
        string?                         details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
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
        [CallerMemberName] string? rejectedBy = default
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
        [InstantHandle] SOURCE? actualValues,
        string?                 details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    )
        where SOURCE : ICollection {
        return Be(actualValues, it => it?.Count > 0, details, parameterName, rejectedBy);
    }

    public static IEnumerable<T> NotBeEmpty<T>(
        [InstantHandle] IEnumerable<T>? actualValues,
        string?                         details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        return Be(actualValues, it => it.IsNotEmpty(), details, parameterName, rejectedBy);
    }

    #endregion

    #endregion

    #region Tuplizing

    public static T HaveSize<T, T2>(
        [InstantHandle]    T?  actualValues,
        [NonNegativeValue] int requiredSize,
        string?                details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
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
    [Experimental("This doesn't quite work as I'd like to yet - if you TakeExactly(3) and then Take(2), a collection with 2 will pass or something like that")]
    public static IEnumerable<T> TakeExactly<T>(
        this               IEnumerable<T> actualValues,
        [NonNegativeValue] int            requiredCount,
        string?                           details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
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

    [Experimental("Still needs fiddling")]
    private record TakeExactlyEnumerator<T>(IEnumerator<T> Source, int Count) : IEnumerator<T> {
        private int _taken = 0;

        public bool MoveNext() {
            _taken += 1;
            if (_taken >= Count || !Source.MoveNext()) {
                throw new Exception($"too many!");
            }

            return true;
        }

        public void Reset() {
            throw new NotImplementedException();
        }

        public T            Current => Source.Current;
        object? IEnumerator.Current => Current;

        public void Dispose() {
            if (_taken != Count) { }

            if (MoveNext()) {
                throw new Exception("Still had some left!!");
            }
        }
    }

    public static (T, T) Have2<T>(
        [InstantHandle] IEnumerable<T>? actualValues,
        string?                         details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        (T, T) result;
        var    has2 = HaveSize<IList<T>, T>(actualValues?.AsList(), 2, details, parameterName, rejectedBy);
        return (has2[0], has2[1]);
    }

    public static (T, T, T) Have3<T>(
        [InstantHandle] IEnumerable<T>? actualValues,
        string?                         details = default,
        [CallerArgumentExpression("actualValues")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        var has3 = HaveSize<IList<T>, T>(actualValues?.AsList(), 3, details, parameterName, rejectedBy);

        return (has3[0], has3[1], has3[2]);
    }

    #endregion
}