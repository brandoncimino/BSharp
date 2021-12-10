using System;
using System.Diagnostics.Contracts;

using BrandonUtils.Standalone.Optional;

namespace BrandonUtils.Standalone.Refreshing {
    /// <summary>
    /// Represents a value that is expensive to compute, so you want to limit how often this occurs - such as once per frame.
    /// <p/>
    /// This consists of three main parts:
    /// <ul>
    /// <li>A <see cref="ValueSupplier"/>, which is the expensive function that you don't want to call all the time</li>
    /// <li>A <see cref="StalenessBasisSupplier"/>, which provides the value we track to determine staleness, such as the current time or current frame</li>
    /// <li>A <see cref="StalenessPredicate"/>, which compares the results of <see cref="StalenessBasisSupplier"/></li>
    /// </ul>
    ///
    /// A good candidate for <see cref="Refreshing{TValue,TStaleness}"/> is a value that:
    /// <ul>
    /// <li>Is expensive to generate</li>
    /// <li>Can become <see cref="Standalone.Refreshing.Freshness.Stale"/>, at which point it will need to be <see cref="Refresh"/>ed</li>
    /// <li>Has a <see cref="StalenessPredicate"/> that is cheap and simple</li>
    /// <li>Has a <see cref="StalenessPredicate"/> that callers don't need to worry about</li>
    /// </ul>
    /// </summary>
    /// <remarks>
    /// A cute alternative to the name "Refreshing" would be "Slacking" or "Slacker", because it:
    /// <ul>
    /// <li>Is <see cref="System.Lazy{T}"/> as often as possible</li>
    /// <li>Works only when you ask it to</li>
    /// </ul>
    /// <b>NOTE:</b> The <see cref="Freshness"/> does <b>not</b> imply anything about the <b>accuracy</b> of the <see cref="Value"/>.
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TStaleness"></typeparam>
    public class Refreshing<TValue, TStaleness> : IEquatable<TValue>
        where TValue : IEquatable<TValue> {
        /// <summary>
        /// The backing field for <see cref="Value"/>; i.e. the latest "cached" <see cref="TValue"/>.
        /// </summary>
        private TValue _storedValue;

        /// <summary>
        /// The "C# style" of retrieving the value from the <see cref="Refreshing{TValue,TStaleness}"/>
        /// (mimicking <see cref="System.Lazy{T}"/>.<see cref="Lazy{T}.Value"/>).
        /// </summary>
        public TValue Value {
            get {
                if (IsStale) {
                    Refresh();
                }

                return _storedValue;
            }
        }

        /// <summary>
        /// The "Java-style" of retrieving the value from the <see cref="Refreshing{TValue,TStaleness}"/>.
        /// </summary>
        /// <returns><see cref="Value"/></returns>
        [Pure]
        public TValue Get() => Value;

        /// <summary>
        /// The number of times that <see cref="RefreshCount"/> has been called
        /// </summary>
        public int RefreshCount => ValueSupplier.InvocationCount;

        /// <summary>
        /// The current <see cref="Freshness"/> of the <see cref="_storedValue"/>.
        /// </summary>
        public Freshness Freshness {
            get {
                if (!PreviousStalenessBasis.HasValue) {
                    return Freshness.Stale;
                }

                return IsStale ? Freshness.Stale : Freshness.Fresh;
            }
        }

        /// <summary>
        /// Whether or not the current <see cref="_storedValue"/> is "stale", and so will be <see cref="Refresh"/>ed the next time <see cref="Value"/> is retrieved.
        /// </summary>
        private bool IsStale {
            get {
                if (!PreviousStalenessBasis.HasValue) {
                    return true;
                }

                var currentStalenessBasis = StalenessBasisSupplier.Invoke();
                return StalenessPredicate.Invoke(PreviousStalenessBasis.Value, currentStalenessBasis);
            }
        }

        /// <summary>
        /// The expensive function that produces <see cref="Standalone.Refreshing.Freshness.Fresh"/> <see cref="Value"/>s.
        /// </summary>
        public CountedFunc<TValue> ValueSupplier { get; }

        /// <summary>
        /// Compares the previous and current <see cref="TStaleness"/> values to determine if <see cref="_storedValue"/> is <see cref="Refreshing.Freshness.Stale"/>.
        /// </summary>
        public Func<TStaleness, TStaleness, bool> StalenessPredicate { get; }

        /// <summary>
        /// The function that provides the <see cref="TStaleness"/> values used to determine <see cref="Freshness"/>.
        /// </summary>
        /// <remarks>
        /// This value is compared to <see cref="PreviousStalenessBasis"/> via <see cref="StalenessPredicate"/>.
        /// </remarks>
        /// <seealso cref="StalenessPredicate"/>
        /// <seealso cref="PreviousStalenessBasis"/>
        public Func<TStaleness> StalenessBasisSupplier { get; }

        /// <summary>
        /// The result of <see cref="StalenessBasisSupplier"/> the last time <see cref="Refresh"/> was called.
        /// </summary>
        /// <remarks>
        /// Would a better name for this be "StalenessBasisAtLastRefresh"? Or is that too verbose?
        /// </remarks>
        public Optional<TStaleness> PreviousStalenessBasis;

        /// <summary>
        /// Constructs a new <see cref="Refreshing{TValue,TStaleness}"/> value.
        /// </summary>
        /// <param name="valueSupplier">the <see cref="ValueSupplier"/></param>
        /// <param name="stalenessBasisSupplier">the <see cref="StalenessBasisSupplier"/></param>
        /// <param name="stalenessPredicate">the <see cref="StalenessPredicate"/></param>
        public Refreshing(
            Func<TValue>                       valueSupplier,
            Func<TStaleness>                   stalenessBasisSupplier,
            Func<TStaleness, TStaleness, bool> stalenessPredicate
        ) {
            ValueSupplier          = valueSupplier;
            StalenessBasisSupplier = stalenessBasisSupplier;
            StalenessPredicate     = stalenessPredicate;
        }

        /// <summary>
        /// Forces an invocation of <see cref="ValueSupplier"/> and stores the result in <see cref="_storedValue"/>, regardless of the current <see cref="Freshness"/>.
        /// </summary>
        /// <remarks>
        /// This method should rarely be called externally, as it bypasses the <see cref="StalenessPredicate"/> and <b>always</b> invokes the <see cref="ValueSupplier"/>.
        /// Most of the time, you should call <see cref="Value"/> instead.
        /// </remarks>
        /// <returns>the result of <see cref="ValueSupplier"/></returns>
        public TValue Refresh() {
            _storedValue           = ValueSupplier.Invoke();
            PreviousStalenessBasis = StalenessBasisSupplier.Invoke();
            return _storedValue;
        }

        public bool Equals(TValue other) {
            return Value.Equals(other);
        }

        /// <returns>the current <see cref="_storedValue"/> <b>without</b> refreshing it.</returns>
        public TValue Peek() {
            return _storedValue;
        }

        /// <summary>
        /// An implicit cast to <see cref="TValue"/> for convenience.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static implicit operator TValue(Refreshing<TValue, TStaleness> self) {
            return self.Value;
        }
    }
}