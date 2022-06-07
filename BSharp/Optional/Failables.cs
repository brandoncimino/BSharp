using System;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

public static partial class Failables {
    private const string SuccessIcon = "✅";
    private const string FailIcon    = "❌";

    public static string GetIcon(this IFailable failable) => failable.Failed ? FailIcon : SuccessIcon;

    /// <summary>
    /// TODO: replace this with a default implementation once I'm using .NET 6
    /// </summary>
    /// <param name="failable"></param>
    /// <returns></returns>
    public static bool Passed(this IFailable failable) => !failable.Failed;

    #region Trying

    #region Actions

    /// <summary>
    /// Attempts to execute this <see cref="Action"/>, capturing <see cref="Exception"/>s and returning a <see cref="Timeable"/>
    /// that describes what happened.
    /// </summary>
    /// <param name="actionThatMightFail"></param>
    /// <returns></returns>
    public static Timeable TryTimed([InstantHandle] this Action actionThatMightFail, [CallerArgumentExpression("actionThatMightFail")] string? expression = default) {
        return new Timeable(actionThatMightFail, expression);
    }

    #endregion

    #endregion
}