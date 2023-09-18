namespace FowlFever.Clerical;

public interface IValidatable<TSelf> where TSelf : IValidatable<TSelf> {
#if NET7_0_OR_GREATER
    public static abstract void Validate(TSelf toBeValidated);
#endif

    public void Validate();
}