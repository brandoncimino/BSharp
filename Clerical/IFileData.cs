using FluentValidation;

using FowlFever.BSharp.Clerical;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FowlFever.Clerical;

internal enum PersistenceState {
    /// <summary>
    /// <ul>
    /// <li>The <see cref="IFileData{T}.Data"/> has been initialized (i.e. is not <c>null</c>)</li>
    /// <li>The <see cref="IFileData{T}.File"/> does not exist, or exists but is empty</li>
    /// </ul>
    /// </summary>
    NotYetSerialized,

    /// <summary>
    /// <ul>
    /// <li>The <see cref="IFileData{T}.File"/> <see cref="FileInfoExtensions.ExistsWithContent"/></li>
    /// <li>The contents of the <see cref="IFileData{T}.File"/> have not been loaded into <see cref="IFileData{T}.Data"/></li>
    /// <li>The <see cref="IFileData{T}.Data"/> is <c>null</c></li>
    /// </ul>
    /// </summary>
    NotYetDeserialized,

    /// <summary>
    /// The <see cref="IFileData{T}.File"/> exists and contains the same content as <see cref="IFileData{T}.Data"/>
    /// </summary>
    FullySynced,
}

/// <summary>
/// Combines a <see cref="FileInfo"/> with the deserialized <see cref="T"/> data that it contains.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFileData<out T> {
    /// <summary>
    /// The implementation-specific, "meaningful" data that this <see cref="IFileData{TData}"/> manages.
    /// </summary>
    /// <remarks>
    /// If <see cref="Data"/> is <c>null</c>, then the <see cref="PersistenceState"/> <b>must</b> be <see cref="Clerical.PersistenceState.NotYetDeserialized"/>.
    /// </remarks>
    public T? Data { get; }

    /// <summary>
    /// The <see cref="System.IO.FileInfo"/> that the <see cref="Data"/> will be written to / read from.
    /// </summary>
    public FileInfo File { get; }

    internal PersistenceState PersistenceState { get; }
}

#region Validators

internal class CanDeserializeValidator<T> : AbstractValidator<IFileData<T>> {
    public CanDeserializeValidator() {
        RuleFor(it => it.Data).Null();
        RuleFor(it => it.File).Must(it => it.ExistsWithContent());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.NotYetDeserialized);
    }
}

internal class CanSerializeValidator<T> : AbstractValidator<IFileData<T>> {
    public CanSerializeValidator() {
        RuleFor(it => it.Data).NotNull();
        RuleFor(it => it.File).Must(it => it.IsEmptyOrMissing());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.NotYetSerialized);
    }
}

internal class FullySyncedValidator<T> : AbstractValidator<IFileData<T>> {
    public FullySyncedValidator() {
        RuleFor(it => it.Data).NotNull();
        RuleFor(it => it.File).Must(it => it.ExistsWithContent());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.FullySynced);
    }
}

#endregion