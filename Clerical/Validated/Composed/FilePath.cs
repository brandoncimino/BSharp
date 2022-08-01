using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
[SuppressMessage("ReSharper", "InvertIf")]
public readonly record struct FilePath : IFilePath, IHas<string> {
    [MaybeNull] private readonly StrongBox<string> _value = new();
    public string Value => _value switch {
        null     => "",
        not null => _value.Value ??= Parts.JoinPathString(Separator) + FileName,
    };
    public bool                     IsEmpty => Value.IsEmpty();
    public ImmutableArray<PathPart> Parts   { get; }

    public DirectoryPath Parent {
        get => Directory;
        init {
            if (_value == null || Directory != value) {
                Directory = value;
                _value    = new StrongBox<string>();
            }
        }
    }

    private readonly FileName _fileName;
    public FileName FileName {
        get => _fileName;
        init {
            if (_value == null || _fileName != value) {
                _fileName = FileName;
                _value    = new StrongBox<string>();
            }
        }
    }

    public DirectorySeparator Separator {
        get => Directory.Separator;
        init {
            if (_value == null || Directory.Separator != value) {
                Directory = Directory with { Separator = value };
                _value    = new StrongBox<string>();
            }
        }
    }

    private readonly DirectoryPath _directory;
    public DirectoryPath Directory {
        get => _directory;
        init {
            if (_value == null || _directory != value) {
                _directory = value;
                _value     = new StrongBox<string>();
            }
        }
    }

    public FileNamePart BaseName {
        get => FileName.BaseName;
        init {
            if (_value == null || FileName.BaseName != value) {
                FileName = FileName with {
                    BaseName = value
                };
                _value = new StrongBox<string>();
            }
        }
    }

    public ImmutableArray<FileExtension> Extensions {
        get => FileName.Extensions;
        init {
            if (_value == null || FileName.Extensions != value) {
                FileName = FileName with {
                    Extensions = value
                };
            }
        }
    }

    #region Construction

    public FilePath(IEnumerable<PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) : this() {
        Parts      = parts.ToImmutableArray();
        FileName   = new FileName(Parts.Last());
        _directory = new DirectoryPath(Parts.Slice(..^1));
        Separator  = separator;
    }

    public FilePath(DirectoryPath directoryPath, FileName fileName) : this() {
        Directory = directoryPath;
        FileName  = fileName;
        Parts     = directoryPath.Parts.Add(fileName.ToPathPart());
    }

    #endregion

    public PathString ToPathString() => new(Value, MustRatify.No);

    public FileName ToFileName() => FileName;
}