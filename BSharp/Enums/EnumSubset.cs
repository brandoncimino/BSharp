using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// A singleton collection of <see cref="Enum"/> <typeparamref name="T"/> values.
    /// <br/>
    /// Use this to "restrict" an existing <see cref="Enum"/>.
    /// </summary>
    /// <example>
    /// <see cref="EnumSubset{T}"/> Weekends = new <see cref="EnumSubset{T}"/>(<see cref="DayOfWeek.Saturday"/>, <see cref="DayOfWeek.Sunday"/>);
    /// <p/>
    /// Weekends.<see cref="Validate"/>(<see cref="DayOfWeek.Monday"/>); //this throws an <see cref="EnumNotInSetException{T}"/>
    /// </example>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Please use " + nameof(EnumSet) + " instead, which is a proper " + nameof(HashSet<object>) + " implementation")]
    public class EnumSubset<T> : ICollection<T> where T : struct, Enum {
        /// <summary>
        /// The backing <see cref="IList{T}"/> for the <see cref="EnumSubset{T}"/>.
        /// </summary>
        private readonly IList<T> Subset;

        #region Constructors

        public EnumSubset(IEnumerable<T> values, bool isReadOnly = false) {
            var valueList = values.ToList();
            Subset = isReadOnly ? (IList<T>)new ReadOnlyCollection<T>(valueList) : valueList;
        }

        public EnumSubset(params T[] values) : this(values, false) { }

        public EnumSubset(IEnumerable<IEnumerable<T>> values, bool isReadOnly = false) : this(values.SelectMany(it => it).ToList(), isReadOnly) { }

        #endregion

        #region IList Delegated Implementation

        public IEnumerator<T> GetEnumerator() {
            return Subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Subset.GetEnumerator();
        }

        public void Add(T item) {
            Subset.Add(item);
        }

        public void Clear() {
            Subset.Clear();
        }

        public bool Contains(T item) {
            return Subset.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Subset.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return Subset.Remove(item);
        }

        public int  Count      => Subset.Count;
        public bool IsReadOnly => Subset.IsReadOnly;

        public int IndexOf(T item) {
            return Subset.IndexOf(item);
        }

        public void Insert(int index, T item) {
            Subset.Insert(index, item);
        }

        public void RemoveAt(int index) {
            Subset.RemoveAt(index);
        }

        public T this[int index] {
            get => Subset[index];
            set => Subset[index] = value;
        }

        #endregion

        #region Meaningful Stuff

        /// <summary>
        /// Throws an exception unless <see cref="Subset"/> contains <b>all</b> of the <paramref name="expectedValue"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used when you want strict validation that throws an <see cref="Exception"/> on failure.
        /// <br/>If you want to "test" whether or not a value is in an <see cref="EnumSubset{T}"/>, use <see cref="Contains"/>.
        /// </remarks>
        /// <param name="expectedValue">The <see cref="T"/> <see cref="Enum"/> value that should be within this <see cref="EnumSubset{T}"/>.</param>
        /// <returns><paramref name="expectedValue"/>, if <see cref="Contains">Contains</see>(<paramref name="expectedValue"/>) returns <c>true</c>.</returns>
        /// <exception cref="EnumNotInSetException{T}">if <see cref="Contains">Contains</see>(<paramref name="expectedValue"/>) returns <c>false</c>.</exception>
        public void MustContain(params T[] expectedValue) {
            if (this.IsSupersetOf(expectedValue) == false) {
                throw new EnumNotInSetException<T>(this, expectedValue);
            }
        }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(IEnumerable<T> expectedValues) {
            MustContain(expectedValues.ToArray());
        }

        /// <summary>
        /// If this <see cref="CollectionUtils.DoesNotContain{T}"/> <paramref name="expectedValue"/>, then throw the exception created by <paramref name="exceptionOnFailure"/>.
        /// </summary>
        /// <param name="expectedValue">the <typeparamref name="T"/> value that this MUST contain</param>
        /// <param name="exceptionOnFailure">a <see cref="Func{TResult}"/> that generates the <see cref="Exception"/> when <paramref name="expectedValue"/> wasn't found</param>
        /// <exception cref="Exception"></exception>
        public void MustContain(T expectedValue, Func<Exception> exceptionOnFailure) {
            if (this.DoesNotContain(expectedValue)) {
                throw exceptionOnFailure.Invoke();
            }
        }

        /// <summary>
        /// <inheritdoc cref="MustContain(T, Func{Exception})"/>
        /// </summary>
        /// <param name="expectedValue"><inheritdoc cref="MustContain(T, Func{Exception})"/></param>
        /// <param name="exceptionOnFailure">a <see cref="Func{TIn,TResult}"/> that consumes <paramref name="expectedValue"/> and produces an <see cref="Exception"/></param>
        /// <exception cref="Exception"></exception>
        public void MustContain(T expectedValue, Func<T, Exception> exceptionOnFailure) {
            if (this.DoesNotContain(expectedValue)) {
                throw exceptionOnFailure.Invoke(expectedValue);
            }
        }

        /// <summary>
        /// Returns a new <see cref="EnumSubset{T}"/> containing all of the <see cref="T"/> values that are <b>NOT</b> in the original <see cref="EnumSubset{T}"/>.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// var weekEnds = new EnumSubset<DayOfWeek>(DayOfWeek.Saturday, DayOfWeek.Sunday);
        /// var weekDays = weekEnds.Inverse(); // == Monday...Friday
        /// ]]></code>
        /// </example>
        public EnumSubset<T> Inverse() {
            var possibleValues = BEnum.GetValues<T>();
            return possibleValues.Where(it => this.ContainsNone(it)).ToEnumSubset();
        }

        #endregion
    }

    [Obsolete(nameof(EnumSubset<DayOfWeek>) + " is obsolete; please use " + nameof(EnumSet<DayOfWeek>))]
    public static class EnumSubsetExtensions {
        public static EnumSubset<T> ToEnumSubset<T>(this IEnumerable<T> source) where T : struct, Enum {
            return new EnumSubset<T>(source);
        }
    }
}