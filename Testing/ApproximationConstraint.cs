using System;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// Possible alternative adjectives to "Approximately", in order by silliness:
    /// <ul>
    ///     <li>CloseTo (à la AssertJ's <a href="https://www.javadoc.io/doc/org.assertj/assertj-core/latest/org/assertj/core/api/NumberAssert.html#isCloseTo(ACTUAL,org.assertj.core.data.Offset)">isCloseTo()</a>)</li>
    ///     <li>Roughly</li>
    ///     <li>Almost</li>
    ///     <li>Roundabout(s)</li>
    ///     <li>Basically</li>
    /// </ul>
    /// </summary>
    public class ApproximationConstraint : RangeConstraint {
        private readonly object    ExpectedValue;
        private readonly object    Threshold;
        private readonly object    MinValue;
        private readonly object    MaxValue;
        private readonly Clusivity MinClusivity;
        private readonly Clusivity MaxClusivity;
        private const    string    NUnitDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public override string Description => new[] {
            $"≈ {FormatObject(ExpectedValue)} ± {Threshold}",
            $"\tRange: {ClusivityUtils.FormatRange(MinValue, MinClusivity, MaxValue, MaxClusivity)}"
        }.JoinLines();

        public ApproximationConstraint(
            object    expectedValue,
            object    threshold,
            Clusivity minClusivity,
            Clusivity maxClusivity
        ) : base(
            (IComparable)Coercively.Subtract(expectedValue, threshold),
            (IComparable)Coercively.Add(expectedValue, threshold)
        ) {
            ExpectedValue = expectedValue;
            Threshold     = threshold;
            MinValue      = Coercively.Subtract(ExpectedValue, Threshold);
            MaxValue      = Coercively.Add(ExpectedValue, Threshold);
            MinClusivity  = minClusivity;
            MaxClusivity  = maxClusivity;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual) {
            var minCompare = ComparisonAdapter.Default.Compare(MinValue, actual);
            var minCheck   = MinClusivity == Clusivity.Inclusive ? minCompare <= 0 : minCompare < 0;
            var maxCompare = ComparisonAdapter.Default.Compare(MaxValue, actual);
            var maxCheck   = MaxClusivity == Clusivity.Inclusive ? maxCompare >= 0 : maxCompare > 0;
            var isSuccess  = minCheck && maxCheck;
            return new ConstraintResult(this, actual, isSuccess);
        }

        public ApproximationConstraint(
            object    expectedValue,
            object    threshold,
            Clusivity clusivity = Clusivity.Inclusive
        ) : this(
            expectedValue,
            threshold,
            clusivity,
            clusivity
        ) { }

        public ApproximationConstraint(object expectedValue) : this(
            expectedValue,
            GetDefaultThreshold(expectedValue)
        ) { }

        public ApproximationConstraint(DateTime expectedValue, TimeSpan? threshold = default, Clusivity clusivity = Clusivity.Inclusive) : this((object)expectedValue, threshold) { }

        /// <summary>
        /// Formats <paramref name="obj"/> in a similar style to NUnit's "MsgUtils" (which, unfortunately, is an <see langword="internal"/> class...)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string FormatObject(object obj) {
            return obj switch {
                DateTime date => date.ToString(NUnitDateTimeFormat),
                _             => obj.ToString()
            };
        }

        private static object GetDefaultThreshold(object expectedValue) {
            return expectedValue switch {
                DateTime dt => TestUtils.ApproximationTimeThreshold,
                TimeSpan ts => TestUtils.ApproximationTimeThreshold,
                _           => TestUtils.ApproximationThreshold
            };
        }
    }
}