using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated.Atomic;

public interface IFileExtension : IFileNamePart {
    public FileExtension ToFileExtension();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileExtension) {
        if (fileExtension[0] == '.') {
            fileExtension = fileExtension[1..];
        }

        foreach (var c in fileExtension) {
            if (c.IsWhitespace() || Clerk.InvalidFileNamePartChars.Contains(c)) {
                throw new ArgumentException($"{nameof(IFileExtension)}s must not contain whitespace or any {nameof(Clerk.InvalidFileNamePartChars)}!", nameof(fileExtension));
            }
        }

        return fileExtension;
    }

    public new static string Ratify(string fileExtension) {
        var og       = fileExtension.AsSpan();
        var ratified = Ratify(fileExtension.AsSpan());
        if (og != ratified) {
            fileExtension = ratified.ToString();
        }

        return fileExtension;
    }
}