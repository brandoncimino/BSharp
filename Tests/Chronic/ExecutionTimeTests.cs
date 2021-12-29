using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using FowlFever.BSharp;
using FowlFever.BSharp.Chronic;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Randomization;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using Is = FowlFever.Testing.Is;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace BSharp.Tests.Chronic {
    public class ExecutionTimeTests {
        [Test]
        public void SingleActionTime() {
            Ignore.This("I need to accept that I don't understand enough about computers to properly measure method efficiency 😢");
            Action action = Quick;

            //stopwatch version
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            Console.WriteLine($"Stopwatch version: {stopwatch.Elapsed:g}");

            var exTime = new ExecutionTime(action);
            Console.WriteLine($"{nameof(exTime)}: {exTime}");
            Asserter.Against(exTime)
                    .And(Has.Property(nameof(exTime.Duration)).CloseTo(SlowSpan, TimeSpan.FromSeconds(0.01)))
                    .And(it => it.Execution.Failed, Is.EqualTo(false))
                    .And(Has.Property(nameof(exTime.Duration)).CloseTo(stopwatch.Elapsed))
                    .Invoke();
        }

        private static readonly TimeSpan QuickSpan = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan SlowSpan  = TimeSpan.FromSeconds(10);

        private static void Quick() {
            Thread.Sleep(QuickSpan);
        }

        private static void Slow() {
            Thread.Sleep(SlowSpan);
        }

        #region Demonstration of timing with List.Contains() vs. Set.Contains()

        /// <summary>
        /// This demonstrates that:
        /// <ul>
        /// <li><see cref="List{T}"/>.<see cref="List{T}.Contains"/> is much slower than <see cref="HashSet{T}"/>.<see cref="HashSet{T}.Contains"/></li>
        /// <li>These speeds are maintained when called via <see cref="Enumerable.Contains{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/></li>
        /// </ul>
        /// </summary>
        [Test]
        public void ListVsSet() {
            Ignore.This("I need to accept that I don't understand enough about computers to properly measure method efficiency 😢");
            const int iterations = 1000;
            var       items      = MakeHugeCollection(iterations).ToArray();
            var       toFind     = items.Random();

            // Using their respecting .Contains() methods

            var list = items.ToList();
            var set  = items.ToHashSet();

            var listTimes = MethodTimer.MeasureExecution(() => ListContains(list, toFind), iterations);
            var setTimes  = MethodTimer.MeasureExecution(() => SetContains(set, toFind),   iterations);

            Console.WriteLine($"list: {listTimes}");
            Console.WriteLine($"set:  {setTimes}");

            // Using the extension Enumerable.Contains()

            list.RandomizeEntries();
            set.RandomizeEntries();

            var listableTimes = MethodTimer.MeasureExecution(() => EnumerableContains(list, toFind), iterations);
            var settableTimes = MethodTimer.MeasureExecution(() => EnumerableContains(set,  toFind), iterations);

            Console.WriteLine($"{nameof(listableTimes)}: {listableTimes}");
            Console.WriteLine($"{nameof(settableTimes)}: {settableTimes}");

            // Actually doing a comparison
            var comparison = new AggregateExecutionComparison(listTimes, setTimes);
            Console.WriteLine(comparison);
            Asserter.Against(comparison)
                    .And(AssertComparison(comparison.Average, 1))
                    .And(AssertComparison(comparison.Total,   1))
                    .Invoke();
        }

        public Asserter<AggregateExecutionComparison.TimeComparison> AssertComparison(
            AggregateExecutionComparison.TimeComparison results,
            int                                         expectedComparison
        ) {
            Constraint ratioConstraint = expectedComparison switch {
                -1 => Is.Positive.And.LessThan(1),
                0  => Is.EqualTo(1),
                1  => Is.Positive.And.GreaterThan(1),
                _  => throw new ArgumentOutOfRangeException(nameof(expectedComparison))
            };

            return Asserter.Against(results)
                           .And(it => it.First.CompareTo(it.Second), Is.EqualTo(expectedComparison))
                           .And(it => it.Difference.Sign(),          Is.EqualTo(expectedComparison))
                           .And(it => it.Ratio,                      ratioConstraint);
        }

        public class HasGuid {
            public Guid Guid;

            protected bool Equals(HasGuid other) {
                return Guid.Equals(other.Guid);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }

                if (ReferenceEquals(this, obj)) {
                    return true;
                }

                if (obj.GetType() != this.GetType()) {
                    return false;
                }

                return Equals((HasGuid)obj);
            }

            public override int GetHashCode() {
                return Guid.GetHashCode();
            }
        }

        private IEnumerable<HasGuid> MakeHugeCollection(int size) {
            return size.Repeat(Guid.NewGuid).Select(it => new HasGuid() { Guid = it });
        }

        public static bool ListContains<T>(List<T> list, T item) {
            return list.Contains(item);
        }

        public static bool SetContains<T>(ISet<T> set, T item) {
            return set.Contains(item);
        }

        public static bool EnumerableContains<T>(IEnumerable<T> enumerable, T item) {
            return enumerable.Contains(item);
        }

        #endregion

        private static void FickleFunc(double successRate01) {
            var roll = Brandom.Gen.Double();
            if (roll > successRate01) {
                throw new BrandonException($"Failed by rolling {roll} with a success rate of {successRate01}");
            }
        }

        [Test]
        [TestCase(12000, 0.33)]
        public void SuccessRate(int iterations, double successRate) {
            var fickleTimes = MethodTimer.MeasureExecution(() => FickleFunc(successRate), iterations);
            Asserter.Against(fickleTimes)
                    .And(Has.Property(nameof(fickleTimes.SuccessRate)).CloseTo(successRate, 0.005))
                    .And(Has.Property(nameof(fickleTimes.Iterations)).EqualTo(iterations))
                    .Invoke();
        }
    }
}