using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Optional {
    /// <inheritdoc cref="IOptional{T}"/>
    /// <remarks>
    /// An <see cref="Optional{T}"/> can be considered a <see cref="IReadOnlyCollection{T}"/> with a max capacity of 1.
    /// This gives access to the full suite of <see cref="System.Linq"/> extension methods.
    /// </remarks>
    [PublicAPI]
    [JsonConverter(typeof(OptionalJsonConverter))]
    public readonly struct Optional<T> : IEquatable<T>, IEquatable<Optional<T>>, IOptional<T> {
        public override int GetHashCode() {
            unchecked {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ HasValue.GetHashCode();
            }
        }

        public override bool Equals(object obj) {
            // this syntax...is scary
            return obj switch {
                Optional<T> optional when Equals(optional) => true,
                T t when Equals(t)                         => true,
                _                                          => false
            };
        }

        private readonly T _value;

        /// <summary>
        /// Whether or not a <see cref="Value"/> is present.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#isPresent--">Optional.isPresent()</a></li>
        /// </ul>
        /// </remarks>
        public bool HasValue { get; }

        public T Value => HasValue ? _value : throw OptionalException.IsEmptyException(this);

        public Optional(T value) {
            _value   = value;
            HasValue = true;
        }

        #region Implementations

        public int Count => HasValue ? 1 : 0;

        private IEnumerable<T> GetEnumerable() {
            return Enumerable.Repeat(_value, Count);
        }

        public IEnumerator<T> GetEnumerator() {
            return GetEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)GetEnumerable()).GetEnumerator();
        }

        public bool Equals(T           other) => Optional.AreEqual(this, other);
        public bool Equals(Optional<T> other) => Optional.AreEqual(this, other);

        // 📝 NOTE: No longer considering IOptional<T> for comparisons because going between structs and interfaces is clunky and weird
        [Pure]
        public static bool operator ==(Optional<T> a, Optional<T> b) => Optional.AreEqual(a, b);

        [Pure]
        public static bool operator !=(Optional<T> a, Optional<T> b) => !Optional.AreEqual(a, b);

        // NOTE: An equality comparator for a left-side IOptional isn't supported because it would cause ambiguity with other operators.
        // public static bool operator ==(IOptional<T> a, Optional<T> b) => Optional.AreEqual(a, b);
        // public static bool operator !=(IOptional<T> a, Optional<T> b) => !Optional.AreEqual(a, b);

        [Pure]
        public static bool operator ==(T? a, Optional<T> b) => Optional.AreEqual(a, b);

        [Pure]
        public static bool operator !=(T? a, Optional<T> b) => !Optional.AreEqual(a, b);

        [Pure]
        public static bool operator ==(Optional<T> a, T? b) => Optional.AreEqual(a, b);

        [Pure]
        public static bool operator !=(Optional<T> a, T? b) => !Optional.AreEqual(a, b);


        public override string ToString() {
            return Optional.ToString(this, new PrettificationSettings());
        }

        #endregion

        public static implicit operator Optional<T>(T value) {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Works the same as <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})">Enumerable.Select</see>,
        /// but returns a new <see cref="Optional{T}"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#map-java.util.function.Function-">Optional.map()</a></li>
        /// </ul>
        /// </remarks>
        /// <param name="selector"></param>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public Optional<TNew> Select<TNew>(Func<T, TNew> selector) {
            return HasValue ? new Optional<TNew>(selector.Invoke(Value)) : new Optional<TNew>();
        }

        /// <summary>
        /// If <see cref="HasValue"/> and <see cref="Value"/> satisfies <paramref name="predicate"/>, return this <see cref="Optional{T}"/>.
        ///
        /// Otherwise, return an empty <see cref="Optional{T}"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Optional<T> Where(Func<T, bool> predicate) {
            if (HasValue && predicate.Invoke(Value)) {
                return this;
            }
            else {
                return default;
            }
        }
    }
}