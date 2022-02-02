using FluentValidation;
using FluentValidation.Validators;

using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical.Validated;

public class PathPart {
    private class PathPartValidator : AbstractValidator<PathPart> {
        public PathPartValidator() {
            RuleFor(it => it.Value)
                .NotEmpty()
                .Matches(BPath.DirectorySeparatorPattern);
        }
    }

    private static readonly Lazy<PathPartValidator> Validator = new ();

    public readonly string Value;

    public PathPart(string value) {
        Validator.Value.ValidateAndThrow(this);
        Value = value;
    }

    public override string ToString() {
        return Value;
    }
}