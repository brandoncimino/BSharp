using System;
using System.Collections.Generic;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    [PublicAPI]
    public static class TestUtils {
        public const           double   ApproximationThreshold       = 0.001;
        public const           double   ApproximationThreshold_Loose = 0.005;
        public const           long     ApproximationTickThreshold   = (long)(TimeSpan.TicksPerSecond * ApproximationThreshold_Loose);
        public static readonly TimeSpan ApproximationTimeThreshold   = TimeSpan.FromTicks(ApproximationTickThreshold);

        /// <summary>
        /// An extension method, intended to be called against <see cref="NUnit.Framework.Has"/>, to apply the <see cref="AllValuesConstraint"/>.
        /// </summary>
        /// <param name="constraintExpression"></param>
        /// <returns></returns>
        public static ConstraintExpression Values(this ConstraintExpression constraintExpression) {
            return constraintExpression.Append(new ValuesOperator());
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, object expectedValue, object threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold, clusivity)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, object expectedValue, object threshold) {
            return constraintExpression.Approximately(expectedValue, threshold);
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, object expectedValue) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, object expectedValue) {
            return constraintExpression.Approximately(expectedValue);
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, DateTime expectedValue, TimeSpan threshold) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, DateTime expectedValue, TimeSpan threshold) {
            return constraintExpression.Approximately(expectedValue, threshold);
        }

        /// <summary>
        /// Converts <paramref name="original"/> to a JSON and then back into a <typeparamref name="T"/> to ensure that
        /// no information is lost.
        /// </summary>
        /// <param name="original">the <typeparamref name="T"/> instance we started with</param>
        /// <param name="simulacra">the number of recursive simulacra to be produced</param>
        /// <typeparam name="T">the type of the <paramref name="original"/></typeparam>
        /// <returns>the de-serialized simulacrum of <paramref name="original"/>, in case you want it</returns>
        public static T SerialCompare<T>(T original, int simulacra = 3) where T : IEquatable<T> {
            T simulacrum = default;
            simulacra.Repeat(
                i => {
                    Console.WriteLine($"Simulacrum [#{i + 1}/{simulacra}]");

                    var json = JsonConvert.SerializeObject(original);
                    simulacrum = JsonConvert.DeserializeObject<T>(json);

                    Console.WriteLine(
                        new Dictionary<string, object> {
                            [nameof(original)]   = original,
                            [nameof(json)]       = json,
                            [nameof(simulacrum)] = simulacrum
                        }.Prettify()
                    );
                    Console.WriteLine();

                    AssertAll.Of(
                        () => Assert.That(original, NUnit.Framework.Is.EqualTo(simulacrum)),
                        () => Assert.True(original.Equals(simulacrum), "original.Equals(simulacrum)")
                    );
                }
            );
            return simulacrum;
        }
    }
}