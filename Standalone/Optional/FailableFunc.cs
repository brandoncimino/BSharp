using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <inheritdoc cref="IFailableFunc{TValue}"/>
     */
    [PublicAPI]
    public readonly struct FailableFunc<TValue> : IFailableFunc<TValue>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
        public bool HasValue { get; }

        private readonly Optional<TValue?> _value;
        internal         Optional<TValue>  SafeValue => HasValue ? _value : default;
        public           TValue            Value     => HasValue ? _value.Value : throw FailableException.FailedException(this, _excuse);

        private readonly Optional<Exception?> _excuse;

        /// <returns><see cref="_excuse"/>.<see cref="IOptional{T}.Value"/> if present and non-null; otherwise, returns <see cref="NoExcuseExcuse"/></returns>
        private Exception _getExcuseSafely() => _excuse.OrElse(null) ?? NoExcuseExcuse();

        public Exception Excuse => Failed ? _getExcuseSafely() : throw FailableException.DidNotFailException(this, _value);

        internal Optional<Exception> SafeExcuse => Failed ? _excuse : default;

        public bool Failed => !HasValue;
        public int  Count  => HasValue ? 1 : 0;


        private IEnumerable<TValue?> EnumerableImplementation => HasValue ? Enumerable.Repeat(Value, 1) : Enumerable.Empty<TValue>();


        public FailableFunc(Func<TValue> valueSupplier) {
            try {
                _value   = valueSupplier.Invoke();
                HasValue = true;
                _excuse  = default;
            }
            catch (Exception e) {
                _value   = default;
                HasValue = false;
                _excuse  = e;
            }
        }

        #region Equality

        public IEnumerator<TValue> GetEnumerator() {
            return EnumerableImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override bool Equals(object obj) {
            return obj switch {
                IOptional<TValue> optional => Equals(optional),
                TValue value               => Equals(value),
                _                          => Equals(this, obj)
            };
        }

        public bool Equals(FailableFunc<TValue> other) {
            return _value.Equals(other._value);
        }

        public override int GetHashCode() {
            return _value.GetHashCode();
        }

        public bool Equals(IOptional<TValue> other) {
            return Optional.AreEqual(this, other);
        }

        public bool Equals(TValue other) {
            return Optional.AreEqual(this, other);
        }

        public static bool operator ==(FailableFunc<TValue?> a, IOptional<TValue?>? b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(FailableFunc<TValue?> a, IOptional<TValue?>? b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(FailableFunc<TValue?> a, TValue? b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(FailableFunc<TValue?> a, TValue? b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(TValue? a, FailableFunc<TValue?> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(TValue? a, FailableFunc<TValue?> b) {
            return !Optional.AreEqual(a, b);
        }

        #endregion


        public override string ToString() {
            return Optional.ToString(this, new PrettificationSettings());
        }


        private InvalidOperationException NoExcuseExcuse() {
            return new InvalidOperationException($"This {GetType().PrettifyType(default)} has no {nameof(_excuse)}! This probably means it is a default value, or was created using a no-argument constructor.");
        }
    }
}