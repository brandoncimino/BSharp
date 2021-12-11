using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// Wraps a <see cref="List{T}"/> of <see cref="ExecutionTime"/>s and provides some convenient Linq methods and a flexible <see cref="CompareTo"/>.
    /// </summary>
    /// <remarks>
    /// From Brandon on 8/15/2021: What <i>is</i> this...?
    /// <br/>
    /// From Brandon on 10/15/2021: I think I just started making this again.
    /// </remarks>
    [PublicAPI]
    public class AggregateExecutionTime : IComparable<AggregateExecutionTime> {
        public string Nickname { get; }

        public ExecutionTime[] Executions { get; }

        public int Iterations => Executions.Length;

        public long MinTicks => Executions.Min(it => it.Duration.Ticks);

        public TimeSpan Min => TimeSpan.FromTicks(MinTicks);

        public long MaxTicks => Executions.Max(it => it.Duration.Ticks);

        public TimeSpan Max => TimeSpan.FromTicks(MaxTicks);

        public double AverageTicks => Executions.Average(it => it.Duration.Ticks);

        public TimeSpan Average => TimeSpan.FromTicks((long)AverageTicks);

        public long TotalTicks => Executions.Sum(it => it.Duration.Ticks);

        public TimeSpan Total => TimeSpan.FromTicks(TotalTicks);

        public double SuccessRate => (double)Executions.Count(it => it.Execution.Failed == false) / Executions.Length;

        internal AggregateExecutionTime(string nickname, IEnumerable<ExecutionTime> executions) {
            Executions = executions.ToArray();
            Nickname   = string.IsNullOrWhiteSpace(nickname) ? throw new ArgumentNullException(nameof(nickname)) : nickname; //nickname.MustNotBeBlank();
        }

        /// <summary>
        /// Compares <b><i>this</i></b> to <paramref name="other"/> using <see cref="CompareTo"/> calls against multiple properties:
        /// <li><see cref="Min"/></li>
        /// <li><see cref="Max"/></li>
        /// <li><see cref="Average"/></li>
        /// Returns -1 or 1 if <b>any</b> properties return that value and <b>none</b> return the other;
        /// otherwise, returns 0.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(AggregateExecutionTime other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            return Average.CompareTo(other.Average);
            //
            // var minCompare = Min.CompareTo(other.Min);
            // var maxCompare = Max.CompareTo(other.Max);
            // var avgCompare = Average.CompareTo(other.Average);
            //
            // var compares = new int[] { minCompare, maxCompare, avgCompare };
            // Console.WriteLine(
            //     new Dictionary<object, object>() {
            //         [$"Min: {Min}, {other.Min}"]         = minCompare,
            //         [$"Max: {Max}, {other.Max}"]         = maxCompare,
            //         [$"Avg: {Average}, {other.Average}"] = avgCompare
            //     }.Prettify()
            // );
            // if (compares.Any(it => it > 0) && compares.All(it => it >= 0)) {
            //     return 1;
            // }
            // else if (compares.Any(it => it < 0) && compares.All(it => it <= 0)) {
            //     return -1;
            // }
            // else {
            //     return 0;
            // }
        }


        public override string ToString() {
            if (Executions == null) {
                throw new ArgumentNullException(nameof(Executions));
            }

            if (Executions.Contains(null)) {
                throw new ArgumentNullException($"{nameof(Executions)} contained null! {Executions.Prettify()}");
            }

            var table = new Dictionary<object, object>() {
                [nameof(Total)]   = Total,
                [nameof(Average)] = Average,
                [nameof(Min)]     = Min,
                [nameof(Max)]     = Max,
            }.Prettify();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Nickname != null) {
                table = table.PrefixIfMissing("\n").Prefix(Nickname);
            }

            return table;
        }
    }

    public class AggregateExecutionComparison {
        public enum Which {
            First,
            Second,
            Neither
        }

        public readonly AggregateExecutionTime FirstTimes;
        public readonly AggregateExecutionTime SecondTimes;

        public Which Faster => FirstTimes.CompareTo(SecondTimes) switch {
            -1 => Which.First,
            0  => Which.Neither,
            1  => Which.Second,
            _  => throw new ArgumentOutOfRangeException()
        };

        public AggregateExecutionComparison(AggregateExecutionTime firstTimes, AggregateExecutionTime secondTimes) {
            FirstTimes  = firstTimes;
            SecondTimes = secondTimes;
        }

        public readonly struct TimeComparison {
            public readonly (string nickname, TimeSpan duration) First;
            public readonly (string nickname, TimeSpan duration) Second;
            public          TimeSpan                             Difference => First.duration - Second.duration;
            public          double                               Ratio      => First.duration.Divide(Second.duration);
            public          double                               DeltaRatio => Mathb.DeltaRatio(First.duration, Second.duration);

            [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
            public TimeComparison(
                (string nickname, TimeSpan duration) first,
                (string nickname, TimeSpan duration) second
            ) {
                First  = (first.nickname  ?? nameof(First), first.duration);
                Second = (second.nickname ?? nameof(Second), second.duration);
            }

            public override string ToString() {
                return new Dictionary<object, object>() {
                        [First.nickname]     = First.duration,
                        [Second.nickname]    = Second.duration,
                        [nameof(Difference)] = Difference,
                        [nameof(Ratio)]      = Ratio,
                        [nameof(DeltaRatio)] = DeltaRatio
                    }.AsEnumerable()
                     .JoinString(", ");
            }


            public string GetSummary() {
                return First.duration.CompareTo(Second.duration) switch {
                    -1 => $"[{First.nickname}] is {Mathb.DeltaRatio(Second.duration, First.duration):P0} faster than [{Second.nickname}]",
                    0  => $"[{First.nickname}] and [{Second.nickname}] are equally fast",
                    1  => $"[{Second.nickname}] is {Mathb.DeltaRatio(First.duration, Second.duration):P0} faster than [{First.nickname}]",
                    _  => throw new ArgumentOutOfRangeException()
                } + " on average.";
            }
        }

        private TimeComparison Comparing(Func<AggregateExecutionTime, TimeSpan> extractor) {
            return new TimeComparison(
                (FirstTimes.Nickname, extractor.Invoke(FirstTimes)),
                (SecondTimes.Nickname, extractor.Invoke(SecondTimes))
            );
        }

        public TimeComparison Total   => Comparing(it => it.Total);
        public TimeComparison Average => Comparing(it => it.Average);


        public override string ToString() {
            return new Dictionary<object, object>() {
                    [nameof(Total)]   = Total,
                    [nameof(Average)] = Average
                }.Prettify(new PrettificationSettings())
                 .SplitLines()
                 .Append(Total.GetSummary())
                 .JoinLines();
        }
    }
}