using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional {
    /// <summary>
    /// Represents something <b>with a return value</b> that <i>might</i> have failed.
    /// <p/>
    /// To represent something <b>without</b> a return value that might have failed, use <see cref="IFailable"/>.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>This interface combines <see cref="IOptional{T}"/> with <see cref="IFailable"/>.</li>
    /// <li>An <see cref="IFailableFunc{TValue}"/> should generally be used when there is a meaningful return value (i.e. a <see cref="Func{TResult}"/>).
    ///     When the failable code does not have a return value, i.e. <see cref="Action"/>, <see cref="IFailable"/> should be used instead.</li>
    /// </ul>
    /// TODO: Is there a fancy-schmancy way to enforce these kinds of contractual obligations? If we had default interfaces methods, that would be ideal, but those aren't supported by Unity yet...
    /// <p><b>Notes to Implementers</b></p>
    /// <see cref="IOptional{TValue}.Value"/> and <see cref="IFailable.Excuse"/> should be <b>mutually exclusive</b>.
    /// <ul>
    /// <li><see cref="IOptional{TValue}.HasValue"/> and <see cref="IFailable.Failed"/> should <b>never</b> be equal.</li>
    /// <li>If <see cref="IOptional{T}.HasValue"/> is true, <see cref="IFailable.Excuse"/> should throw an exception.</li>
    /// <li>If <see cref="IFailable.Failed"/> is true, <see cref="IOptional{TValue}.Value"/> should throw an exception.</li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="TValue">the <see cref="IOptional{T}.Value"/>, if this succeeded</typeparam>
    [PublicAPI]
    public interface IFailableFunc<out TValue> : IOptional<TValue>, IFailable { }
}