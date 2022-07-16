using System.Runtime.CompilerServices;

namespace Ratified;

public static class RatifierExtensions {
    /// <summary>
    /// Applies this <see cref="IRatifier{T}"/> to <paramref name="target"/>, returning the <paramref name="target"/> if it succeeds.
    /// </summary>
    /// <param name="ratifier">this <see cref="IRatifier{T}"/></param>
    /// <param name="target">the <typeparamref name="T"/> instance being ratified</param>
    /// <param name="targetName">see <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/></param>
    /// <param name="requiredBy">see <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the <paramref name="target"/></typeparam>
    /// <returns>the <paramref name="target"/>, if is ratified</returns>
    public static T GetRatified<T>(this IRatifier<T> ratifier, T target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default)
        where T : notnull {
        ratifier.Ratify(target, targetName, requiredBy);
        return target;
    }

    /// <summary>
    /// Creates a <see cref="RatifiedProp{T}"/> that applies this <see cref="IRatifier{T}"/> whenever it is <see cref="RatifiedProp{T}.Set"/> (or <see cref="RatifiedProp{T}.set_Value"/>).
    /// </summary>
    /// <param name="ratifier">this <see cref="IRatifier{T}"/></param>
    /// <param name="initialValue">the initial value of the <see cref="RatifiedProp{T}"/></param>
    /// <param name="ratifyInitialValue">whether or not the <paramref name="initialValue"/> should have the <paramref name="ratifier"/> applied</param>
    /// <param name="changeChecker">the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> to use when determining if the <see cref="RatifiedProp{T}.Value"/> of the <see cref="RatifiedProp{T}"/> has changed and needs to be re-ratified. Defaults to <see cref="System.Collections.Generic.EqualityComparer{T}.Default"/></param>
    /// <typeparam name="T">the type being ratified</typeparam>
    /// <returns>a new <see cref="RatifiedProp{T}"/></returns>
    public static RatifiedProp<T> CreateRatifiedProp<T>(this IRatifier<T> ratifier, T initialValue, MustRatify ratifyInitialValue = MustRatify.Yes, IEqualityComparer<T?>? changeChecker = default)
        where T : notnull => new(ratifier, initialValue, ratifyInitialValue, changeChecker);

    /// <inheritdoc cref="Ratifier{T}.Negate"/>
    public static IRatifier<T> Negate<T>(this IRatifier<T> ratifier)
        where T : notnull {
        return Ratifier<T>.Negate(ratifier);
    }
}