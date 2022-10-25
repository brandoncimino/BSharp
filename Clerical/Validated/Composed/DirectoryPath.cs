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
/// Represents a group of <see cref="Atomic.PathPart"/>s.
/// </summary>
[SuppressMessage("ReSharper", "InvertIf")]
public readonly record struct DirectoryPath() : IDirectoryPath, IHasDirectoryInfo {
    public static readonly DirectoryPath Empty = new();

    [MaybeNull] private readonly StrongBox<string> _value = new();
    public string Value => _value switch {
        null     => "",
        not null => _value.Value ??= Parts.ToPathString(Separator),
    };

    #region "Fields"

    private readonly ImmutableArray<PathPart> _parts = ImmutableArray<PathPart>.Empty;
    public ImmutableArray<PathPart> Parts {
        get => _parts;
        init {
            if (_value == null || _parts.SequenceEqual(value) == false) {
                _parts = value;
                _value = new StrongBox<string>();
            }
        }
    }

    private readonly DirectorySeparator _separator = DirectorySeparator.Universal;
    public DirectorySeparator Separator {
        get => _separator;
        init {
            if (_value == null || _separator != value) {
                _separator = value;
                _value     = new StrongBox<string>();
            }
        }
    }

    #endregion

    public DirectoryInfo  Directory      => new(Value);
    public FileSystemInfo FileSystemInfo => Directory;
    public bool           IsEmpty        => Value.IsEmpty();
    public PathPart       DirectoryName  => Parts.Last();
    public DirectoryPath  Parent         => new(Parts.RemoveAt(Parts.Length));

    #region Construction

    public DirectoryPath(ImmutableArray<PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) : this() {
        Parts     = parts;
        Separator = separator;
    }

    public DirectoryPath(
        IEnumerable<PathPart> parts,
        DirectorySeparator    separator = DirectorySeparator.Universal
    ) : this(parts.ToImmutableArray(), separator) { }

    public DirectoryPath(ReadOnlySpan<char> directoryPath, DirectorySeparator separator = DirectorySeparator.Universal) : this(
        Clerk.SplitPath(directoryPath),
        separator
    ) { }

    public DirectoryPath(
        string             directoryPath,
        DirectorySeparator separator = DirectorySeparator.Universal
    ) : this(Clerk.SplitPath(directoryPath), separator) { }

    #endregion

    public bool Equals(string?        other)                          => Value.Equals(other);
    public bool Equals(IHas<string?>? other)                          => Equals(other?.Value);
    public bool Equals<T>(T?          other) where T : IHas<string?>? => Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string        ToString()        => Value;
    public          PathString    ToPathString()    => new(Value, MustRatify.No);
    public          DirectoryPath ToDirectoryPath() => this;
}