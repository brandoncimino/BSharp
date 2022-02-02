using FluentValidation;
using FluentValidation.Results;

namespace FowlFever.Clerical;

public static class ValidationResultExtensions {
    public static void OrThrow(this ValidationResult result, string? message = default) {
        if (result.IsValid == false) {
            throw message == null
                      ? new ValidationException(result.Errors)
                      : new ValidationException(message, result.Errors);
        }
    }
}