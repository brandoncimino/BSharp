using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Clerical;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Composed;

/// <summary>
/// Represents a group of <see cref="PathPart"/>s.
/// </summary>
public readonly record struct DirectoryPath : IDirectoryPath, IHasDirectoryInfo, IHas<string> {
    private readonly StrongBox<string>        _value = new();
    public           ImmutableArray<PathPart> Parts          { get; init; }
    public           FileSystemInfo           FileSystemInfo => Directory;
    public           DirectoryInfo            Directory      => new(ToString());
    public           string                   Value          => _value.Value ??= Parts.JoinPath(Separator);
    public           DirectorySeparator       Separator      { get; init; }

    public DirectoryPath(ImmutableArray<PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) {
        Parts     = parts;
        Separator = separator;
    }

    public override string        ToString()        => Value;
    public          PathString    ToPathString()    => new(Value, MustRatify.No);
    public          DirectoryPath Parent            => new(Parts.RemoveAt(Parts.Length));
    public          DirectoryPath ToDirectoryPath() => this;
    public          PathPart      DirectoryName     => Parts.Last();
}