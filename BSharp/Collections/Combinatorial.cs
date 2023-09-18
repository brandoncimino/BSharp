using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// Contains similarly named factory methods for <see cref="ICombinatorial{T}"/> implementations
    /// </summary>
    [Obsolete($"Please use CartesianProduct from BSharp.Core instead")]
    public static class Combinatorial {
        public static Combinatorial<TA, TB> Of<TA, TB>(ICollection<TA> a, ICollection<TB> b) => new(a, b);

        public static Combinatorial<TA, TB, TC> Of<TA, TB, TC>(ICollection<TA> a, ICollection<TB> b, ICollection<TC> c) => new(a, b, c);

        public static Combinatorial<TA, TB, TC, TD> Of<TA, TB, TC, TD>(ICollection<TA> a, ICollection<TB> b, ICollection<TC> c, ICollection<TD> d) => new(a, b, c, d);
    }

    public interface ICombinatorial<out T>
        : IEnumerable<T>
        where T : struct, ITuple {
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public sealed record Combinatorial<TA, TB>(ICollection<TA> A, ICollection<TB> B) : ICombinatorial<(TA, TB)> {
        public IEnumerator<(TA, TB)> GetEnumerator() {
            return A.SelectMany(a => B.Select(b => (a, b))).GetEnumerator();
        }

        #region Cartesian Multiplication

        /// <summary>
        /// Builds a <see cref="Combinatorial{TA,TB,TC}"/> representing the cartesian product of this * <see cref="c"/>
        /// </summary>
        /// <param name="c"></param>
        /// <typeparam name="TC"></typeparam>
        /// <returns></returns>
        public Combinatorial<TA, TB, TC> Multiply<TC>(ICollection<TC> c) {
            return new Combinatorial<TA, TB, TC>(A, B, c);
        }

        /// <summary>
        /// Builds a <see cref="Combinatorial{TA,TB,TC,TD}"/> representing the cartesian product of this * <see cref="c"/> * <see cref="d"/>
        /// </summary>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TD"></typeparam>
        /// <returns></returns>
        public Combinatorial<TA, TB, TC, TD> Multiply<TC, TD>(ICollection<TC> c, ICollection<TD> d) {
            return new Combinatorial<TA, TB, TC, TD>(A, B, c, d);
        }

        /// <summary>
        /// Builds a <see cref="Combinatorial{TA,TB,TC,TD}"/> representing the cartesian product of this * <see cref="cd"/>.<see cref="A"/> * <see cref="cd"/>.<see cref="B"/>
        /// </summary>
        /// <param name="cd"></param>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TD"></typeparam>
        /// <returns></returns>
        public Combinatorial<TA, TB, TC, TD> Multiply<TC, TD>(Combinatorial<TC, TD> cd) {
            return new Combinatorial<TA, TB, TC, TD>(A, B, cd.A, cd.B);
        }

        #region * Operators

        /// <summary>
        /// An operator overload for <see cref="Multiply{TC}"/>.
        /// </summary>
        /// <remarks>
        /// Due to the limitations of <a href="https://stackoverflow.com/questions/14020486/operator-overloading-with-generics">generic operator overloading</a>,
        /// it is not possible to create a <c>*</c> operator method that can work with any type of <see cref="c"/> (which <see cref="Multiply{TC}"/> can handle).
        ///
        /// Instead, each type of <see cref="c"/> must have an separate method.
        ///
        /// So, I decided to include:
        /// <ul>
        /// <li>Common primitive types (<see cref="int"/>, <see cref="string"/>, etc.)</li>
        /// <li>Incest / nepotism using <see cref="TA"/> and <see cref="TB"/></li>
        /// </ul>
        /// </remarks>
        /// <example>
        /// Given:
        /// <code><![CDATA[
        /// var a = new [] { 1 };
        /// var b = new [] { 2, 2 };
        /// var c = new [] { 3, 3, 3 };
        ///
        /// var ab = Combinatorial.of(a, b);
        /// ]]></code>
        /// You could write:
        /// <code><![CDATA[
        /// ab.Multiply(c);
        /// ]]></code>
        /// Instead as:
        /// <code><![CDATA[
        /// ab * c;
        /// ]]></code>
        /// </example>
        /// <param name="ab"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Combinatorial<TA, TB, int> operator *(Combinatorial<TA, TB> ab, ICollection<int> c) {
            return ab.Multiply(c);
        }

        /// <inheritdoc cref="op_Multiply(FowlFever.BSharp.Collections.Combinatorial{TA,TB},System.Collections.Generic.ICollection{int})"/>
        public static Combinatorial<TA, TB, string> operator *(Combinatorial<TA, TB> ab, ICollection<string> c) {
            return ab.Multiply(c);
        }

        /// <inheritdoc cref="op_Multiply(FowlFever.BSharp.Collections.Combinatorial{TA,TB},System.Collections.Generic.ICollection{int})"/>
        public static Combinatorial<TA, TB, float> operator *(Combinatorial<TA, TB> ab, ICollection<float> c) {
            return ab.Multiply(c);
        }

        /// <inheritdoc cref="op_Multiply(FowlFever.BSharp.Collections.Combinatorial{TA,TB},System.Collections.Generic.ICollection{int})"/>
        public static Combinatorial<TA, TB, double> operator *(Combinatorial<TA, TB> ab, ICollection<double> c) {
            return ab.Multiply(c);
        }

        /// <inheritdoc cref="op_Multiply(FowlFever.BSharp.Collections.Combinatorial{TA,TB},System.Collections.Generic.ICollection{int})"/>
        public static Combinatorial<TA, TB, TA> operator *(Combinatorial<TA, TB> ab, ICollection<TA> c) {
            return ab.Multiply(c);
        }

        /// <inheritdoc cref="op_Multiply(FowlFever.BSharp.Collections.Combinatorial{TA,TB},System.Collections.Generic.ICollection{int})"/>
        public static Combinatorial<TA, TB, TB> operator *(Combinatorial<TA, TB> ab, ICollection<TB> c) {
            return ab.Multiply(c);
        }

        #endregion

        #endregion

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public sealed record Combinatorial<TA, TB, TC>(ICollection<TA> A, ICollection<TB> B, ICollection<TC> C) : ICombinatorial<(TA, TB, TC)> {
        public IEnumerator<(TA, TB, TC)> GetEnumerator() {
            return A.SelectMany(a => B.SelectMany(b => C.Select(c => (a, b, c)))).GetEnumerator();
        }

        #region Multiplication

        public Combinatorial<TA, TB, TC, TD> Multiply<TD>(ICollection<TD> d) {
            return new Combinatorial<TA, TB, TC, TD>(A, B, C, d);
        }

        #endregion
    }

    public sealed record Combinatorial<TA, TB, TC, TD>(ICollection<TA> A, ICollection<TB> B, ICollection<TC> C, ICollection<TD> D)
        : ICombinatorial<(TA, TB, TC, TD)> {
        public IEnumerator<(TA, TB, TC, TD)> GetEnumerator() {
            return A.SelectMany(
                        a =>
                            B.SelectMany(
                                b =>
                                    C.SelectMany(
                                        c =>
                                            D.Select(
                                                d => (a, b, c, d)
                                            )
                                    )
                            )
                    )
                    .GetEnumerator();
        }
    }
}