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

        public static void Invoke<A, B>(this                Action<A, B>                action, (A a, B b)                          args) => action.Invoke(args.a, args.b);
        public static void Invoke<A, B, C>(this             Action<A, B, C>             action, (A a, B b, C c)                     args) => action.Invoke(args.a, args.b, args.c);
        public static void Invoke<A, B, C, D>(this          Action<A, B, C, D>          action, (A a, B b, C c, D d)                args) => action.Invoke(args.a, args.b, args.c, args.d);
        public static void Invoke<A, B, C, D, E>(this       Action<A, B, C, D, E>       action, (A a, B b, C c, D d, E e)           args) => action.Invoke(args.a, args.b, args.c, args.d, args.e);
        public static void Invoke<A, B, C, D, E, F>(this    Action<A, B, C, D, E, F>    action, (A a, B b, C c, D d, E e, F f)      args) => action.Invoke(args.a, args.b, args.c, args.d, args.e, args.f);
        public static void Invoke<A, B, C, D, E, F, G>(this Action<A, B, C, D, E, F, G> action, (A a, B b, C c, D d, E e, F f, G g) args) => action.Invoke(args.a, args.b, args.c, args.d, args.e, args.f, args.g);

        #endregion

        #region Func with Tuple args

        [Pure] public static TResult Invoke<A, B, TResult>(this                Func<A, B, TResult>                func, (A a, B b)                          args) => func.Invoke(args.a, args.b);
        [Pure] public static TResult Invoke<A, B, C, TResult>(this             Func<A, B, C, TResult>             func, (A a, B b, C c)                     args) => func.Invoke(args.a, args.b, args.c);
        [Pure] public static TResult Invoke<A, B, C, D, TResult>(this          Func<A, B, C, D, TResult>          func, (A a, B b, C c, D d)                args) => func.Invoke(args.a, args.b, args.c, args.d);
        [Pure] public static TResult Invoke<A, B, C, D, E, TResult>(this       Func<A, B, C, D, E, TResult>       func, (A a, B b, C c, D d, E e)           args) => func.Invoke(args.a, args.b, args.c, args.d, args.e);
        [Pure] public static TResult Invoke<A, B, C, D, E, F, TResult>(this    Func<A, B, C, D, E, F, TResult>    func, (A a, B b, C c, D d, E e, F f)      args) => func.Invoke(args.a, args.b, args.c, args.d, args.e, args.f);
        [Pure] public static TResult Invoke<A, B, C, D, E, F, G, TResult>(this Func<A, B, C, D, E, F, G, TResult> func, (A a, B b, C c, D d, E e, F f, G g) args) => func.Invoke(args.a, args.b, args.c, args.d, args.e, args.f, args.g);

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

        #region Actions

        [Pure] public static Action    Reduce<T>(this       Action<T>       action, T arg)    => () => action(arg);
        [Pure] public static Action    Reduce<A, B>(this    Action<A, B>    action, A a, B b) => () => action(a, b);
        [Pure] public static Action<A> Reduce<A, B>(this    Action<A, B>    action, B b)           => a => action(a,  b);
        [Pure] public static Action    Reduce<A, B, C>(this Action<A, B, C> action, A a, B b, C c) => () => action(a, b, c);
        [Pure] public static Action<A> Reduce<A, B, C>(this Action<A, B, C> action, B b, C c) => a => action(a, b, c);

        [Pure]
        public static Action Reduce<A, B, C, D>(
            this Action<A, B, C, D> action,
            A                       a,
            B                       b,
            C                       c,
            D                       d
        ) => () => action(a, b, c, d);

        [Pure] public static Action<A> Reduce<A, B, C, D>(this Action<A, B, C, D> action, B b, C c, D d) => a => action(a, b, c, d);

        #endregion

        #region Functions

        [Pure] public static Func<TOut>    Reduce<A, TOut>(this       Func<A, TOut>       func, A a)      => () => func(a);
        [Pure] public static Func<TOut>    Reduce<A, B, TOut>(this    Func<A, B, TOut>    func, A a, B b) => () => func(a, b);
        [Pure] public static Func<A, TOut> Reduce<A, B, TOut>(this    Func<A, B, TOut>    func, B b)           => a => func(a,  b);
        [Pure] public static Func<TOut>    Reduce<A, B, C, TOut>(this Func<A, B, C, TOut> func, A a, B b, C c) => () => func(a, b, c);
        [Pure] public static Func<A, TOut> Reduce<A, B, C, TOut>(this Func<A, B, C, TOut> func, B b, C c) => a => func(a, b, c);

        #endregion

        #region Discard (i.e. Func -> Action)

        [Pure] public static Action             Discard<TOut>(this             Func<TOut>             func) => () => func.Invoke();
        [Pure] public static Action<A>          Discard<A, TOut>(this          Func<A, TOut>          func) => a => func.Invoke(a);
        [Pure] public static Action<A, B>       Discard<A, B, TOut>(this       Func<A, B, TOut>       func) => (a, b) => func.Invoke(a,       b);
        [Pure] public static Action<A, B, C>    Discard<A, B, C, TOut>(this    Func<A, B, C, TOut>    func) => (a, b, c) => func.Invoke(a,    b, c);
        [Pure] public static Action<A, B, C, D> Discard<A, B, C, D, TOut>(this Func<A, B, C, D, TOut> func) => (a, b, c, d) => func.Invoke(a, b, c, d);

        #endregion

        #endregion
    }
}