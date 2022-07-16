namespace Ratified;

/// <summary>
/// Provides static methods to construct <see cref="IRatifier{T}"/>s.
/// </summary>
/// <typeparam name="T">the type being ratified</typeparam>
/// <todo>
/// Re-evaluate whether the static class needs to be generic or not. I think the benefit was that it would allow me to specify <typeparamref name="T"/> explicitly
/// while having other generic type parameters (in stuff like <see cref="DelegateRatifier{TDel,TArg}"/>) be implied.
/// <ul>
/// <li>Do other aspects of <see cref="Ratifier{T}"/> need this?</li>
/// <li>Is there a benefit in keeping everything together in <see cref="Ratifier{T}"/>, even if it doesn't <b><i>need</i></b> ot be accessed through a generic type?</li>
/// </ul>
/// </todo>
public static partial class Ratifier<T>
    where T : notnull { }