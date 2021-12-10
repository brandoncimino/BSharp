using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Utility and extension methods for <see cref="Optional{T}"/>.
    /// </summary>
    [PublicAPI]
    public static class Optional {
        internal const string NullPlaceholder  = "⛔";
        internal const string EmptyPlaceholder = "🈳";

        /// <summary>
        /// Creates an <see cref="Optional{T}"/> without ugly type parameters.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Optional<T> Of<T>(T value) {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Similar to <see cref="Of{T}"/>, but will treat a null <typeparamref name="T"/> <paramref name="value"/> as <see cref="Empty{T}"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [ItemNotNull]
        public static Optional<T> OfNullable<T>(T? value) {
            return value == null ? default : Of(value);
        }

        /// <summary>
        /// A more verbose way to say <c>"default"</c>, if that's what you're into.
        /// </summary>
        /// <typeparam name="T">the underlying type of the <see cref="Optional{T}"/></typeparam>
        /// <returns>an empty <see cref="Optional{T}"/></returns>
        [ItemNotNull]
        public static Optional<T> Empty<T>() {
            return default;
        }

        /// <summary>
        /// Executes <paramref name="predicate"/>, and if it returns <c>true</c>, returns the result of <paramref name="supplier"/>.
        /// </summary>
        /// <remarks>
        /// It is possible...sleepy
        /// TODO: Should this be called `GetIf`...? That sounds more grammatically correct, but the parameters are ordered <paramref name="predicate"/> then <paramref name="supplier"/>...
        /// </remarks>
        /// <param name="predicate"></param>
        /// <param name="supplier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [ItemNotNull]
        public static Optional<T> IfGet<T>([InstantHandle] Func<bool> predicate, Func<T> supplier) {
            return predicate.Invoke() ? Of(supplier.Invoke()) : default;
        }

        /// <summary>
        /// Converts an <see cref="IOptional{T}"/> to an <see cref="Optional{T}"/>.
        /// </summary>
        /// <remarks>
        /// Technically, this creates a "new" <see cref="Optional{T}"/> even of the input <paramref name="optional"/> was already an <see cref="Optional{T}"/> -
        /// this is to mimic the usage of <see cref="Enumerable.ToList{TSource}"/>, etc., which always return new objects.
        ///
        /// However, because <see cref="Optional{T}"/> is a <see cref="ValueType"/>, this isn't a particularly meaningful distinction.
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <typeparam name="T">the type of the underlying value in <paramref name="optional"/></typeparam>
        /// <returns>a new <see cref="Optional{T}"/> containing the same <see cref="IOptional{T}.Value"/> as <paramref name="optional"/>; or an empty <see cref="Optional{T}"/> if <paramref name="optional"/> was null</returns>
        public static Optional<T> ToOptional<T>(this IOptional<T> optional) {
            return optional switch {
                null => default,
                _    => optional.Select(Of).OrElse(default)
            };
        }

        /// <summary>
        /// "Collects" an <see cref="IEnumerable{T}"/> as an <see cref="Optional{T}"/>.
        ///
        /// This works similarly to <see cref="Enumerable.Single{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>, except:
        ///
        /// <ul>
        /// <li>If <see cref="source"/> is empty ⇒ return an empty <see cref="Optional{T}"/>.</li>
        /// <li>If <see cref="source"/> is <c>null</c> ⇒ return an empty <see cref="Optional{T}"/>.</li>
        /// <li>If <see cref="source"/> has more than 1 entry ⇒ throw an <see cref="InvalidOperationException"/>.</li>
        /// </ul>
        ///
        /// </summary>
        /// <param name="source">an <see cref="IEnumerable{T}"/> with 0 or 1 elements</param>
        /// <typeparam name="T">the type of <see cref="source"/></typeparam>
        /// <returns>an <see cref="Optional{T}"/> containing the 0 or 1 elements of <see cref="source"/></returns>
        /// <exception cref="InvalidOperationException">If <see cref="source"/> has more than 1 element</exception>
        /// <remarks>
        /// Corresponds to Guava's <a href="https://guava.dev/releases/21.0/api/docs/com/google/common/collect/MoreCollectors.html#toOptional--">MoreCollectors.toOptional()</a>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">if <paramref name="source"/> contains <b>more than one element</b>.</exception>
        public static Optional<T?> ToOptional<T>([InstantHandle] this IEnumerable<T?>? source) {
            if (source == null) {
                return default;
            }

            var ls = source.ToList();
            return ls.Any() ? Of(ls.Single()) : default;
        }

        #region Flatten

        /// <summary>
        /// Reduces a nested <see cref="Optional{T}"/> of <see cref="Optional{T}"/>s into a simple <see cref="Optional{T}"/>.
        /// </summary>
        /// <example>
        /// <code><![CDATA[Optional.Of(Optional.Of(Optional.Of(5))).Flatten() == Optional.Of(5)]]></code>
        /// </example>
        /// <param name="optional">a <a href="https://en.wikipedia.org/w/index.php?title=%D0%9C%D0%B0%D1%82%D1%80%D1%91%D1%88%D0%BA%D0%B0">матрёшка</a> <see cref="Optional{T}"/></param>
        /// <typeparam name="T">the "most reduced" type of the <see cref="Optional{T}"/></typeparam>
        /// <returns>a single-tier <see cref="Optional{T}"/></returns>
        [Pure]
        public static Optional<T?> Flatten<T>(this Optional<Optional<T?>> optional) => optional.OrDefault();

        /**
         * <inheritdoc cref="Flatten{T}(BrandonUtils.Standalone.Optional.Optional{BrandonUtils.Standalone.Optional.Optional{T}})"/>
         */
        [Pure]
        public static Optional<T?> Flatten<T>(this Optional<Optional<Optional<T?>>> optional) => optional.OrDefault().Flatten();

        /**
         * <inheritdoc cref="Flatten{T}(BrandonUtils.Standalone.Optional.Optional{BrandonUtils.Standalone.Optional.Optional{T}})"/>
         */
        [Pure]
        public static Optional<T?> Flatten<T>(this Optional<Optional<Optional<Optional<T?>>>> optional) => optional.OrDefault().Flatten();

        /**
         * <inheritdoc cref="Flatten{T}(BrandonUtils.Standalone.Optional.Optional{BrandonUtils.Standalone.Optional.Optional{T}})"/>
         */
        [Pure]
        public static Optional<T?> Flatten<T>(this Optional<Optional<Optional<Optional<Optional<T?>>>>> optional) => optional.OrDefault().Flatten();

        #endregion

        [Pure]
        [LinqTunnel]
        public static IOptional<TOut> Select<TIn, TOut>(
            this IOptional<TIn> optional,
            Func<TIn, TOut>     selector
        ) {
            return optional.AsEnumerable().Select(selector).ToOptional();
        }

        /// <summary>
        /// Provides a "default" <see cref="object.ToString"/> method for <see cref="IOptional{T}"/> implementations to use.
        /// </summary>
        /// <param name="optional">an <see cref="IOptional{T}"/></param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="IOptional{T}"/></typeparam>
        /// <returns>a <see cref="object.ToString"/> representation of the given <see cref="IOptional{T}"/></returns>
        public static string ToString<T>(IOptional<T?>? optional, PrettificationSettings? settings) {
            var realType   = optional?.GetType() ?? typeof(T);
            var prettyType = realType.Prettify(settings);
            if (optional == null) {
                return $"({prettyType}){NullPlaceholder}";
            }
            else {
                var valueString = optional.HasValue ? optional.Value.Prettify(settings) : EmptyPlaceholder;
                return $"{prettyType}[{valueString}]";
            }
        }

        #region Failables

        #region FailableFunc

        /// <summary>
        /// Attempts to <see cref="Func{TResult}.Invoke"/> <see cref="functionThatMightFail"/>, returning a <see cref="FailableFunc{TValue}"/>
        /// that contains either the successful result or the <see cref="IFailableFunc{TValue}.Excuse"/> for failure.
        /// </summary>
        /// <param name="functionThatMightFail"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <example>
        /// TODO: Add an example, but I'm tired right now and when I started writing one instead made <see cref="DayOfWeekExtensions.IsSchoolNight"/>
        /// </example>
        public static FailableFunc<T> Try<T>([InstantHandle] this Func<T> functionThatMightFail) {
            if (functionThatMightFail == null) {
                throw new ArgumentNullException(nameof(functionThatMightFail));
            }

            return new FailableFunc<T>(functionThatMightFail);
        }

        public static FailableFunc<TOut> Try<TIn, TOut>(
            this Func<TIn, TOut> functionThatMightFail,
            TIn                  input
        ) {
            return new FailableFunc<TOut>(() => functionThatMightFail.Invoke(input));
        }

        public static FailableFunc<TOut> Try<T1, T2, TOut>(
            this Func<T1, T2, TOut> functionThatMightFail,
            T1                      arg1,
            T2                      arg2
        ) {
            return new FailableFunc<TOut>(() => functionThatMightFail.Invoke(arg1, arg2));
        }

        public static FailableFunc<TOut> Try<T1, T2, T3, TOut>(
            this Func<T1, T2, T3, TOut> functionThatMightFail,
            T1                          arg1,
            T2                          arg2,
            T3                          arg3
        ) {
            return new FailableFunc<TOut>(() => functionThatMightFail.Invoke(arg1, arg2, arg3));
        }

        #endregion

        #region Failable Action

        /// <summary>
        /// Attempts to <see cref="Action.Invoke"/> <see cref="actionThatMightFail"/>, returning a <see cref="Failable"/>
        /// that (might) contain the <see cref="IFailableFunc{TValue}.Excuse"/> for failure.
        /// </summary>
        /// <param name="actionThatMightFail">the <see cref="Action"/> being executed</param>
        /// <returns>a <see cref="Failable"/> containing information about execution of the <paramref name="actionThatMightFail"/></returns>
        public static Failable Try([InstantHandle] this Action actionThatMightFail) => new Failable(actionThatMightFail);

        public static Failable Try<T>([InstantHandle] this                  Action<T>                  actionThatMightFail, T  argument)                                 => new Failable(() => actionThatMightFail.Invoke(argument));
        public static Failable Try<T1, T2>([InstantHandle] this             Action<T1, T2>             actionThatMightFail, T1 arg1, T2 arg2)                            => new Failable(() => actionThatMightFail.Invoke(arg1, arg2));
        public static Failable Try<T1, T2, T3>([InstantHandle] this         Action<T1, T2, T3>         actionThatMightFail, T1 arg1, T2 arg2, T3 arg3)                   => new Failable(() => actionThatMightFail.Invoke(arg1, arg2, arg3));
        public static Failable Try<T1, T2, T3, T4>([InstantHandle] this     Action<T1, T2, T3, T4>     actionThatMightFail, T1 arg1, T2 arg2, T3 arg3, T4 arg4)          => new Failable(() => actionThatMightFail.Invoke(arg1, arg2, arg3, arg4));
        public static Failable Try<T1, T2, T3, T4, T5>([InstantHandle] this Action<T1, T2, T3, T4, T5> actionThatMightFail, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => new Failable(() => actionThatMightFail.Invoke(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Attempts to execute this <see cref="Action"/>, capturing <see cref="Exception"/>s and returning a <see cref="Timeable"/>
        /// that describes what happened.
        /// </summary>
        /// <param name="actionThatMightFail"></param>
        /// <returns></returns>
        public static Timeable TryTimed([InstantHandle] this Action actionThatMightFail) {
            return new Timeable(actionThatMightFail);
        }

        #endregion

        #endregion

        #region GetValueOrDefault

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/>; otherwise, returns <see cref="fallback"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.GetValueOrDefault()"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElse-T-">Optional.orElse()</a></li>
        /// </ul>
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="fallback">the return value if this <see cref="CollectionUtils.IsEmpty{T}"/></param>
        /// <returns><see cref="IOptional{T}.Value"/> if it this <see cref="IOptional{T}.HasValue"/>; otherwise, returns <see cref="fallback"/>.</returns>
        public static T? GetValueOrDefault<T>(this IOptional<T?>? optional, T? fallback) {
            return optional?.HasValue == true ? optional.Value : fallback;
        }

        /**
         * <inheritdoc cref="GetValueOrDefault{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
         */
        public static T? OrElse<T>(this IOptional<T?>? optional, T? fallback) {
            return optional.GetValueOrDefault(fallback);
        }

        public static T? OrDefault<T>(this IOptional<T?>? optional) {
            return optional.OrElse(default);
        }

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if it is present; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// Corresponds to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElseGet-java.util.function.Supplier-">Optional.orElseGet()</a>.
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="fallbackSupplier">a <see cref="Func{TResult}"/> that produces a <see cref="T"/> if <see cref="IOptional{T}.Value"/> isn't present</param>
        /// <returns><see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/>; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/></returns>
        public static T? GetValueOrDefault<T>(this IOptional<T?>? optional, Func<T> fallbackSupplier) {
            if (fallbackSupplier == null) {
                throw new ArgumentNullException(nameof(fallbackSupplier));
            }

            return optional?.HasValue == true ? optional.Value : fallbackSupplier.Invoke();
        }

        /**
         * <inheritdoc cref="GetValueOrDefault{T}(BrandonUtils.Standalone.Optional.IOptional{T},Func{T})"/>
         */
        public static T? OrElseGet<T>(this IOptional<T?>? optional, Func<T> fallbackSupplier) {
            return optional.GetValueOrDefault(fallbackSupplier);
        }

        #endregion

        #region AreEqual

        /// <summary>
        /// Tests the underlying values of <see cref="a"/> and <see cref="b"/> for equality.
        /// <ul>
        /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <b>empty</b>, then they are considered <b>equal</b>.</li>
        /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <c>null</c>, then they are considered <b>equal</b>.</li>
        /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> <i><b>contain</b></i> <c>null</c>, then they are considered <b>equal</b>.</li>
        /// <li>A <c>null</c> <see cref="IOptional{T}"/> is <b><i>NOT</i></b> considered equal to an <see cref="IOptional{T}"/> with a <c>null</c> <see cref="IOptional{T}.Value"/>!</li>
        /// </ul>
        /// </summary>
        /// <remarks>
        /// I made this so that I had the same logic across all of the different <see cref="IEquatable{T}"/> and <c>==</c>
        /// operator comparisons in <see cref="Optional{T}"/>, <see cref="FailableFunc{TValue}"/>, etc.
        /// <p/>
        /// In fancy language, this method provides a default equality comparator for <see cref="IOptional{T}"/> implementations.
        /// </remarks>
        /// <param name="a">aka "x", aka "left-hand side"</param>
        /// <param name="b">aka "y", aka "right-hand side"</param>
        /// <typeparam name="T">the actual type of the underlying <see cref="IOptional{T}.Value"/>s</typeparam>
        /// <returns>the equality of the underlying <see cref="IOptional{T}.Value"/>s of <see cref="a"/> and <see cref="b"/></returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        /// <seealso cref="AreEqual{T}(T,BrandonUtils.Standalone.Optional.IOptional{T})"/>
        [ContractAnnotation("a:null, b:null => true")]
        [ContractAnnotation("a:null, b:notnull => false")]
        [ContractAnnotation("a:notnull, b:null => false")]
        public static bool AreEqual<T>(IOptional<T>? a, IOptional<T>? b) {
            // return true if EITHER:
            // - a & b are the same object, OR
            // - a & b are both null
            if (ReferenceEquals(a, b)) {
                return true;
            }

            // since at this point we know that they can't _both_ be null, if _either_ of them is null, return false
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }

            // if a & b are both EITHER:
            // - not empty, OR
            // - empty
            if (a.HasValue == b.HasValue) {
                // if a is empty, then b is empty, and two empties are considered to match, so we return TRUE
                // otherwise, we compare the actual values
                return !a.HasValue || Equals(a.Value, b.Value);
            }

            return false;
        }

        /// <summary>
        /// Compares the <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/> to a vanilla <typeparamref name="T"/> value
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>A null <see cref="IOptional{T}"/> should <b>not</b> be considered equal to a null <typeparamref name="T"/></li>
        /// </ul>
        /// </remarks>
        /// <param name="a">an <see cref="IOptional{T}"/></param>
        /// <param name="b">a vanilla <typeparamref name="T"/> value</param>
        /// <typeparam name="T">the underlying type being compared</typeparam>
        /// <returns>the equality of (<paramref name="a"/>.<see cref="IOptional{T}.Value"/>) and (<paramref name="b"/>)</returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},BrandonUtils.Standalone.Optional.IOptional{T})"/>
        /// <seealso cref="AreEqual{T}(T,BrandonUtils.Standalone.Optional.IOptional{T})"/>
        [ContractAnnotation("a:null => false")]
        public static bool AreEqual<T>(IOptional<T>? a, T? b) {
            // this method compares the _value_ of `a` to `b`, which means a value has to exist
            if (!(a is { HasValue: true })) {
                return false;
            }

            return a.HasValue && Equals(a.Value, b);
        }

        /// <summary>
        /// Compares a vanilla <typeparamref name="T"/> value to the underlying <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/>.
        /// </summary>
        /// <remarks>
        /// Behind the scenes, this flips <paramref name="a"/> and <paramref name="b"/> and sends them to <see cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        /// </remarks>
        /// <param name="a">a vanilla <typeparamref name="T"/> value</param>
        /// <param name="b">an <see cref="IOptional{T}"/></param>
        /// <typeparam name="T">the underlying type being compared</typeparam>
        /// <returns>the equality of (<paramref name="a"/>) and (<paramref name="b"/>.<see cref="IOptional{T}.Value"/>)</returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},BrandonUtils.Standalone.Optional.IOptional{T})"/>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        [ContractAnnotation("b:null => false")]
        public static bool AreEqual<T>(T? a, IOptional<T>? b) {
            return AreEqual(b, a);
        }

        #endregion AreEqual

        #region IfPresent

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, <see cref="Action{T}.Invoke"/> <paramref name="actionIfPresent"/>.
        /// Otherwise, do nothing.
        /// </summary>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="actionIfPresent">an action performed on <see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/></param>
        /// <typeparam name="T">the underlying type of this <see cref="IOptional{T}"/></typeparam>
        public static void IfPresent<T>(this IOptional<T>? optional, Action<T> actionIfPresent) {
            if (optional?.HasValue == true) {
                actionIfPresent.Invoke(optional.Value);
            }
        }

        /**
         * <inheritdoc cref="IfPresent{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Action{T})"/>
         */
        public static void IfPresent<T>(this T? nullable, Action<T> actionIfPresent) where T : struct {
            if (nullable.HasValue) {
                actionIfPresent.Invoke(nullable.Value);
            }
        }

        #endregion

        #region IfPresentOrElse

        /**
         * <inheritdoc cref="IfPresentOrElse{TIn,TOut}(IOptional{TIn},System.Func{TIn,TOut},System.Func{TOut})"/>
         */
        public static TOut IfPresentOrElse<TIn, TOut>(this TIn? nullable, Func<TIn, TOut> ifPresent, Func<TOut> orElse) where TIn : struct {
            return nullable.HasValue ? ifPresent.Invoke(nullable.Value) : orElse.Invoke();
        }

        /**
         * <inheritdoc cref="IfPresentOrElse{TIn,TOut}(IOptional{TIn},System.Func{TIn,TOut},System.Func{TOut})"/>
         */
        public static void IfPresentOrElse<TIn>(this TIn? nullable, Action<TIn> ifPresent, Action orElse) where TIn : struct {
            if (nullable.HasValue) {
                ifPresent.Invoke(nullable.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, return the result of <paramref name="ifPresent"/>.
        /// Otherwise, return the result of <paramref name="orElse"/>.
        /// </summary>
        /// <remarks>
        /// Doesn't mimic one of Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html">Optional</a> methods directly,
        /// but is equivalent to calling:
        /// <code><![CDATA[//JAVA
        /// Optional<Integer> optional;
        ///
        /// optional.map(it -> ifPresent(it))
        ///     .orElseGet(() -> orElse())
        /// ]]></code>
        /// I suppose it could be called "<c>IfPresentOrElseGet</c>", but that's silly
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="ifPresent">the "mapping" <see cref="Func{TIn, TResult}"/> to run if this <see cref="IOptional{T}.HasValue"/></param>
        /// <param name="orElse">the <see cref="Func{TResult}"/> that generates the "default" / "fallback" value when this <see cref="IsEmpty{T}"/></param>
        /// <typeparam name="TIn">the underlying type of the original <see cref="IOptional{T}"/></typeparam>
        /// <typeparam name="TOut">the resulting type</typeparam>
        /// <returns></returns>
        public static TOut IfPresentOrElse<TIn, TOut>(this IOptional<TIn>? optional, Func<TIn, TOut> ifPresent, Func<TOut> orElse) {
            return optional?.HasValue == true ? ifPresent.Invoke(optional.Value) : orElse.Invoke();
        }

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, executes <paramref name="ifPresent"/>.
        /// Otherwise, executes <paramref name="orElse"/>.
        /// </summary>
        /// <remarks>
        /// Mimics Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html#ifPresentOrElse-java.util.function.Consumer-java.lang.Runnable-">ifPresentOrElse()</a>
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="ifPresent">the <see cref="Action{T}"/> to run if this <see cref="IOptional{T}.HasValue"/></param>
        /// <param name="orElse">the parameterless <see cref="Action"/> to run if this <see cref="IsEmpty{T}"/></param>
        /// <typeparam name="TIn">the underlying type of this <see cref="Optional{T}"/></typeparam>
        public static void IfPresentOrElse<TIn>(this IOptional<TIn>? optional, Action<TIn> ifPresent, Action orElse) {
            if (optional?.HasValue == true) {
                ifPresent.Invoke(optional.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        #endregion IfPresentOrElse

        #region OrElseThrow

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, returns <see cref="IOptional{T}.Value"/>.
        ///
        /// Throws an exception if the <see cref="IOptional{T}"/> is empty.
        /// </summary>
        /// <remarks>
        /// For the <see cref="Nullable{T}"/> equivalent, see <see cref="OrElseThrow{T}(Nullable{T},System.Func{System.Exception})"/>
        /// </remarks>
        /// <param name="optional">an <see cref="IOptional{T}"/></param>
        /// <param name="exceptionProvider">the <see cref="Func{TResult}"/> that generates the exception. Defaults to <see cref="NoCanHasValue{T}"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="IOptional{T}"/></typeparam>
        /// <returns>the <see cref="IOptional{T}.Value"/></returns>
        /// <exception cref="Exception">if the <see cref="IOptional{T}"/> is empty</exception>
        public static T OrElseThrow<T>(this IOptional<T> optional, Func<Exception>? exceptionProvider = default) {
            return optional.HasValue ? optional.Value : throw (exceptionProvider?.Invoke() ?? NoCanHasValue(optional));
        }

        /// <summary>
        /// If this <see cref="Nullable{T}.HasValue"/>, returns <see cref="Nullable{T}.Value"/>.
        ///
        /// Throws and exception if the <see cref="Nullable{T}"/> is empty.
        /// </summary>
        /// <remarks>
        /// For the <see cref="Optional{T}"/> equivalent, see <see cref="OrElseThrow{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Func{System.Exception})"/>
        /// </remarks>
        /// <param name="nullable">a <see cref="Nullable{T}"/></param>
        /// <param name="exceptionProvider"><inheritdoc cref="OrElseThrow{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Func{System.Exception})"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="Nullable{T}"/></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception">if the <see cref="Nullable{T}"/> is empty</exception>
        [ContractAnnotation("nullable:null => stop")]
        public static T OrElseThrow<T>(this T? nullable, Func<Exception>? exceptionProvider = default) where T : struct {
            return nullable ?? throw (exceptionProvider?.Invoke() ?? NoCanHasValue<T?>());
        }

        #endregion OrElseThrow

        private static ArgumentNullException NoCanHasValue<T>(T? emptyThing = default) {
            return new ArgumentNullException($"The {typeof(T).Name} was empty!");
        }

        #region IsEmpty

        /// <returns>negation of <see cref="Nullable{T}.HasValue"/></returns>
        [ContractAnnotation("null => true")]
        [ContractAnnotation("notnull => false")]
        public static bool IsEmpty<T>(this T? nullable) where T : struct {
            return !nullable.HasValue;
        }

        #endregion IsEmpty

        #region FirstWithValue

        /// <summary>
        /// Returns the <see cref="Enumerable.First{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> entry from <paramref name="optionals"/>
        /// that <see cref="IOptional{T}.HasValue"/>.
        /// </summary>
        /// <param name="optionals">a sequence of <see cref="Optional{T}"/>s</param>
        /// <typeparam name="T">the underlying type of the <see cref="Optional{T}"/>s</typeparam>
        /// <returns>the first <see cref="Optional{T}"/> that <see cref="Optional{T}.HasValue"/>; or an empty <see cref="Optional{T}"/> if no values were found</returns>
        public static Optional<T> FirstWithValue<T>(
            this IEnumerable<Optional<T>> optionals
        ) {
            return optionals.FirstOrDefault(it => it.HasValue);
        }

        #region 0 args (Func<TOut>)

        public static Optional<TOut> FirstWithValue<TOut>(
            [InstantHandle]
            IEnumerable<Func<Optional<TOut>>> functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return functions.Select(fn => fn.MustNotBeNull().Invoke())
                            .FirstOrDefault(it => it.HasValue);
        }

        public static Optional<TOut> FirstWithValue<TOut>(
            params Func<Optional<TOut>>[] functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return FirstWithValue(functions.AsEnumerable());
        }

        #endregion

        #region 1 arg (Func<TIn, TOut>)

        public static Optional<TOut> FirstWithValue<TIn, TOut>(
            TIn input,
            [InstantHandle]
            IEnumerable<Func<TIn, Optional<TOut>>> functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return functions.Select(fn => fn.MustNotBeNull().Invoke(input))
                            .FirstWithValue();
        }

        public static Optional<TOut> FirstWithValue<TIn, TOut>(
            TIn                                input,
            params Func<TIn, Optional<TOut>>[] functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return FirstWithValue(input, functions.AsEnumerable());
        }

        #endregion

        #region 2 args (Func<T1, T2, TOut>)

        public static Optional<TOut> FirstWithValue<T1, T2, TOut>(
            (T1 arg1, T2 arg2) args,
            [InstantHandle]
            IEnumerable<Func<T1, T2, Optional<TOut>>> functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return functions.Select(fn => fn.MustNotBeNull().Invoke(args))
                            .FirstWithValue();
        }

        public static Optional<TOut> FirstWithValue<T1, T2, TOut>(
            (T1 arg1, T2 arg2)                    args,
            params Func<T1, T2, Optional<TOut>>[] functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return FirstWithValue(args, functions.AsEnumerable());
        }

        #endregion

        #region 3 args (Func<T1, T2, T3, TOut>)

        public static Optional<TOut> FirstWithValue<T1, T2, T3, TOut>(
            (T1 arg1, T2 arg2, T3 arg3) args,
            [InstantHandle]
            IEnumerable<Func<T1, T2, T3, Optional<TOut>>> functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return functions.Select(it => it.Invoke(args))
                            .FirstWithValue();
        }

        public static Optional<TOut> FirstWithValue<T1, T2, T3, TOut>(
            (T1 arg1, T2 arg2, T3 arg3)               args,
            params Func<T1, T2, T3, Optional<TOut>>[] functions
        ) {
            if (functions == null) {
                throw new ArgumentNullException(nameof(functions));
            }

            return FirstWithValue(args, functions.AsEnumerable());
        }

        #endregion

        #region 😱 DYNAMIC

        public static Optional<TResult> FirstWithValue<TDelegate, TResult>(
            IEnumerable<TDelegate> delegates,
            params object[]        inputs
        ) where TDelegate : Delegate {
            foreach (var dg in delegates) {
                var result = dg.DynamicInvoke(inputs);

                if (result is Optional<TResult> { HasValue: true } or) {
                    return or;
                }
            }

            return default;
        }

        #endregion

        #endregion Function Fallbacks
    }
}