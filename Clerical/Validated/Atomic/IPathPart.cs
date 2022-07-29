namespace FowlFever.Clerical.Validated;

public interface IPathPart : IPathString {
    public PathPart ToPathPart();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> pathPart) {
        BadCharException.Assert(pathPart, Clerk.InvalidPathPartChars);
        return pathPart;
    }

    public new static string Ratify(string pathPart) {
        Ratify(pathPart.AsSpan());
        return pathPart;
    }
}