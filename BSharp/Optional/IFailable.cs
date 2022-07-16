using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

/// <summary>
/// Represents something that <b><i>might</i></b> have failed.
/// </summary>
/// <remarks>
/// TODO: A looser, cuter version of this called <c>ITried</c>, with <c>"FailableOutcome"</c> => <c>Verdict</c>, <c>"FailableOutcome.Passed"</c> => <c>Acquitted</c>, etc.!
/// </remarks>
[PublicAPI]
public interface IFailable {
    /// <summary>
    /// The <see cref="Exception"/> that caused this <see cref="IFailable"/> to fail (if it did).
    /// </summary>
    public Exception? Excuse { get; }

    /// <summary>
    /// Whether or not this <see cref="IFailable"/> was a failure.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>If <see cref="Failed"/> is <c>true</c>, then <see cref="Excuse"/> should be present.</li>
    /// <li>If <see cref="Failed"/> is <c>false</c>, then <see cref="Excuse"/> should <b>not</b> be present.</li>
    /// </ul>
    /// </remarks>
    [MemberNotNullWhen(true, nameof(Excuse))]
    public bool Failed => Excuse != null;

    /// <summary>
    /// The inverse of <see cref="Failed"/>.
    /// </summary>
    public sealed bool Passed => !Failed;

    /// <summary>
    /// <see cref="Exception"/> types that shouldn't cause this <see cref="IFailable"/> to fail.
    /// <p/>
    /// If one of the <see cref="IgnoredExceptionTypes"/> is caught, it is put into <see cref="IgnoredException"/> rather than <see cref="Excuse"/>.
    /// </summary>
    /// <remarks>
    /// This property does not have to be implemented.
    /// However, if it is, <see cref="IgnoredException"/> should also be implemented.
    /// </remarks>
    ImmutableArray<Type> IgnoredExceptionTypes => ImmutableArray<Type>.Empty;

    /// <summary>
    /// If an <see cref="Exception"/> of one of the <see cref="IgnoredExceptionTypes"/> is caught, it will be placed here instead of <see cref="Excuse"/>.
    /// </summary>
    /// <remarks>
    /// This property does not have to be implemented.
    /// However, if it is, <see cref="IgnoredExceptionTypes"/> should also be implemented.
    /// </remarks>
    Exception? IgnoredException => null;

    /// <summary>
    /// A description of the code that this <see cref="IFailable"/> represents.
    /// </summary>
    /// <remarks>
    /// Should ideally be retrieved via <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/>.
    /// </remarks>
    public Supplied<string?> Description { get; }
}