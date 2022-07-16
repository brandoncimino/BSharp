using System.Runtime.CompilerServices;

using FowlFever.BSharp;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Sugar;

namespace Ratified;

/// <summary>
/// Another attempt at making a nice class for assertions.
/// </summary>
/// <remarks>
/// <ul>
/// <li>Uses default implementations to work with <see cref="FowlFever.BSharp.IHas{T}"/>, <see cref="System.Lazy{T}"/>, etc.</li>
/// <li>The <see cref="Ratify(T?,string?,string?)"/> methods throw <see cref="FowlFever.BSharp.Exceptions.RejectionException"/>s on failure</li>
/// <li>The <see cref="TryRatify(T?,string?,string?)"/> methods return <see cref="FowlFever.BSharp.Optional.IFailable"/>s containing the results</li>
/// </ul>
/// </remarks>
/// <typeparam name="T">the type being ratified</typeparam>
public interface IRatifier<in T>
    where T : notnull {
    public string? Description { get; }

    protected internal IFailable _TryRatify(T? target, string? targetName, string? requiredBy);

    public sealed IFailable TryRatify(T?        target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target,             targetName, requiredBy);
    public sealed IFailable TryRatify(IHas<T?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target.OrDefault(), targetName, requiredBy);

    public sealed IFailable TryRatify<T2>(Lazy<T2?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default)
        where T2 : T => _TryRatify(target.OrDefault(), targetName, requiredBy);

    public sealed void Ratify(T?        target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target,             targetName, requiredBy).Assert();
    public sealed void Ratify(IHas<T?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default) => _TryRatify(target.OrDefault(), targetName, requiredBy).Assert();

    public sealed void Ratify<T2>(Lazy<T2?>? target, [CallerArgumentExpression("target")] string? targetName = default, [CallerMemberName] string? requiredBy = default)
        where T2 : T => _TryRatify(target.OrDefault(), targetName, requiredBy).Assert();
}

/// <summary>
/// An <see cref="IRatifier{T}"/> that takes in <see cref="TArg"/> arguments.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TArg"></typeparam>
public interface IRatifier<T, TArg> : IRatifier<(T target, TArg args)>
    where T : notnull {
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