using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated.Atomic;

public interface IFileNamePart : IPathPart {
    public FileNamePart ToFileNamePart();

    public new static bool IsValid(ReadOnlySpan<char> fileNamePart) => _tryValidate(fileNamePart) == null;

    private static Exception? _tryValidate(ReadOnlySpan<char> fileNamePart) {
        var specialPathPart = PathPart.GetSpecialPathPart(fileNamePart);
        if (specialPathPart != null) {
            return Reject.Parameter(fileNamePart, $"The {nameof(SpecialPathPart)} {specialPathPart} `{specialPathPart.Value.ToPathString()}` cannot be an {nameof(IFileNamePart)}!");
        }

        return IPathPart._tryValidate(fileNamePart) ??
               BadCharException.TryAssert(fileNamePart, Clerk.InvalidFileNameChars.AsSpan());
    }

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileNamePart) {
        _tryValidate(fileNamePart)?.Assert();
        return fileNamePart;
    }

    public new static string Ratify(string fileNamePart) {
        Ratify(fileNamePart.AsSpan());
        return fileNamePart;
    }
}