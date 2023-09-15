using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Core;

/// <summary>
/// Utilities for <see cref="IEqualityComparer{T}"/>s.
/// </summary>
public static partial class Equality {
    private abstract class BaseEquality<T> : EqualityComparer<T?> {
        public sealed override bool Equals(T? x, T? y) {
            return (x, y) switch {
                (null, null)         => true,
                (not null, not null) => _Equals(x, y),
                _                    => false
            };
        }

        /// <summary>
        /// The actual implementation of <see cref="Equals"/>, without having to worry about <c>null</c> values.
        /// </summary>
        protected abstract bool _Equals([DisallowNull] T x, [DisallowNull] T y);

        /// <inheritdoc/>
        /// <remarks>This uses <see cref="EqualityComparer{T}.Default"/> to get the hash code, rather than <see cref="RuntimeHelpers"/>, because:
        /// <ul>
        /// <li><see cref="EqualityComparer{T}.Default"/> is more accurate - for example, for <see cref="string"/>s</li>
        /// <li><see cref="RuntimeHelpers"/> isn't generic, so it requires the input to be boxed</li>
        /// </ul>
        /// </remarks>
        public override int GetHashCode([DisallowNull] T obj) => Default.GetHashCode(obj);
    }

    #region Delegating equality

    private sealed class DelegatingEqualityComparer<T, TResult> : BaseEquality<T> {
        private IEqualityComparer<TResult> ResultComparer { get; }
        private Func<T, TResult>           Transformation { get; }

        public DelegatingEqualityComparer(Func<T, TResult> transformation, IEqualityComparer<TResult>? resultComparer = default) {
            Transformation = transformation;
            ResultComparer = resultComparer ?? EqualityComparer<TResult>.Default;
        }

        protected override bool _Equals(T x, T y) {
            return ResultComparer.Equals(Transformation(x), Transformation(y));
        }

        /// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// <remarks>While the documentation for <see cref="IEqualityComparer{T}.GetHashCode(T)"/> says that it should reject null <paramref name="obj"/>s, the basic <see cref="EqualityComparer{T}"/> implementation specifically returns 0 for null values.</remarks>
        public override int GetHashCode(T obj) {
            if (obj is null) {
                return 0;
            }

            var selected = Transformation(obj);
            return selected is null ? 0 : ResultComparer.GetHashCode(selected);
        }
    }

    /// <param name="transformation">converts <typeparamref name="T"/> to <typeparamref name="TResult"/></param>
    /// <param name="resultComparer">compares the computed <typeparamref name="TResult"/> values <i>(📎 defaults to <see cref="Comparer{T}.Default"/>)</i></param>
    /// <typeparam name="T">the original objects we're comparing</typeparam>
    /// <typeparam name="TResult">the computed values that we're comparing on <typeparamref name="T"/>'s behalf</typeparam>
    /// <returns>an <see cref="IEqualityComparer{T}"/> that compares the results of a <paramref name="transformation"/></returns>
    [Pure]
    public static IEqualityComparer<T> OnResultOf<T, TResult>(
        Func<T, TResult>            transformation,
        IEqualityComparer<TResult>? resultComparer = null
    ) {
        return new DelegatingEqualityComparer<T, TResult>(transformation, resultComparer);
    }

    #endregion

    #region Reference equality

    private static readonly IEqualityComparer<object?> ReferenceEqualityComparerInstance =
#if NET7_0_OR_GREATER
        ReferenceEqualityComparer.Instance;
#else
        new BiPredicateEqualityComparer<object?>(ReferenceEquals);
#endif

    /// <typeparam name="T">the type being compared</typeparam>
    /// <returns>An <see cref="IEqualityComparer{T}"/> that uses <see cref="object.ReferenceEquals"/></returns>
    /// <seealso cref="ReferenceEqualityComparer"/>
    [Pure]
    public static IEqualityComparer<T?> ByReference<T>() where T : class {
        return ReferenceEqualityComparerInstance;
    }

    #endregion

    /// <inheritdoc cref="EqualityComparer{T}.Default"/>
    [Pure]
    public static IEqualityComparer<T> ByEqualsMethod<T>() => EqualityComparer<T?>.Default;

    private sealed class ComparisonEqualityComparer<T> : BaseEquality<T> {
        public static readonly ComparisonEqualityComparer<T> Instance = new(Comparer<T>.Default);
        private readonly       IComparer<T>                  Comparer;

        public ComparisonEqualityComparer(IComparer<T>? comparer) {
            Comparer = comparer ?? Comparer<T>.Default;
        }

        protected override bool _Equals(T x, T y) {
            return Comparer.Compare(x, y) == 0;
        }
    }

    /// <returns>an <see cref="IEqualityComparer{T}"/> that returns true if <see cref="IComparer{T}.Compare"/> returns 0</returns>
    [Pure]
    public static IEqualityComparer<T> ByComparison<T>(IComparer<T>? comparer = default) {
        if (comparer is null || ReferenceEquals(comparer, Comparer<T>.Default)) {
            return ComparisonEqualityComparer<T>.Instance;
        }

        return new ComparisonEqualityComparer<T>(comparer);
    }
}