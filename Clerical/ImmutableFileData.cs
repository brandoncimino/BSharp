using FluentValidation;

namespace FowlFever.Clerical;

public abstract class ImmutableFileData<T> : ValidatedFileData<T> {
    protected ImmutableFileData(FileInfo fileInfo) : base(fileInfo) { }
    protected ImmutableFileData(T        data, FileInfo fileInfo) : base(data, fileInfo) { }

    protected sealed override IValidator<ValidatedFileData<T>> CanSerializeValidator()   => new StrictCanSerializeValidator<T>();
    protected sealed override IValidator<ValidatedFileData<T>> CanDeserializeValidator() => new StrictCanDeserializeValidator<T>();
}