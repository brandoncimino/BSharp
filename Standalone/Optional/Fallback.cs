using System;
using System.Collections;
using System.Collections.Generic;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A "box" for a value that that <i>might</i> have an <see cref="ExplicitValue"/>, but otherwise, has a <see cref="FallbackValue"/>.
    /// </summary>
    /// <remarks>
    /// Can also be thought of as a "default" and "override", i.e.:
    /// <ul>
    /// <li><see cref="FallbackValue"/> -> "default"</li>
    /// <li><see cref="ExplicitValue"/> -> "override"</li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="T">the underlying value's type</typeparam>
    [PublicAPI]
    [JsonObject(MemberSerialization.OptIn)]
    public class Fallback<T> : IOptional<T> {
        /// <summary>
        /// The <see cref="Optional{T}"/> value that might have been <see cref="Set"/>.
        /// </summary>
        [JsonProperty(Order = int.MaxValue)]
        public Optional<T?> ExplicitValue { get; private set; }
        /// <summary>
        /// Whether or not <see cref="ExplicitValue"/> has been <see cref="Set"/>.
        /// </summary>
        public bool HasValue => ExplicitValue.HasValue;

        /// <summary>
        /// <see cref="ExplicitValue"/>, if present; otherwise, <see cref="FallbackValue"/>.
        /// </summary>
        public T Value {
            get => ExplicitValue.OrElseGet(() => FallbackSource == null ? FallbackValue : FallbackSource.Value);
            set => ExplicitValue = value;
        }

        /// <summary>
        /// The value returned when <see cref="ExplicitValue"/> isn't present.
        /// </summary>
        [JsonProperty(Order = int.MinValue)]
        public T? FallbackValue { get; }

        [JsonProperty(IsReference = true, ItemIsReference = true, NullValueHandling = NullValueHandling.Ignore)]
        public Fallback<T>? FallbackSource { get; }

        public int Count => ExplicitValue.Count;

        #region Constructors

        public Fallback(T? fallbackValue = default) {
            FallbackValue  = fallbackValue;
            FallbackSource = null;
        }

        public Fallback(Fallback<T> fallbackFallback) {
            FallbackValue  = default;
            FallbackSource = fallbackFallback;
        }

        #endregion

        #region Factories

        public static Fallback<T> Of(T? fallbackValue) {
            return new Fallback<T>(fallbackValue);
        }


        public static Fallback<T> Of(T? fallbackValue, T? explicitValue) {
            return new Fallback<T>(fallbackValue).Set(explicitValue);
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="ExplicitValue"/>'s <see cref="Optional{T}.Value"/>.
        /// </summary>
        /// <remarks>
        /// Passing <paramref name="explicitValue"/> as <c>null</c> will result in <see cref="ExplicitValue"/> being an <see cref="Optional{T}"/> that <i>contains</i> <c>null.</c>
        ///
        /// If you want to <b>remove</b> the <see cref="ExplicitValue"/>, so that <see cref="FallbackValue"/> is returned instead, use <see cref="Unset"/>.
        /// </remarks>
        /// <param name="explicitValue">the new <see cref="ExplicitValue"/>'s <see cref="Optional{T}.Value"/></param>
        /// <returns>this, for method chaining</returns>
        public Fallback<T> Set(T? explicitValue) {
            ExplicitValue = new Optional<T>(explicitValue);
            return this;
        }

        /// <summary>
        /// Sets <see cref="ExplicitValue"/> to an <b>empty</b> <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns>this, for method chaining</returns>
        public Fallback<T> Unset() {
            ExplicitValue = new Optional<T>();
            return this;
        }

        /// <summary>
        /// Implicitly casts a <see cref="Fallback{T}"/> to a <typeparamref name="T"/> by returning <see cref="Value"/>.
        /// </summary>
        /// <param name="self">this <see cref="Fallback{T}"/></param>
        /// <returns><see cref="Value"/></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is null</exception>
        public static implicit operator T?(Fallback<T> self) {
            if (self == null) {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Value;
        }

        #region GetEnumerator()

        public IEnumerator<T> GetEnumerator() {
            return ExplicitValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)ExplicitValue).GetEnumerator();
        }

        #endregion


        public override string ToString() {
            var t        = typeof(T);
            var settings = new PrettificationSettings();
            return $"{t.Prettify(settings)}[{FallbackValue.Prettify(settings)},{ExplicitValue.Select(it => it.Prettify()).OrElse("")}]";
        }
    }
}