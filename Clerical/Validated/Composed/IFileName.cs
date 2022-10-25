using System.Collections.Immutable;

using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFileName : Atomic.IPathPart {
    public FileName                      ToFileName();
    public FileNamePart                  BaseName   { get; }
    public ImmutableArray<FileExtension> Extensions { get; }

    private const           string               DoublePeriod         = "..";
    private static readonly ImmutableArray<char> InvalidFileNameChars = Path.GetInvalidFileNameChars().ToImmutableArray();

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> fileName) {
        Atomic.IPathPart.Ratify(fileName);
        BadCharException.Assert(fileName, InvalidFileNameChars);
        return fileName;
    }

    public new static string Ratify(string fileName) {
        Ratify(fileName.AsSpan());
        return fileName;
    }
}