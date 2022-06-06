using System;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp {
    /// <summary>
    /// Extension methods that operate on <see cref="Action"/>s and <see cref="Func{TResult}"/>s.
    /// </summary>
    [PublicAPI]
    public static class LambdaExtensions {
        #region Action with Tuple args

        public static void Invoke<T1, T2>(this                     Action<T1, T2>                     action, (T1 arg1, T2 arg2)                                              args) => action.Invoke(args.arg1, args.arg2);
        public static void Invoke<T1, T2, T3>(this                 Action<T1, T2, T3>                 action, (T1 arg1, T2 arg2, T3 arg3)                                     args) => action.Invoke(args.arg1, args.arg2, args.arg3);
        public static void Invoke<T1, T2, T3, T4>(this             Action<T1, T2, T3, T4>             action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4)                            args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4);
        public static void Invoke<T1, T2, T3, T4, T5>(this         Action<T1, T2, T3, T4, T5>         action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)                   args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5);
        public static void Invoke<T1, T2, T3, T4, T5, T6>(this     Action<T1, T2, T3, T4, T5, T6>     action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)          args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6);
        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6, args.arg7);

        #endregion

        #region Func with Tuple args

        [Pure] public static TResult Invoke<T1, T2, TResult>(this Func<T1, T2, TResult> func, (T1 arg1, T2 arg2) args) => func.Invoke(args.arg1, args.arg2);

        [Pure] public static TResult Invoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, (T1 arg1, T2 arg2, T3 arg3) args) => func.Invoke(args.arg1, args.arg2, args.arg3);

        [Pure] public static TResult Invoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4);

        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5);

        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6);

        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6, args.arg7);

        #endregion

        #region Laziness

        /// <summary>
        /// Creates a <see cref="Lazy{T}"/> that will execute an <see cref="Action"/>.
        /// </summary>
        /// <remarks>
        /// The output's <see cref="Lazy{T}.Value"/> - a <c>default</c> <see cref="byte"/> - is meaningless.
        /// It is used because <see cref="Void"/> cannot be used as a type parameter.
        /// </remarks>
        /// <param name="oneTimeAction">an <see cref="Action"/> that will only be executed once</param>
        /// <returns>a new <see cref="Lazy{T}"/></returns>
        public static Lazy<byte> Lazily(this Action oneTimeAction) {
            return new Lazy<byte>(
                () => {
                    oneTimeAction.Invoke();
                    return default;
                }
            );
        }

        /// <inheritdoc cref="Lazily"/>
        public static Lazy<byte> Lazily<T>(this Action<T> oneTimeAction, T input) {
            return new Lazy<byte>(
                () => {
                    oneTimeAction.Invoke(input);
                    return default;
                }
            );
        }

        /// <summary>
        /// Shorthand for new <see cref="Lazy{T}"/>(<paramref name="valueFactory"/>).
        /// </summary>
        /// <param name="valueFactory">a <see cref="Func{TResult}"/> that should only be executed once</param>
        /// <typeparam name="T">the output type of <paramref name="valueFactory"/></typeparam>
        /// <returns>a new <see cref="Lazy{T}"/></returns>
        public static Lazy<T> Lazily<T>(this Func<T> valueFactory) {
            return new Lazy<T>(valueFactory);
        }

        /// <summary>
        /// Shorthand for new <see cref="Lazy{T}"/>(() => <paramref name="valueFactory"/>, <paramref name="input"/>)
        /// </summary>
        /// <param name="valueFactory">a <see cref="Func{TResult}"/> that should only be executed once</param>
        /// <param name="input">the input value to <paramref name="valueFactory"/></param>
        /// <typeparam name="TIn">the type <paramref name="input"/></typeparam>
        /// <typeparam name="TOut">the output type of <paramref name="valueFactory"/></typeparam>
        /// <returns>a new <see cref="Lazy{T}"/></returns>
        public static Lazy<TOut> Lazily<TIn, TOut>(this Func<TIn, TOut> valueFactory, TIn input) {
            return new Lazy<TOut>(() => valueFactory(input));
        }

        #endregion

        #region From "Assertion" (Action<T>)

        public static Func<T, T> ToCheckpoint<T>(this Action<T> action) {
            return it => {
                action(it);
                return it;
            };
        }

        public static Func<T, bool> ToPredicate<T>(this Action<T> action) {
            return it => {
                try {
                    action(it);
                    return true;
                }
                catch {
                    return false;
                }
            };
        }

        #endregion

        #region From "Checkpoint" (Func<T,T>)

        public static Action<T> ToAssertion<T>(this Func<T, T> checkpoint) => obj => checkpoint.Invoke(obj);

        public static Func<T, bool> ToPredicate<T>(this Func<T, T> checkpoint) {
            return it => {
                try {
                    checkpoint(it);
                    return true;
                }
                catch {
                    return false;
                }
            };
        }

        #endregion

        #region From "Predicate" (Func<T, bool>)

        public static Action<T> ToAssertion<T>(this Func<T, bool> predicate) {
            return it => {
                var result = predicate(it);
                if (!result) {
                    throw new ArgumentException($"{predicate.Prettify()} returned {result}!");
                }
            };
        }

        public static Func<T, T> ToCheckpoint<T>(this Func<T, bool> predicate) {
            return it => {
                predicate.ToCheckpoint().Invoke(it);
                return it;
            };
        }

        #endregion

        #region Parameter "Reduction" (pretty sure "binding" is the correct word)

        public static Action Reduce<T>(this      Action<T>      action, T  arg)           => () => action(arg);
        public static Action Reduce<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2) => () => action(arg1, arg2);

        public static Action Reduce<T1, T2, T3>(
            this Action<T1, T2, T3> action,
            T1                      arg1,
            T2                      arg2,
            T3                      arg3
        ) => () => action(arg1, arg2, arg3);

        public static Action Reduce<T1, T2, T3, T4>(
            this Action<T1, T2, T3, T4> action,
            T1                          arg1,
            T2                          arg2,
            T3                          arg3,
            T4                          arg4
        ) => () => action(arg1, arg2, arg3, arg4);

        #endregion
    }
}