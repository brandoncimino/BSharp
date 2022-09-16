using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp;
using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing;

public interface IMultipleAsserter : IDisposable {
    void Invoke();

    Supplied<string?>? Heading { get; }

    /// <summary>
    /// An optional <see cref="string"/> that represents the value that was validated instead of the object's <see cref="object.ToString"/>.
    /// </summary>
    /// <remarks>
    /// Generally should be populated by <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/>.
    /// </remarks>
    string? ActualValueAlias { get; }

    /// <remarks>
    /// The <c>protected</c> modifier, in an interface, does <b>NOT</b> allow <b>implementing classes</b> to access
    /// that member - it only allows <b>child interfaces</b> to access it!
    /// </remarks>
    int Indent { get; }

    PrettificationSettings? PrettificationSettings { get; }
}

public interface IMultipleAsserter<out TSelf> : IMultipleAsserter where TSelf : IMultipleAsserter<TSelf> {
    public TSelf Self { get; }

    [MustUseReturnValue]
    public TSelf And<T>(
        T?                                           target,
        IResolveConstraint                           constraint,
        Supplied<string?>?                           description = default,
        [CallerArgumentExpression("target")] string? _target     = default
    );

    [MustUseReturnValue]
    public TSelf And(
        IMultipleAsserter subAsserter
    );
}