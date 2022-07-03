using System;
using System.Collections.Generic;

namespace FowlFever.BSharp;

/// <summary>
/// Provides equality and comparison interoperability between <typeparamref name="WRAPPER"/>s and <typeparamref name="CANON"/>s.
/// </summary>
/// <remarks>
/// This interface makes heavy use of "default implementations" via <see cref="LiaisonExtensions"/>.
/// </remarks>
/// <typeparam name="CANON">the underlying, "wrapped" type</typeparam>
/// <typeparam name="WRAPPER">the "outer" type</typeparam>
public interface ILiaison<CANON, in WRAPPER> :
    IEqualityComparer<CANON?>,
    IComparer<CANON?> {
    /// <summary>
    /// Converts a <typeparamref name="WRAPPER"/> to its <typeparamref name="CANON"/>-ical form.
    /// </summary>
    /// <param name="source">the original <typeparamref name="WRAPPER"/> instance</param>
    /// <returns>the <typeparamref name="CANON"/>-ical form of <paramref name="source"/>, if <paramref name="source"/> is non-<c>null</c>; otherwise, the <c>default</c> value of <typeparamref name="CANON"/></returns>
    CANON? Liaise(WRAPPER? source);
}

/// <summary>
/// Provides the "default implementation" for <see cref="ILiaison{CANON,WRAPPER}"/>.
/// </summary>
public static class LiaisonExtensions {
    #region Equals

    [Pure] public static bool Equals<CANON, T>(this ILiaison<CANON, T> liaison, CANON? x, CANON? y) => liaison.Equals(x,                 y);
    [Pure] public static bool Equals<CANON, T>(this ILiaison<CANON, T> liaison, CANON? x, T?     y) => liaison.Equals(x,                 liaison.Liaise(y));
    [Pure] public static bool Equals<CANON, T>(this ILiaison<CANON, T> liaison, T?     x, T?     y) => liaison.Equals(liaison.Liaise(x), liaison.Liaise(y));
    [Pure] public static bool Equals<CANON, T>(this ILiaison<CANON, T> liaison, T?     x, CANON? y) => liaison.Equals(liaison.Liaise(x), y);

    #endregion

    #region Compare

    [Pure] public static int Compare<CANON, T>(this ILiaison<CANON?, T?> liaison, CANON? x, CANON? y) => liaison.Compare(x,                 y);
    [Pure] public static int Compare<CANON, T>(this ILiaison<CANON?, T?> liaison, CANON? x, T?     y) => liaison.Compare(x,                 liaison.Liaise(y));
    [Pure] public static int Compare<CANON, T>(this ILiaison<CANON?, T?> liaison, T?     x, T?     y) => liaison.Compare(liaison.Liaise(x), liaison.Liaise(y));
    [Pure] public static int Compare<CANON, T>(this ILiaison<CANON?, T?> liaison, T?     x, CANON? y) => liaison.Compare(liaison.Liaise(x), y);

    #endregion

    #region GetHashCode

    [Pure] public static int GetHashCode<CANON, T>(this ILiaison<CANON?, T?>? liaison, CANON? obj) => liaison.GetHashCode(obj);
    [Pure] public static int GetHashCode<CANON, T>(this ILiaison<CANON?, T?>? liaison, T?     obj) => liaison.GetHashCode(liaison.Liaise(obj));

    #endregion
}

/// <summary>
/// Provides <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/> interoperability between <a href="https://en.wikipedia.org/wiki/Wrapper_library">wrapper</a>-style classes.
/// </summary>
/// <typeparam name="CANON">the underlying, "true" value that is being wrapped</typeparam>
public abstract class LiaisonComparer<CANON> :
    ILiaison<CANON, IHas<CANON?>>,
    ILiaison<CANON, Func<CANON?>>,
    ILiaison<CANON, Lazy<CANON?>> {
    public abstract bool Equals(CANON?      x, CANON? y);
    public abstract int  GetHashCode(CANON? obj);
    public abstract int  Compare(CANON?     x, CANON? y);

    public CANON? Liaise(IHas<CANON?>? source) => source == null ? default : source.Value;
    public CANON? Liaise(Func<CANON?>? source) => source == null ? default : source.Invoke();
    public CANON? Liaise(Lazy<CANON?>? source) => source == null ? default : source.Value;

    private sealed class DelegationLiaisonComparer : LiaisonComparer<CANON> {
        public IEqualityComparer<CANON?> CanonEquality { get; init; } = EqualityComparer<CANON?>.Default;
        public IComparer<CANON?>         CanonComparer { get; init; } = Comparer<CANON?>.Default;

        public override bool Equals(CANON?      x, CANON? y) => CanonEquality.Equals(x, y);
        public override int  GetHashCode(CANON? obj)         => CanonEquality.GetHashCode(obj);
        public override int  Compare(CANON?     x, CANON? y) => CanonComparer.Compare(x, y);
    }

    public static LiaisonComparer<CANON> Create(
        IEqualityComparer<CANON?>? canonEquality = default,
        IComparer<CANON?>?         canonComparer = default
    ) => new DelegationLiaisonComparer {
        CanonEquality = canonEquality ?? EqualityComparer<CANON?>.Default,
        CanonComparer = canonComparer ?? Comparer<CANON?>.Default,
    };
}