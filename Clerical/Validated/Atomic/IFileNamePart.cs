using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated.Atomic;

public interface IFileNamePart : IPathPart {
    public FileNamePart ToFileNamePart();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileNamePart) {
        var specialPathPart = PathPart.GetSpecialPathPart(fileNamePart);
        if (specialPathPart != null) {
            throw Reject.Parameter(fileNamePart, $"The {nameof(SpecialPathPart)} {specialPathPart} `{specialPathPart.Value.PathString()}` cannot be an {nameof(IFileNamePart)}!");
        }

        IPathPart.Ratify(fileNamePart);
        BadCharException.Assert(fileNamePart, Clerk.InvalidFileNamePartChars);
        return fileNamePart;
    }

    public new static string Ratify(string fileNamePart) {
        Ratify(fileNamePart.AsSpan());
        return fileNamePart;
    }
}