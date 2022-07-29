namespace FowlFever.Clerical.Validated;

public interface IFileExtension : IFileNamePart {
    public FileExtension ToFileExtension();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileExtension) {
        BadCharException.Assert(fileExtension, Clerk.InvalidFileNameChars);
        return fileExtension;
    }

    public new static string Ratify(string fileExtension) {
        Ratify(fileExtension.AsSpan());
        return fileExtension;
    }
}