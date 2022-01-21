using System;

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

        [Pure]
        public static TResult Invoke<T1, T2, TResult>(this Func<T1, T2, TResult> func, (T1 arg1, T2 arg2) args) => func.Invoke(args.arg1, args.arg2);

        [Pure]
        public static TResult Invoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, (T1 arg1, T2 arg2, T3 arg3) args) => func.Invoke(args.arg1, args.arg2, args.arg3);

        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4);

        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5);

        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6);

        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6, args.arg7);

        #endregion

        #region Laziness

        /// <summary>
        /// Shorthand for new <see cref="Lazy{T}"/>(<paramref name="valueFactory"/>).
        /// </summary>
        /// <param name="valueFactory">a <see cref="Func{TResult}"/> that should only be executed once</param>
        /// <typeparam name="T">the output type of <paramref name="valueFactory"/></typeparam>
        /// <returns>a new <see cref="Lazy{T}"/></returns>
        public static Lazy<T> Lazily<T>(this Func<T> valueFactory) {
            return new Lazy<T>(valueFactory);
        }

        #endregion
    }
}