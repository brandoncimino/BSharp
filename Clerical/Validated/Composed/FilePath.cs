using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Composed;

/// <summary>
/// Combines a <see cref="DirectoryPath"/> with a <see cref="FileName"/>.
/// </summary>
public readonly record struct FilePath : IFilePath, IHas<string> {
    private readonly StrongBox<string>        _value = new();
    public           string                   Value => _value.Value ??= (Parts.JoinPath(Separator) + FileName);
    public           ImmutableArray<PathPart> Parts { get; }

    public DirectoryPath Parent {
        get => Directory;
        init => Directory = value;
    }

    public FileName FileName { get; init; }

    public DirectorySeparator Separator {
        get => Directory.Separator;
        init => Directory = Directory with { Separator = value };
    }

    public DirectoryPath Directory { get; init; }

    public FileNamePart BaseName {
        get => FileName.BaseName;
        init => FileName = FileName with {
            BaseName = value
        };
    }

    public ImmutableArray<FileExtension> Extensions {
        get => FileName.Extensions;
        init => FileName = FileName with {
            Extensions = value
        };
    }

    public FilePath(IEnumerable<PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) {
        Parts     = parts.ToImmutableArray();
        FileName  = new FileName(Parts.Last());
        Directory = new DirectoryPath(Parts.Slice(..^1));
        Separator = separator;
    }

    public FilePath(DirectoryPath directoryPath, FileName fileName) {
        Directory = directoryPath;
        FileName  = fileName;
        Parts     = directoryPath.Parts.Add(fileName.ToPathPart());
    }

    public PathString ToPathString() => new(Value, MustRatify.No);

    public FileName ToFileName() => FileName;
}