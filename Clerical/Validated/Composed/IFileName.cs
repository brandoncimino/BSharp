using System.Collections.Immutable;

using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFileName {
    public FileName                      ToFileName();
    public FileNamePart                  BaseName   { get; }
    public ImmutableArray<FileExtension> Extensions { get; }
}