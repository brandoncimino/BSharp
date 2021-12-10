using System;

namespace BrandonUtils.Standalone {
    /// <summary>
    /// A wrapper for a <see cref="Func{TResult}"/> that counts the number of times it was <see cref="Func{TResult}.Invoke"/>d.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class CountedFunc<TResult> {
        private readonly Func<TResult> Func;
        public           int           InvocationCount { get; private set; }

        public CountedFunc(Func<TResult> func) {
            Func = func;
        }

        /// <summary>
        /// <see cref="Func{TResult}.Invoke"/>s the wrapped <see cref="Func"/> and iterates <see cref="InvocationCount"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="InvocationCount"/> is iterated even if <see cref="Func"/> throws an exception!
        /// </remarks>
        /// <returns></returns>
        public TResult Invoke() {
            InvocationCount += 1;
            return Func.Invoke();
        }

        public static implicit operator CountedFunc<TResult>(Func<TResult> func) {
            return new CountedFunc<TResult>(func);
        }
    }
}