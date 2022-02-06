using FluentValidation;
using FluentValidation.Results;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

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
/// An implementation of <see cref="IFileData{T}"/> that uses <see cref="IValidator{T}"/>s to determine <see cref="CanSerialize"/>, <see cref="CanDeserialize"/>, etc.
/// </summary>
/// <inheritdoc/>
[PublicAPI]
public abstract class ValidatedFileData<T> : IFileData<T> {
    public   T?               Data             { get; protected set; }
    public   FileInfo         File             { get; }
    internal PersistenceState PersistenceState { get; set; }

    /// <summary>
    /// Initializes a <see cref="ValidatedFileData{T}"/> for a pre-existing, non-empty <see cref="File"/>.
    /// </summary>
    /// <param name="fileInfo">the <see cref="FileInfoExtensions.ExistsWithContent"/> containing the <see cref="Data"/></param>
    protected ValidatedFileData(FileInfo fileInfo) {
        var methodName = $"new {GetType().Name} with a pre-existing file";
        Must.BeNull(Data, nameof(Data), methodName);
        Must.ExistWithContent(fileInfo, nameof(fileInfo), methodName);
        File             = fileInfo;
        PersistenceState = PersistenceState.NotYetDeserialized;
    }

    /// <summary>
    /// Initializes a <see cref="ValidatedFileData{T}"/> for an initialized <see cref="Data"/> object that has <b>not been written to a <see cref="File"/> yet</b>!
    /// </summary>
    /// <param name="data">the <typeparamref name="T"/> object that will be written to a file</param>
    /// <param name="fileInfo">the <b><see cref="FileInfoExtensions.IsEmptyOrMissing"/></b> that the <see cref="Data"/> will be written to</param>
    protected ValidatedFileData(T data, FileInfo fileInfo) {
        var methodName = $"new {GetType().Name} without an existing file";
        Must.NotBeNull(data, nameof(data), methodName);
        Must.BeEmptyOrMissing(fileInfo, nameof(fileInfo), methodName);
        Data             = data;
        File             = fileInfo;
        PersistenceState = PersistenceState.NotYetSerialized;
    }

    #region CanSerialize

    public bool CanSerialize   => ValidateCanSerialize(out _);

    public bool ValidateCanSerialize(out ValidationResult validationResult) {
        validationResult = CanSerializeValidator().Validate(this);
        return validationResult.IsValid;
    }

    #endregion

    #region CanDeserialize

    public bool CanDeserialize => _CanDeserialize(out _);

    private bool _CanDeserialize(out ValidationResult validationResult) {
        validationResult = CanDeserializeValidator().Validate(this);
        return validationResult.IsValid;
    }

    #endregion

    protected virtual ValidationResult NSync() {
        return new StrictFullySyncedValidator<T>().Validate(this);
    }

    public void Sync() {
        switch (PersistenceState) {
            case PersistenceState.NotYetDeserialized:
                Deserialize();
                break;
            case PersistenceState.NotYetSerialized:
                Serialize();
                break;
            case PersistenceState.FullySynced:
                break;
            default:
                throw BEnum.UnhandledSwitch(PersistenceState, nameof(PersistenceState), nameof(Sync));
        }

        NSync().OrThrow($"{GetType().Name} was in an invalid state for the {nameof(PersistenceState)} {PersistenceState} when {nameof(Sync)} was called!");
    }

    public void Serialize() {
        //pre-condition
        ValidateCanSerialize(out var result);
        result.OrThrow($"Could not serialize {GetType().Name}!");

        //action
        ExecuteSerialization();

        //post-condition
        NSync().OrThrow($"{GetType().Name} was not in a proper state after {nameof(ExecuteSerialization)}!");
        PersistenceState = PersistenceState.FullySynced;
    }

    public void Deserialize() {
        //pre-condition
        _CanDeserialize(out var result);
        result.OrThrow($"Could not deserialize {GetType().Name}!");

        //action
        ExecuteDeserialization();

        //post-check
        NSync().OrThrow($"{GetType().Name} was not in a proper state after {nameof(ExecuteDeserialization)}!");
        PersistenceState = PersistenceState.FullySynced;
    }

    #region Abstract

    /// <summary>
    /// The <see cref="IValidator{T}"/> used to determine <see cref="ValidateCanSerialize"/>
    /// </summary>
    /// <returns></returns>
    protected abstract IValidator<ValidatedFileData<T>> CanSerializeValidator();

    /// <summary>
    /// The <see cref="IValidator{T}"/> used to determine <see cref="_CanDeserialize"/>
    /// </summary>
    /// <returns></returns>
    protected virtual IValidator<ValidatedFileData<T>> CanDeserializeValidator() => new StrictCanDeserializeValidator<T>();

    /// <summary>
    /// The method that turns <see cref="Data"/> → <see cref="File"/> when <see cref="Serialize"/> is called.
    /// </summary>
    protected abstract void ExecuteSerialization();

    /// <summary>
    /// The method that turns <see cref="File"/> → <see cref="Data"/> when <see cref="Deserialize"/> is called.
    /// </summary>
    protected abstract void ExecuteDeserialization();

    #endregion
}