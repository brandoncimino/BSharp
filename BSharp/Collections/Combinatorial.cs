using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// Contains similarly named factory methods for <see cref="ICombinatorial{T}"/> implementations
    /// </summary>
    public static class Combinatorial {
        public static Combinatorial<TA, TB> Of<TA, TB>(
            ICollection<TA> a,
            ICollection<TB> b
        ) {
            return new Combinatorial<TA, TB>(a, b);
        }

        public static Combinatorial<TA, TB, TC> Of<TA, TB, TC>(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c
        ) {
            return new Combinatorial<TA, TB, TC>(a, b, c);
        }

        public static Combinatorial<TA, TB, TC, TD> Of<TA, TB, TC, TD>(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c,
            ICollection<TD> d
        ) {
            return new Combinatorial<TA, TB, TC, TD>(a, b, c, d);
        }

        public static Combinatorial<TA, TB, TC, TD, TE> Of<TA, TB, TC, TD, TE>(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c,
            ICollection<TD> d,
            ICollection<TE> e
        ) {
            return new Combinatorial<TA, TB, TC, TD, TE>(a, b, c, d, e);
        }

        public static Combinatorial<TA, TB, TC, TD, TE, TF> Of<TA, TB, TC, TD, TE, TF>(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c,
            ICollection<TD> d,
            ICollection<TE> e,
            ICollection<TF> f
        ) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(a, b, c, d, e, f);
        }
    }

    public interface ICombinatorial<out T> where T : struct, IStructuralEquatable, IStructuralComparable, IComparable {
        /// <summary>
        /// Generates an <see cref="IEnumerable{T}"/> containing <b>all</b> of the combinations in the <see cref="ICombinatorial{T}"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> CartesianProduct();
    }

    public class Combinatorial<TA, TB> : ICombinatorial<(TA, TB)>,
                                         IEquatable<Combinatorial<TA, TB>> {
        #region Sets

        public ICollection<TA> A;
        public ICollection<TB> B;

        #endregion

        public Combinatorial(ICollection<TA> a, ICollection<TB> b) {
            A = a;
            B = b;
        }

        public IEnumerable<(TA, TB)> CartesianProduct() {
            return A.SelectMany(a => B.Select(b => (a, b)));
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

        public Combinatorial<TA, TB, TC, TD, TE> Multiply<TC, TD, TE>(ICollection<TC> c, ICollection<TD> d, ICollection<TE> e) {
            return new Combinatorial<TA, TB, TC, TD, TE>(A, B, c, d, e);
        }

        public Combinatorial<TA, TB, TC, TD, TE> Multiply<TC, TD, TE>(Combinatorial<TC, TD, TE> cde) {
            return new Combinatorial<TA, TB, TC, TD, TE>(A, B, cde.A, cde.B, cde.C);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TC, TD, TE, TF>(ICollection<TC> c, ICollection<TD> d, ICollection<TE> e, ICollection<TF> f) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, c, d, e, f);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TC, TD, TE, TF>(Combinatorial<TC, TD, TE, TF> cdef) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, cdef.A, cdef.B, cdef.C, cdef.D);
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

        #region Equality

        public bool Equals(Combinatorial<TA, TB> other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return A.SequenceEqual(other.A) && B.SequenceEqual(other.B);
        }

        #endregion
    }

    public class Combinatorial<TA, TB, TC> : ICombinatorial<(TA, TB, TC)>, IEquatable<Combinatorial<TA, TB, TC>> {
        #region Sets

        public ICollection<TA> A;
        public ICollection<TB> B;
        public ICollection<TC> C;

        #endregion

        public Combinatorial(ICollection<TA> a, ICollection<TB> b, ICollection<TC> c) {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<(TA, TB, TC)> CartesianProduct() {
            return A.SelectMany(a => B.SelectMany(b => C.Select(c => (a, b, c))));
        }

        #region Multiplication

        public Combinatorial<TA, TB, TC, TD> Multiply<TD>(ICollection<TD> d) {
            return new Combinatorial<TA, TB, TC, TD>(A, B, C, d);
        }

        public Combinatorial<TA, TB, TC, TD, TE> Multiply<TD, TE>(ICollection<TD> d, ICollection<TE> e) {
            return new Combinatorial<TA, TB, TC, TD, TE>(A, B, C, d, e);
        }

        public Combinatorial<TA, TB, TC, TD, TE> Multiply<TD, TE>(Combinatorial<TD, TE> de) {
            return new Combinatorial<TA, TB, TC, TD, TE>(A, B, C, de.A, de.B);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TD, TE, TF>(ICollection<TD> d, ICollection<TE> e, ICollection<TF> f) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, C, d, e, f);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TD, TE, TF>(Combinatorial<TD, TE, TF> def) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, C, def.A, def.B, def.C);
        }

        #endregion

        #region Equality

        public bool Equals(Combinatorial<TA, TB, TC> other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return A.SequenceEqual(other.A) &&
                   B.SequenceEqual(other.B) &&
                   C.SequenceEqual(other.C);
        }

        #endregion
    }

    public class Combinatorial<TA, TB, TC, TD> : ICombinatorial<(TA, TB, TC, TD)>, IEquatable<Combinatorial<TA, TB, TC, TD>> {
        #region

        public ICollection<TA> A;
        public ICollection<TB> B;
        public ICollection<TC> C;
        public ICollection<TD> D;

        #endregion

        public Combinatorial(ICollection<TA> a, ICollection<TB> b, ICollection<TC> c, ICollection<TD> d) {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public IEnumerable<(TA, TB, TC, TD)> CartesianProduct() {
            return A.SelectMany(a => B.SelectMany(b => C.SelectMany(c => D.Select(d => (a, b, c, d)))));
        }

        public bool Equals(Combinatorial<TA, TB, TC, TD> other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return A.SequenceEqual(other.A) &&
                   B.SequenceEqual(other.B) &&
                   C.SequenceEqual(other.C) &&
                   D.SequenceEqual(other.D);
        }

        #region Cartesian Multiplication

        public Combinatorial<TA, TB, TC, TD, TE> Multiply<TE>(ICollection<TE> e) {
            return new Combinatorial<TA, TB, TC, TD, TE>(A, B, C, D, e);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TE, TF>(ICollection<TE> e, ICollection<TF> f) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, C, D, e, f);
        }

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TE, TF>(Combinatorial<TE, TF> ef) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, C, D, ef.A, ef.B);
        }

        #endregion
    }

    public class Combinatorial<TA, TB, TC, TD, TE> : ICombinatorial<(TA, TB, TC, TD, TE)>, IEquatable<Combinatorial<TA, TB, TC, TD, TE>> {
        #region Sets

        public ICollection<TA> A;
        public ICollection<TB> B;
        public ICollection<TC> C;
        public ICollection<TD> D;
        public ICollection<TE> E;

        #endregion

        public Combinatorial(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c,
            ICollection<TD> d,
            ICollection<TE> e
        ) {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }

        public IEnumerable<(TA, TB, TC, TD, TE)> CartesianProduct() {
            return null;
        }

        public bool Equals(Combinatorial<TA, TB, TC, TD, TE> other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return A.SequenceEqual(other.A) &&
                   B.SequenceEqual(other.B) &&
                   C.SequenceEqual(other.C) &&
                   D.SequenceEqual(other.D) &&
                   E.SequenceEqual(other.E);
        }

        #region Multiplication

        public Combinatorial<TA, TB, TC, TD, TE, TF> Multiply<TF>(ICollection<TF> f) {
            return new Combinatorial<TA, TB, TC, TD, TE, TF>(A, B, C, D, E, f);
        }

        #endregion
    }

    public class Combinatorial<TA, TB, TC, TD, TE, TF> : ICombinatorial<(TA, TB, TC, TD, TE, TF)>, IEquatable<Combinatorial<TA, TB, TC, TD, TE, TF>> {
        #region Sets

        public ICollection<TA> A;
        public ICollection<TB> B;
        public ICollection<TC> C;
        public ICollection<TD> D;
        public ICollection<TE> E;
        public ICollection<TF> F;

        #endregion

        public Combinatorial(
            ICollection<TA> a,
            ICollection<TB> b,
            ICollection<TC> c,
            ICollection<TD> d,
            ICollection<TE> e,
            ICollection<TF> f
        ) {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }

        public IEnumerable<(TA, TB, TC, TD, TE, TF)> CartesianProduct() {
            return null;
        }

        public bool Equals(Combinatorial<TA, TB, TC, TD, TE, TF> other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return A.SequenceEqual(other.A) &&
                   B.SequenceEqual(other.B) &&
                   C.SequenceEqual(other.C) &&
                   D.SequenceEqual(other.D) &&
                   E.SequenceEqual(other.E) &&
                   F.SequenceEqual(other.F);
        }
    }
}