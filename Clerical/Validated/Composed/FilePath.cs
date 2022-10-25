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
public readonly record struct FilePath() : IFilePath, IHas<string> {
    public static readonly FilePath Empty = new();

    [MaybeNull] private readonly StrongBox<string> _value = new();
    public string Value => _value switch {
        null     => "",
        not null => _value.Value ??= Parts.ToPathString(Separator) + FileName,
    };

    public bool                            IsEmpty => Value.IsEmpty();
    public ImmutableArray<Atomic.PathPart> Parts   { get; } = new();

    public DirectoryPath Parent {
        get => Directory;
        init {
            if (_value == null || Directory != value) {
                Directory = value;
                _value    = new StrongBox<string>();
            }
        }
    }

    private readonly FileName _fileName = new();
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

    private readonly DirectoryPath _directory = new();
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

    public FilePath(IEnumerable<Atomic.PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) : this(parts.ToImmutableArray(), separator) { }

    public FilePath(ImmutableArray<Atomic.PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) : this() {
        Parts      = parts;
        FileName   = new FileName(Parts.Last().Value);
        _directory = new DirectoryPath(Parts.Slice(..^1));
        Separator  = separator;
    }

    public FilePath(DirectoryPath directoryPath, FileName fileName) : this() {
        Directory = directoryPath;
        FileName  = fileName;
        Parts     = directoryPath.Parts.Add(fileName.ToPathPart());
    }

    public FilePath(
        ReadOnlySpan<char> filePath,
        DirectorySeparator separator = DirectorySeparator.Universal
    ) : this(
        Clerk.SplitPath(IFilePath.Ratify(filePath))
    ) { }

    #endregion

    public PathString                ToPathString() => new(Value, MustRatify.No);
    public FileName                  ToFileName()   => FileName;
    Atomic.PathPart Atomic.IPathPart.ToPathPart()   => FileName.ToPathPart();

    public bool Equals(string?        other)                          => Value.Equals(other);
    public bool Equals(IHas<string?>? other)                          => Equals(other?.Value);
    public bool Equals<T>(T?          other) where T : IHas<string?>? => Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string ToString() => Value;
}