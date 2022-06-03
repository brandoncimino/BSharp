using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

/// <summary>
/// Represents something that <b><i>might</i></b> have failed.
/// </summary>
[PublicAPI]
public interface IFailable {
    /// <summary>
    /// The <see cref="Exception"/> that caused this <see cref="IFailable"/> to fail (if it did).
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>Retrieving <see cref="Excuse"/> when <see cref="Failed"/> is <c>false</c> should throw an <see cref="InvalidOperationException"/>.</li>
    /// <li><see cref="Excuse"/> should <b>never</b> return <c>null</c>.</li>
    /// </ul>
    /// </remarks>
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
    public bool Failed { get; }

    IReadOnlyCollection<Type> IgnoredExceptionTypes { get; }

    Exception? IgnoredException { get; }

    //TODO: Add a Description pulled from [CallerArgumentExpression]
    // public string? Description { get; }
}

public static class FailableExtensions {
    private const string SuccessIcon = "✅";
    private const string FailIcon    = "❌";

    public static string GetIcon(this IFailable failable) => failable.Failed ? FailIcon : SuccessIcon;

    /// <summary>
    /// TODO: replace this with a default implementation once I'm using .NET 6
    /// </summary>
    /// <param name="failable"></param>
    /// <returns></returns>
    public static bool Passed(this IFailable failable) => !failable.Failed;
}