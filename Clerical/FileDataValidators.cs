using FluentValidation;

using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical;

internal class StrictCanDeserializeValidator<T> : AbstractValidator<ValidatedFileData<T>> {
    public StrictCanDeserializeValidator() {
        RuleFor(it => it.Data).Null();
        RuleFor(it => it.File).Must(it => it.ExistsWithContent());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.NotYetDeserialized);
    }
}

internal class StrictCanSerializeValidator<T> : AbstractValidator<ValidatedFileData<T>> {
    public StrictCanSerializeValidator() {
        RuleFor(it => it.Data).NotNull();
        RuleFor(it => it.File).Must(it => it.IsEmptyOrMissing());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.NotYetSerialized);
    }
}

internal class StrictFullySyncedValidator<T> : AbstractValidator<ValidatedFileData<T>> {
    public StrictFullySyncedValidator() {
        RuleFor(it => it.Data).NotNull();
        RuleFor(it => it.File).Must(it => it.ExistsWithContent());
        RuleFor(it => it.PersistenceState).Equal(PersistenceState.FullySynced);
    }
}