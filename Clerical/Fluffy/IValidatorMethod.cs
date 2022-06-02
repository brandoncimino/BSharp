namespace FowlFever.Clerical.Fluffy;

public interface IValidatorMethod {
    public ValidatorStyle Style { get; }
    public void           Assert(object? value);
    public bool           Try(object?    value);
    public object?        Check(object?  value);
}