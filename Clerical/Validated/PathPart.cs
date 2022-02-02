using System.Text.RegularExpressions;

using FluentValidation;
using FluentValidation.Validators;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated;

public class PathPart {
    public readonly string Value;

    public PathPart(string value) {
        var whitespacePattern = new Regex(@"\s");
        var arg               = new ArgInfo<string?>(value, nameof(Value), $"new {GetType().Name}");
        Must.NotMatch(arg, whitespacePattern);
        Must.NotMatch(arg, BPath.DirectorySeparatorPattern);

        Value = value;
    }

    public override string ToString() {
        return Value;
    }
}