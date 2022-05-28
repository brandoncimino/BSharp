using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp {
    [PublicAPI]
    public static class Bloop {
        #region ForEach

        /// <summary>
        /// Provides extension-method-style <c>foreach</c> loop functionality to <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <remarks>
        /// This is similar to the existing <see cref="List{T}"/>.<see cref="List{T}.ForEach"/>, but works with all <see cref="IEnumerable{T}"/>s.
        /// </remarks>
        /// <param name="enumerable">this <see cref="IEnumerable{T}"/></param>
        /// <param name="action">the <see cref="Action{T}"/> to be performed on each entry of <paramref name="enumerable"/></param>
        /// <typeparam name="T">the type of the entries of <paramref name="enumerable"/></typeparam>
        /// <seealso cref="List{T}.ForEach"/>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var e in enumerable) {
                action.Invoke(e);
            }
        }

        /// <summary>
        /// Executes <see cref="actionWithIndex"/> against each entry in this <see cref="IEnumerable{T}"/> <b>AND</b>
        /// that entry's <see cref="int"/> index.
        /// </summary>
        /// <param name="enumerable">this <see cref="IEnumerable{T}"/></param>
        /// <param name="actionWithIndex">an <see cref="Action{T1,T2}"/> that consumes a <typeparamref name="T"/> entry of <paramref name="enumerable"/> <b>AND</b> its <see cref="int"/> index</param>
        /// <typeparam name="T">the type of each entry in <paramref name="enumerable"/></typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> actionWithIndex) {
            var index = 0;
            foreach (var e in enumerable) {
                actionWithIndex(e, index);
                index += 1;
            }
        }

        #region Try{x} Variations

        public static IEnumerable<Failable>         TryEach<T>(this       IEnumerable<T> enumerable, Action<T>        action)            => enumerable.Select(action.Try);
        public static IEnumerable<Failable>         TryEach<T>(this       IEnumerable<T> enumerable, Action<T, int>   actionWithIndex)   => enumerable.Select(actionWithIndex.Try);
        public static IEnumerable<FailableFunc<T2>> TrySelect<T, T2>(this IEnumerable<T> enumerable, Func<T, T2>      selector)          => enumerable.Select(selector.Try);
        public static IEnumerable<FailableFunc<T2>> TrySelect<T, T2>(this IEnumerable<T> enumerable, Func<T, int, T2> selectorWithIndex) => enumerable.Select(selectorWithIndex.Try);

        #endregion

        public static void ForEach<T1, T2>([InstantHandle] this                     IEnumerable<(T1, T2)>                     enumerable, [InstantHandle] Action<T1, T2>                     action) => enumerable.ForEach(it => action.Invoke(it));
        public static void ForEach<T1, T2, T3>([InstantHandle] this                 IEnumerable<(T1, T2, T3)>                 enumerable, [InstantHandle] Action<T1, T2, T3>                 action) => enumerable.ForEach(it => action.Invoke(it));
        public static void ForEach<T1, T2, T3, T4>([InstantHandle] this             IEnumerable<(T1, T2, T3, T4)>             enumerable, [InstantHandle] Action<T1, T2, T3, T4>             action) => enumerable.ForEach(it => action.Invoke(it));
        public static void ForEach<T1, T2, T3, T4, T5>([InstantHandle] this         IEnumerable<(T1, T2, T3, T4, T5)>         enumerable, [InstantHandle] Action<T1, T2, T3, T4, T5>         action) => enumerable.ForEach(it => action.Invoke(it));
        public static void ForEach<T1, T2, T3, T4, T5, T6>([InstantHandle] this     IEnumerable<(T1, T2, T3, T4, T5, T6)>     enumerable, [InstantHandle] Action<T1, T2, T3, T4, T5, T6>     action) => enumerable.ForEach(it => action.Invoke(it));
        public static void ForEach<T1, T2, T3, T4, T5, T6, T7>([InstantHandle] this IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> enumerable, [InstantHandle] Action<T1, T2, T3, T4, T5, T6, T7> action) => enumerable.ForEach(it => action.Invoke(it));

        #endregion

        #region "Repeat" Extensions

        #region int.Repeat(lambda) NOTE: These are the "real" or "base" methods; the rest of these methods are aliases for these!

        /// <summary>
        /// An extension method version of <see cref="Enumerable"/>.<see cref="Enumerable.Repeat{TResult}"/>.
        /// </summary>
        /// <remarks>
        /// Normally I'd just name the type parameter "<c>T</c>", but I used <typeparamref name="TResult"/> to match <see cref="Repeat{TResult}(int,TResult)"/>.
        /// </remarks>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <param name="element">The value to be repeated.</param>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains a repeated value.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is less than 0.</exception>
        public static IEnumerable<TResult> Repeat<TResult>(this int count, TResult element) {
            return Enumerable.Repeat(element, count);
        }

        /// <summary>
        /// Similar to <see cref="Enumerable.Repeat{TResult}"/>, but evaluates <paramref name="supplier"/> separately for each iteration.
        /// </summary>
        /// <example>
        /// At first I wanted to write:
        ///
        /// <code><![CDATA[
        /// int Total = 0;
        /// void Counter() => Total++;
        /// List<int> Numbers = Enumerable.Repeat(Counter(), 5);
        /// ]]></code>
        ///
        /// Thinking that it would produce <c>{0,1,2,3,4}</c>.
        /// <br/>
        /// However, <c>Counter()</c> will only be evaluated <b>once</b>, and then have <b>that first result</b> repeated,
        /// producing <c>{0,0,0,0,0}</c>.
        /// <br/>
        /// Using <see cref="Repeat{T}(int,System.Func{T})"/> the contents will be evaluated "lazily", once per entry:
        ///
        /// <code><![CDATA[
        /// int Total = 0;
        /// void Counter() => Total++;
        /// List<int> Numbers = Bloop.Repeat(5, () => Counter());
        /// ]]></code>
        /// Will produce <c>{0,1,2,3,4,5}</c>.
        /// </example>
        /// <remarks>
        /// The name "<paramref name="supplier"/>" comes from Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/function/Supplier.html">Supplier</a>.
        /// </remarks>
        /// <param name="count">the number of times to execute the <see cref="Bloop"/></param>
        /// <param name="supplier">a <see cref="Func{TResult}"/> that takes no input and produces an instance of <typeparamref name="T"/></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(this int count, Func<T> supplier) {
            return Enumerable.Repeat(supplier, count).Select(it => it.Invoke());
        }

        /// <summary>
        /// Executes <paramref name="consumer"/> <paramref name="count"/> times, passing the <b>current iteration</b> to <paramref name="consumer"/> each time.
        /// </summary>
        /// <example>
        /// This is essentially the same as a traditional "for" loop, but can be written much more concisely and, I think, intuitively:
        ///
        /// <code><![CDATA[
        /// for(int i=0; i<10; i++){
        ///     Yolo(i);
        /// }
        /// ]]></code>
        ///
        /// Becomes:
        ///
        /// <code><![CDATA[
        /// Bloop.For(10, i => Yolo(i));
        /// ]]></code>
        ///
        /// Or, if you're feeling <i>really</i> frisky:
        ///
        /// <code><![CDATA[
        /// 10.For(i => Yolo(i))
        /// ]]></code>
        /// </example>
        /// <param name="count"></param>
        /// <param name="consumer"></param>
        public static void Repeat(this int count, Action<int> consumer) {
            for (int i = 0; i < count; i++) {
                consumer.Invoke(i);
            }
        }

        /// <summary>
        /// A combination of <see cref="Repeat{T}(int,System.Func{T})"/> and <see cref="Repeat(int,System.Action{int})"/>:
        /// <ul>
        /// <li>Executes <paramref name="loopFunction"/> once per iteration</li>
        /// <li>Passes the current iteration number to <paramref name="loopFunction"/></li>
        /// </ul>
        /// </summary>
        /// <param name="count"></param>
        /// <param name="loopFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [LinqTunnel]
        [Pure]
        public static IEnumerable<T> Repeat<T>(this int count, Func<int, T> loopFunction) {
            return Enumerable.Range(0, count).Select(loopFunction.Invoke);
        }

        /// <summary>
        /// Invokes <paramref name="action"/> <paramref name="count"/> times, without returning any results.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public static void Repeat(this int count, Action action) {
            for (int i = 0; i < count; i++) {
                action.Invoke();
            }
        }

        #endregion

        #region int.For(lambda)

        /**
         * <inheritdoc cref="Repeat{TResult}(int,TResult)"/>
         */
        public static IEnumerable<TResult> For<TResult>(this int iterations, TResult element) {
            return Enumerable.Repeat(element, iterations);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{T})"/>
        public static IEnumerable<T> For<T>(this int iterations, Func<T> supplier) {
            return iterations.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{int,T})"/>
        public static IEnumerable<T> For<T>(this int iterations, Func<int, T> loopFunction) {
            return iterations.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{int,T})"/>
        public static IEnumerable<T> Select<T>(this int iterations, Func<int, T> loopFunction) {
            return iterations.For(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void For(this int iterations, Action action) {
            iterations.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int,Action{int})"/>
        public static void For(this int iterations, Action<int> consumer) {
            iterations.Repeat(consumer);
        }

        #endregion

        #region lambda.Repeat(int)

        /// <inheritdoc cref="Repeat{T}(int, Func{T})"/>
        public static IEnumerable<T> Repeat<T>(this Func<T> supplier, int count) {
            return count.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,Func{int,T})"/>
        public static IEnumerable<T> Repeat<T>(this Func<int, T> loopFunction, int count) {
            return count.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void Repeat(this Action action, int count) {
            count.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int, Action{int})"/>
        public static void Repeat(this Action<int> consumer, int count) {
            count.Repeat(consumer);
        }

        #endregion

        #region lambda.For(int)

        /// <inheritdoc cref="Repeat{T}(int,System.Func{T})"/>
        public static IEnumerable<T> For<T>(this Func<T> supplier, int count) {
            return count.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{int,T})"/>
        public static IEnumerable<T> For<T>(this Func<int, T> loopFunction, int count) {
            return count.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void For(this Action action, int count) {
            count.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int,Action{int})"/>
        public static void For(this Action<int> action, int count) {
            count.Repeat(action);
        }

        #endregion

        #endregion

        #region Stepping through a range

        /// <summary>
        /// Cuts <see cref="distance"/> into <see cref="stepCount"/> equal parts.
        /// </summary>
        /// <remarks>
        /// This does <b>not</b> include a point at <see cref="distance"/>.
        /// </remarks>
        /// <param name="distance"></param>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public static IEnumerable<float> StepExclusive(float distance, int stepCount) {
            var stepSize = distance / stepCount;
            return stepCount.For(it => it * stepSize);
        }

        /// <inheritdoc cref="StepExclusive(float,int)"/>
        public static IEnumerable<double> StepExclusive(double distance, int stepCount) {
            var stepSize = distance / stepCount;
            return stepCount.For(it => it * stepSize);
        }

        /// <summary>
        /// Cuts (<see cref="max_exclusive"/> - <see cref="min_inclusive"/>) into <see cref="stepCount"/> equal parts.
        /// </summary>
        /// <remarks>
        /// This does <b>not</b> include a point at <see cref="max_exclusive"/>.
        /// </remarks>
        /// <example>
        /// <see cref="StepExclusive(float,float,int)">StepExclusive(10,30,5)</see>:
        /// <code>
        /// Distance:   30 - 10 = 20
        /// Step size:  20 / 5  =  4
        /// Results:    10, 14, 18, 22, 26
        /// </code>
        /// </example>
        /// <param name="min_inclusive"></param>
        /// <param name="max_exclusive"></param>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public static IEnumerable<float> StepExclusive(float min_inclusive, float max_exclusive, int stepCount) {
            var distance = max_exclusive - min_inclusive;
            return StepExclusive(distance, stepCount).Select(it => it + min_inclusive);
        }

        /// <inheritdoc cref="StepExclusive(float,float,int)"/>
        public static IEnumerable<double> StepExclusive(double min_inclusive, double max_exclusive, int stepCount) {
            var distance = max_exclusive - min_inclusive;
            return StepExclusive(distance, stepCount).Select(it => it + min_inclusive);
        }

        /// <summary>
        /// Cuts <see cref="distance"/> into <see cref="stepCount"/> parts, and returns the points <b>on each side of those parts</b> (including the "bookend" at <see cref="distance"/>).
        /// </summary>
        /// <remarks>
        /// This will return <b><see cref="stepCount"/>+1 points</b>, because it includes the "bookends" at both 0 and <see cref="distance"/>.
        /// </remarks>
        /// <seealso cref="StepExclusive(float,float,int)"/>
        /// <example>
        /// <b>Example 1</b><p/>
        /// If a staircase of height 10 was being built with 5 steps, then this would return the height of <b>each flat surface</b>:
        /// <code><![CDATA[
        /// StepInclusive(10, 5);
        ///
        /// #1:  0      // the "ground floor"
        /// #2:  2
        /// #3:  4
        /// #4:  6
        /// #5:  8
        /// #6: 10      // the "next floor"
        /// ]]></code>
        ///
        /// <b>Example 2</b><p/>
        /// If (one axis of) a grid of size <see cref="distance"/> was being drawn with <see cref="stepCount"/> boxes, then this would return the distance where <b>each line should be drawn</b>.
        /// </example>
        /// <param name="distance"></param>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public static IEnumerable<float> StepInclusive(float distance, int stepCount) {
            var stepSize = distance / stepCount;
            return (stepCount + 1).For(it => it * stepSize);
        }

        /**
         * <inheritdoc cref="StepInclusive(float,int)"/>
         */
        public static IEnumerable<double> StepInclusive(double distance, int stepCount) {
            var stepSize = distance / stepCount;
            return (stepCount + 1).For(it => it * stepSize);
        }

        /// <summary>
        /// Similar to <see cref="StepInclusive(float,int)"/>, but goes from <see cref="min_inclusive"/> to <see cref="max_inclusive"/>.
        /// </summary>
        /// <param name="min_inclusive"></param>
        /// <param name="max_inclusive"></param>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public static IEnumerable<float> StepInclusive(float min_inclusive, float max_inclusive, int stepCount) {
            var distance = max_inclusive - min_inclusive;
            return StepInclusive(distance, stepCount).Select(it => it + min_inclusive);
        }

        /**
         * <inheritdoc cref="StepInclusive(float,float,int)"/>
         */
        public static IEnumerable<double> StepInclusive(double min_inclusive, double max_inclusive, int stepCount) {
            var distance = max_inclusive - min_inclusive;
            return StepInclusive(distance, stepCount).Select(it => it + min_inclusive);
        }

        #endregion
    }
}