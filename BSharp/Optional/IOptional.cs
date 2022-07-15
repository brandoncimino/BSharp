using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional {
    /// <summary>
    /// A mockery of Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html">Optional</a> class.
    ///
    /// TODO: Add default implementations
    /// TODO: Investigate <see cref="Microsoft.CodeAnalysis.Optional{T}"/> and see if I can achieve the functionality I want from <see cref="IOptional{T}"/> (<see cref="Optional.Select{TIn,TOut}(FowlFever.BSharp.Optional.IOptional{TIn},System.Func{TIn,TOut})"/>, for example) using that
    /// </summary>
    /// <remarks>
    /// After an amount of deliberation, on 9/2/2021 I have decided that an <see cref="IOptional{T}"/> must, by default, assume that <see cref="IHas{T}.Value"/> <see cref="CanBeNullAttribute"/>.
    /// <br/><b>UPDATE:</b> As of 7/15/2022, it seems that I am no longer respecting past me's decisions. I must revisit them.
    ///
    /// <p/>
    ///
    /// Specific implementations might be more strict; for example, I think there may be value in having <see cref="Optional{T}"/>, the primary implementation, never return null.
    ///
    /// With regards to "allowing <c>null</c> as a valid <see cref="IHas{T}.Value"/> in <see cref="IOptional{T}"/>":
    /// <ul><b>Pro</b>
    /// <li>"safest" option - accounting for null when it is impossible is OK; not accounting for null when is is possible will cause exceptions</li>
    /// <li>the delegate / functional interface options can definitely return null (<see cref="IFailable"/>, <see cref="IFailableFunc{TValue}"/>, etc.)</li>
    /// </ul>
    /// <ul><b>Con</b>
    /// <li>Ambiguous comparisons between:
    /// <ul>
    /// <li>A null <see cref="IOptional{T}"/></li>
    /// <li>An <see cref="IOptional{T}"/> <b><i>containing</i></b> a null <typeparamref name="T"/></li>
    /// <li>A null <typeparamref name="T"/></li>
    /// <li><see cref="IOptional{T}"/>s with different <typeparamref name="T"/> types that both contain null</li>
    /// </ul>
    /// </li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="T">the type of the <see cref="IHas{T}.Value"/> that <i>might</i> be there.</typeparam>
    public interface IOptional<out T> : IHas<T>, IReadOnlyCollection<T> {
        /// <summary>
        /// Whether or not a <see cref="IHas{T}.Value"/> is present.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#isPresent--">Optional.isPresent()</a></li>
        /// </ul>
        /// </remarks>
        bool HasValue { get; }
    }
}