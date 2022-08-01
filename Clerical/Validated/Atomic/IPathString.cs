namespace FowlFever.Clerical.Validated.Atomic;

public interface IPathString {
    public PathString ToPathString();

    public static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> pathString) {
        BadCharException.Assert(pathString, Clerk.InvalidPathChars);
        return pathString;
    }

    public static string Ratify(string pathString) {
        Ratify(pathString.AsSpan());
        return pathString;
    }
}