using FowlFever.BSharp.Attributes;

namespace FowlFever.Clerical.Fluffy;

[Experimental(Validator.ExperimentalMessage)]
public enum ValidatorStyle {
    /// <summary>
    /// A method with 1 input that returns a <see cref="bool"/>.
    /// </summary>
    Predicate,
    /// <summary>
    /// A method with 1 input and no return value that might throw an <see cref="Exception"/>.
    /// </summary>
    Assertion,
    /// <summary>
    /// A method with 1 input that returns the same object it took in.
    /// </summary>
    Checkpoint,
}