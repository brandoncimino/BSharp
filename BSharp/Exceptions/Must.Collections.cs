using System.Collections;
using System.Collections.Generic;
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

    #region Contain (Range)

    private static bool IsOutOfRange(int _length, int start, int length) {
#if TARGET_64BIT
            // Since start and length are both 32-bit, their sum can be computed across a 64-bit domain
            // without loss of fidelity. The cast to uint before the cast to ulong ensures that the
            // extension from 32- to 64-bit is zero-extending rather than sign-extending. The end result
            // of this is that if either input is negative or if the input sum overflows past Int32.MaxValue,
            // that information is captured correctly in the comparison against the backing _length field.
            // We don't use this same mechanism in a 32-bit process due to the overhead of 64-bit arithmetic.
            return (ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)_length)
#else
        return (uint)start > (uint)_length || (uint)length > (uint)(_length - start);
#endif
    }

    public static int ContainRange(
        int                                          collectionLength,
        int                                          offset,
        int                                          length,
        string?                                      details = default,
        [CallerArgumentExpression("offset")] string? _offset = default,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        if (!IsOutOfRange(collectionLength, offset, length)) {
            return collectionLength;
        }

        var endPoint = offset + length;
        throw new RejectionException(
            offset..endPoint,
            details: details,
            rejectedBy: _caller,
            _actualValue: $"{_offset}..{endPoint}",
            reason: $"The given range is out-of-bounds for a collection of length {collectionLength}"
        );
    }

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