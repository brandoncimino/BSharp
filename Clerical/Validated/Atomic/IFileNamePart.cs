namespace FowlFever.Clerical.Validated.Atomic;

public interface IFileNamePart : IPathPart {
    public FileNamePart ToFileNamePart();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileNamePart) {
        BadCharException.Assert(fileNamePart, Clerk.InvalidFileNamePartChars);
        return fileNamePart;
    }

    public new static string Ratify(string fileNamePart) {
        Ratify((ReadOnlySpan<char>)fileNamePart);
        return fileNamePart;
    }
}