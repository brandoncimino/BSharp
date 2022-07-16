using System;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

/// <summary>
/// Joins together a <see cref="Delegate"/> and the <see cref="CallerArgumentExpressionAttribute"/> used to create it.
/// </summary>
/// <param name="Lambda">the <typeparamref name="TDelegate"/> instance</param>
/// <param name="Expression">see <see cref="CallerArgumentExpressionAttribute"/></param>
/// <typeparam name="TDelegate">the type of <see cref="Delegate"/></typeparam>
[UsedImplicitly]
public record LambdaInfo<TDelegate>(TDelegate Lambda, [CallerArgumentExpression("Lambda")] string? Expression = default)
    where TDelegate : Delegate {
    /// <inheritdoc cref="LambdaInfo{TDelegate}"/>
    public static LambdaInfo<TDelegate> Of(TDelegate lambda, [CallerArgumentExpression("lambda")] string? expression = default) => new(lambda, expression);
}