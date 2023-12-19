namespace FowlFever.Clerical;

internal enum ClericalValidationMessage {
    BookendWhiteSpace,
    BookendDirectorySeparator,
    EmptyPathPart,
    EmptyOrWhiteSpace,
    ContainsDirectorySeparators,
}

internal static class ClericalErrorExtensions {
    public static string GetMessage(this ClericalValidationMessage clericalValidationMessage) {
        return clericalValidationMessage switch {
            ClericalValidationMessage.BookendWhiteSpace           => "Cannot start or end with white space!",
            ClericalValidationMessage.BookendDirectorySeparator   => "Cannot start or end with a directory separator!",
            ClericalValidationMessage.EmptyPathPart               => $"Cannot contain empty {nameof(PathPart)}s!",
            ClericalValidationMessage.EmptyOrWhiteSpace           => "Cannot be empty or all-white-space!",
            ClericalValidationMessage.ContainsDirectorySeparators => "Cannot contain internal directory separators!",
            _                                                     => throw new ArgumentOutOfRangeException(nameof(clericalValidationMessage), clericalValidationMessage, null)
        };
    }
}