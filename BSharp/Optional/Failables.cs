using System;

namespace FowlFever.BSharp.Optional;

public static partial class Failables {
    private const string SuccessIcon = "✅";
    private const string FailIcon    = "❌";

    public static string GetIcon(this IFailable failable) => failable.Failed ? FailIcon : SuccessIcon;

    /// <summary>
    /// Throws this <see cref="IFailable"/>'s <see cref="IFailable.Excuse"/>, if there is one
    /// </summary>
    /// <param name="failable">this <see cref="IFailable"/></param>
    /// <exception cref="Exception">this <see cref="IFailable"/>'s <see cref="IFailable.Excuse"/>, if it has one</exception>
    public static void Assert(this IFailable failable) {
        if (failable.Failed) {
            throw failable.Excuse;
        }
    }

    /// <summary>
    /// Gives access to default implementations of <see cref="IFailable"/> without needing to explicitly cast to <see cref="IFailable"/>, which is super annoying and ugly to do.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// Failable failable;
    ///
    /// failable.Failed;                // 1. doens't work because `Failed` is a default implementation inherited from `IFailable`
    /// ((IFailable)failable).Failed;   // 2. works, but is EXTREMELY hard to read and to type, and is unintuitive
    /// (failable as IFailable).Failed; // 3. not really any better than #2
    /// failable.AsFailable().Failed;   // 4. works, and is _slightly_ better, because at least it doesn't require you to go "backwards" to the beginning of the line to add parentheses everywhere
    /// ]]></code>
    /// </example>
    /// <param name="failable">this <see cref="IFailable"/></param>
    /// <returns>this <see cref="IFailable"/> <i>(for reals)</i></returns>
    public static IFailable AsFailable(this IFailable failable) => failable;

    /// <summary>
    /// "Converts" an <see cref="IFailable"/> into an <see cref="IFailableFunc{TValue}"/> by providing it with an output <see cref="IHas{T}.Value"/>.
    /// </summary>
    /// <param name="failableAction"></param>
    /// <param name="outputValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IFailableFunc<T> WithOutput<T>(this IFailable failableAction, Optional<T> outputValue) {
        return new FailableFunc<T>(failableAction, outputValue);
    }
}