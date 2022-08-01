using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.Clerical.Validated.Atomic;

using Ratified;

namespace FowlFever.Clerical.Validated.Composed;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
/// </summary>
[SuppressMessage("ReSharper", "InvertIf")]
public readonly record struct FileName : IFileName {
    [MaybeNull] private readonly StrongBox<string> _value = new();
    public string Value => _value switch {
        null     => "",
        not null => _value.Value ??= $"{BaseName}{string.Join("", Extensions)}",
    };
    public bool IsEmpty => Value.IsEmpty();

    private readonly FileNamePart _baseName;
    public FileNamePart BaseName {
        get => _baseName;
        init {
            if (_value == null || _baseName != value) {
                _baseName = value;
                _value    = new StrongBox<string>();
            }
        }
    }
    private readonly ImmutableArray<FileExtension> _extensions;
    public ImmutableArray<FileExtension> Extensions {
        get => _extensions;
        init {
            if (_value == null || _extensions.SequenceEqual(value) == false) {
                _extensions = value;
                _value      = new StrongBox<string>();
            }
        }
    }

    #region Construction

    public FileName(string     pathPart) : this(Clerk.GetBaseName(pathPart), Clerk.GetExtensions(pathPart)) { }
    public FileName(PathPart   pathPart) : this(Clerk.GetBaseName(pathPart.ToString()), Clerk.GetExtensions(pathPart.ToString())) { }
    public FileName(PathString pathString) : this(Clerk.GetBaseName(pathString.ToString()), Clerk.GetExtensions(pathString.ToString())) { }

    #endregion

    /// <summary>
    /// Represents a <see cref="FileSystemInfo"/>-safe name.
    /// </summary>
    public FileName(FileNamePart baseName, ImmutableArray<FileExtension> extensions) : this() {
        BaseName   = baseName;
        Extensions = extensions;
    }

    public FileName   ToFileName()   => this;
    public PathPart   ToPathPart()   => new(Value, MustRatify.No);
    public PathString ToPathString() => new(Value, MustRatify.No);
}