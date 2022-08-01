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
public readonly record struct FileName() : IFileName {
    public static readonly FileName Empty = new();

    [MaybeNull] private readonly StrongBox<string> _value = new();
    public string Value => _value switch {
        null     => "",
        not null => _value.Value ??= $"{BaseName}{string.Join("", Extensions)}",
    };
    public bool IsEmpty => Value.IsEmpty();

    private readonly FileNamePart _baseName = new();
    public FileNamePart BaseName {
        get => _baseName;
        init {
            if (_value == null || _baseName != value) {
                _baseName = value;
                _value    = new StrongBox<string>();
            }
        }
    }
    private readonly ImmutableArray<FileExtension> _extensions = ImmutableArray<FileExtension>.Empty;
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

    internal FileName(string fileName, MustRatify mustRatify) : this(Clerk.GetBaseName(fileName), Clerk.GetExtensions(fileName)) {
        if (mustRatify == MustRatify.Yes) {
            IFileName.Ratify(fileName);
        }
    }

    public FileName(string fileName) : this(fileName, MustRatify.Yes) { }

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