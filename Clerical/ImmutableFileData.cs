using FluentValidation;

namespace FowlFever.Clerical;

/// <summary>
/// An <see cref="ValidatedFileData{T}"/> that, once <see cref="ValidatedFileData{T}.Serialize"/>d or <see cref="ValidatedFileData{T}.Deserialize"/>d,
/// cannot be modified.
/// </summary>
/// <inheritdoc/>
public abstract class ImmutableFileData<T> : ValidatedFileData<T> {
    protected ImmutableFileData(FileInfo fileInfo) : base(fileInfo) { }
    protected ImmutableFileData(T        data, FileInfo fileInfo) : base(data, fileInfo) { }

    protected sealed override IValidator<ValidatedFileData<T>> CanSerializeValidator()   => new StrictCanSerializeValidator<T>();
    protected sealed override IValidator<ValidatedFileData<T>> CanDeserializeValidator() => new StrictCanDeserializeValidator<T>();
}