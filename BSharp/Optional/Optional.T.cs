using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.Implementors;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Optional {
    /// <inheritdoc cref="IOptional{T}"/>
    /// <remarks>
    /// An <see cref="Optional{T}"/> can be considered a <see cref="IReadOnlyCollection{T}"/> with a max capacity of 1.
    /// This gives access to the full suite of <see cref="System.Linq"/> extension methods.
    /// </remarks>
    [PublicAPI]
    [JsonConverter(typeof(OptionalJsonConverter.Newtonsoft))]
    [System.Text.Json.Serialization.JsonConverter(typeof(OptionalJsonConverter.SystemTextFactory))]
    public readonly struct Optional<T> : IOptional<T>,
                                         IEquatable<T>,
                                         IEquatable<IHas<T>>,
                                         IEquatable<IOptional<T>>,
                                         IPrettifiable {
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

        /// <summary>
        /// Inverse of <see cref="HasValue"/>.
        /// </summary>
        public bool IsEmpty => !HasValue;

        public T Value => HasValue ? _value : throw OptionalException.IsEmptyException(this);

        public Optional(T value) {
            _value   = value;
            HasValue = true;
        }

        #region Logic / Filtering

        public Optional<T> OrElse(Optional<T> fallback) => HasValue ? this : fallback;

        [return: NotNullIfNotNull("fallback")] public T? OrElse(T? fallback) => HasValue ? Value : fallback;

        #endregion

        #region Implementations

        #region Enumeration

        int IReadOnlyCollection<T>.Count           => HasValue ? 1 : 0;
        private IEnumerable<T>     GetEnumerable() => Enumerable.Repeat(_value, HasValue ? 1 : 0);
        public  IEnumerator<T>     GetEnumerator() => GetEnumerable().GetEnumerator();
        IEnumerator IEnumerable.   GetEnumerator() => ((IEnumerable)GetEnumerable()).GetEnumerator();

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
            return HasValue && predicate.Invoke(Value) ? this : default;
        }

        /// <summary>
        /// <see cref="Enumerable.Cast{TResult}"/>s this <see cref="Value"/> to <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T2">the destination <see cref="Type"/></typeparam>
        /// <returns>a new <see cref="Optional{T2}"/> of <typeparamref name="T2"/></returns>
        /// <seealso cref="Enumerable.Cast{TResult}"/>
        /// <remarks>The redundant-seeming <c>Cast</c> to <typeparamref name="T2"/> is to account for the nullability of <typeparamref name="T2"/>.
        /// TODO: Experiment on whether or not <see cref="Enumerable.Cast{TResult}"/> actually respects <typeparamref name="T2"/>'s nullability
        /// </remarks>
        public Optional<T2> Cast<T2>() {
            return this.AsEnumerable().Select(it => (T2?)(object?)it).Cast<T2>().ToOptional();
        }

        #endregion

        #region Equality

        private static readonly OptionalEqualityComparer<T?> Comparer = new();

        public override int GetHashCode() => Comparer.GetHashCode(this);

        /// <remarks>
        /// Is this necessary? I feel like the default <see cref="object.Equals(object?)"/> method should be able to use generic <see cref="IEquatable{T}"/> implementations automatically...
        /// </remarks>
        public override bool Equals(object? obj) {
            return obj switch {
                IOptional<T> optional => Equals(optional),
                IHas<T> has           => Equals(has),
                T t                   => Equals(t),
                _                     => Equals(this, obj),
            };
        }

        [Pure] public bool Equals(T?             other) => Comparer.Equals(this, other);
        [Pure] public bool Equals(IOptional<T?>? other) => Comparer.Equals(this, other);
        [Pure] public bool Equals(IHas<T?>?      other) => Comparer.Equals(this, other.OrDefault());

        // 📝 NOTE: No longer considering IOptional<T> for OPERATOR comparisons because going between structs and interfaces is clunky and weird.
        [Pure] public static bool operator ==(Optional<T> a, Optional<T> b) => Comparer.Equals(a, b);
        [Pure] public static bool operator !=(Optional<T> a, Optional<T> b) => !Comparer.Equals(a, b);
        [Pure] public static bool operator ==(T?          a, Optional<T> b) => Comparer.Equals(a, b);
        [Pure] public static bool operator !=(T?          a, Optional<T> b) => !Comparer.Equals(a, b);
        [Pure] public static bool operator ==(Optional<T> a, T?          b) => Comparer.Equals(a, b);
        [Pure] public static bool operator !=(Optional<T> a, T?          b) => !Comparer.Equals(a, b);

        #endregion

        #region Formatting

        public override string ToString() {
            return this switch {
                { HasValue: true }  => $"{Value}",
                { HasValue: false } => Optional.EmptyPlaceholder,
            };
        }

        public string Prettify(PrettificationSettings? settings = default) {
            return Optional.ToString(this, new PrettificationSettings());
        }

        #endregion

        #endregion

        public static implicit operator Optional<T>(T value) {
            return new Optional<T>(value);
        }
    }
}