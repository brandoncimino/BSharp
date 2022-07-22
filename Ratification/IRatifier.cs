using System.Buffers;
using System.Runtime.CompilerServices;

using FowlFever.BSharp;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Sugar;
using FowlFever.Implementors;

namespace Ratified;

/// <summary>
/// Another attempt at making a nice class for assertions.
/// </summary>
/// <remarks>
/// <ul>
/// <li>Uses default implementations to work with <see cref="IHas{T}"/>, <see cref="System.Lazy{T}"/>, etc.</li>
/// <li>The <see cref="Ratify(T?,string?,string?)"/> methods throw <see cref="FowlFever.BSharp.Exceptions.RejectionException"/>s on failure</li>
/// <li>The <see cref="TryRatify(T?,string?,string?)"/> methods return <see cref="FowlFever.BSharp.Optional.IFailable"/>s containing the results</li>
/// </ul>
/// 📎 There is no "pass-through" / "checkpoint" method here because having one would prevent the use of <a href="https://docs.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance#generic-interfaces-with-covariant-type-parameters">contravariance</a> in the <typeparamref name="T"/> parameter.
/// Instead, you can call the extension methods <see cref="RatifierExtensions.GetRatified{T}"/> or <see cref="RatifierExtensions.TryGetRatified{T}"/>, which are not subject to the co- and contra-variance restrictions.
/// </remarks>
/// <typeparam name="T">the type being ratified</typeparam>
public interface IRatifier<in T>
    where T : notnull {
    public string? Description { get; }

    protected internal IFailable _TryRatify(T? target, string? targetName, string? requiredBy);

    /// <summary>
    /// Applies this <see cref="IRatifier{T}"/> to the <paramref name="target"/>, returning an <see cref="IFailable"/> with the results.
    /// </summary>
    /// <param name="target">the <typeparamref name="T"/> instance being ratified</param>
    /// <param name="targetName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="requiredBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <returns>an <see cref="IFailable"/> that either <see cref="IFailable.Passed"/> or <see cref="IFailable.Failed"/> based on this <see cref="IRatifier{T}"/></returns>
    public sealed IFailable TryRatify(T? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target, targetName, requiredBy);

    /// <inheritdoc cref="TryRatify(T?,string?,string?)"/>
    public sealed IFailable TryRatify(IHas<T?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target.OrDefault(), targetName, requiredBy);

    /// <inheritdoc cref="TryRatify(T?,string?,string?)"/>
    public sealed IFailable TryRatify<T2>(Lazy<T2?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default)
        where T2 : T => _TryRatify(target.OrDefault(), targetName, requiredBy);

    /// <summary>
    /// Applies this <see cref="IRatifier{T}"/> to the <paramref name="target"/>, throwing an exception if it fails.
    /// </summary>
    /// <param name="target">the <typeparamref name="T"/> instance being ratified</param>
    /// <param name="targetName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="requiredBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <remarks>
    /// The type of exception thrown by <see cref="Ratify(T?,string?,string?)"/> methods is up to the individual implementation.
    /// </remarks>
    public sealed void Ratify(T? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target, targetName, requiredBy).Assert();

    /// <inheritdoc cref="Ratify(T?,string?,string?)"/>
    public sealed void Ratify(IHas<T?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target.OrDefault(), targetName, requiredBy).Assert();

    /// <inheritdoc cref="Ratify(T?,string?,string?)"/>
    public sealed void Ratify<T2>(Lazy<T2?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default)
        where T2 : T => _TryRatify(target.OrDefault(), targetName, requiredBy).Assert();
}

/// <summary>
/// An <see cref="IRatifier{T}"/> that takes in <see cref="TArg"/> arguments.
/// </summary>
/// <typeparam name="T">the type being ratified</typeparam>
/// <typeparam name="TArg">the type of any additional arguments to the <see cref="Predicate"/></typeparam>
public interface IRatifier<T, TArg> : IRatifier<(T target, TArg args)>
    where T : notnull {
    /// <summary>
    /// Returns <c>true</c> if a given <typeparamref name="T"/> instance is valid.
    /// </summary>
    Func<T, TArg, bool> Predicate { get; }

    /// <param name="arguments">the concrete <typeparamref name="TArg"/> values</param>
    /// <returns>a new <see cref="IRatifier{T}"/> equivalent to this <see cref="IRatifier{T, TArg}"/> but with <typeparamref name="TArg"/> set to the given <paramref name="arguments"/></returns>
    /// <remarks>
    /// TODO: See if this is really meaningful, or if it just undoes the benefits of trying to keep <see cref="IRatifier{T}"/>s static because it captures a value for <paramref name="arguments"/>...
    /// </remarks>
    IRatifier<T> Bind(TArg arguments) {
        return Ratifier<T>.Create(Predicate, arguments, static (delgato, target, args) => target != null && delgato(target, args), Description);
    }
}

/// <summary>
/// Similar to <see cref="ReadOnlySpanAction{T,TArg}"/>, but returns a <see cref="bool"/>.
/// </summary>
/// <param name="span">the <see cref="ReadOnlySpan{T}"/> being tested</param>
/// <param name="arg">additional arguments to the predicate</param>
public delegate bool ReadOnlySpanPredicate<T, in TArg>(ReadOnlySpan<T> span, TArg arg);

/// <summary>
/// A specialized <see cref="IRatifier{T}"/> that ratifies a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <typeparam name="T">the type of elements in the <see cref="ReadOnlySpan{T}"/></typeparam>
public interface ISpanRatifier<T> : IRatifier<T[]>
    where T : notnull {
    /// <summary>
    /// A special predicate that executes against <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// The <c>arg</c> parameter is an empty <see cref="ValueTuple"/> because it is essentially "discarded".
    /// </remarks>
    ReadOnlySpanPredicate<T?, ValueTuple> Predicate { get; }
}

/// <summary>
/// A specialized <see cref="IRatifier{T,TArg}"/> that ratifies a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <typeparam name="T">the type of elements in the <see cref="ReadOnlySpan{T}"/></typeparam>
/// <typeparam name="TArg">the type any additional arguments to the <see cref="Predicate"/></typeparam>
public interface ISpanRatifier<T, TArg> : IRatifier<(T[], TArg)>
    where T : notnull {
    ReadOnlySpanPredicate<T?, TArg> Predicate { get; }
}