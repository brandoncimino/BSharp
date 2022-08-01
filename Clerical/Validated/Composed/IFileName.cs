using System.Collections.Immutable;

using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFileName : IPathPart {
    public FileName                      ToFileName();
    public FileNamePart                  BaseName   { get; }
    public ImmutableArray<FileExtension> Extensions { get; }

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileName) {
        BadCharException.Assert(fileName, Clerk.InvalidFileNameChars);
        return fileName;
    }

    public new string Ratify(string fileName) {
        Ratify(fileName.AsSpan());
        return fileName;
    }
}