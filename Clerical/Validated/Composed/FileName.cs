using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Implementors;

using JetBrains.Annotations;

using Ratified;

namespace FowlFever.Clerical.Validated.Composed;

public readonly record struct ArgLazy<T, ARG> {
    private                      Func<ARG, T> ValueFactory { get; }
    [MaybeNull] private readonly StrongBox<T> _box;
    [MemberNotNullWhen(false, nameof(_box))]
    public bool IsDefault => _box == null;

    public ArgLazy([RequireStaticDelegate] Func<ARG, T> valueFactory) {
        ValueFactory = valueFactory;
        _box         = new StrongBox<T>();
    }

    public T Get(ARG arg) {
        if (IsDefault) {
            throw new InvalidOperationException($"This {nameof(ArgLazy<T, ARG>)} is default! Please make sure to initialize it explicitly!");
        }

        lock (_box) {
            return _box.Value ??= ValueFactory(arg);
        }
    }
}

public interface IHasLazyProp<out T> {
    public T ComputeValue();
}

public readonly record struct LazyProp<TProp, TOwner>(TOwner Owner)
    where TOwner : IHasLazyProp<TProp> {
    private readonly ArgLazy<TProp, TOwner> _lazy = new(static owner => owner.ComputeValue());
    public           TProp                  Value => _lazy.Get(Owner);
}

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
/// </summary>
public readonly record struct FileName : IHas<string>, IFileName, IHasLazyProp<string> {
    private readonly StrongBox<string>             _value = new();
    public           string                        Value      => _value.Value ??= $"{BaseName}{string.Join("", Extensions)}";
    public           FileNamePart                  BaseName   { get; init; }
    public           ImmutableArray<FileExtension> Extensions { get; init; }

    /// <summary>
    /// Represents a <see cref="FileSystemInfo"/>-safe name.
    /// </summary>
    public FileName(FileNamePart baseName, ImmutableArray<FileExtension> extensions) : this() {
        BaseName   = baseName;
        Extensions = extensions;
    }

    public FileName(PathPart pathPart) {
        BaseName   = Clerk.GetBaseName(pathPart.ToString());
        Extensions = Clerk.GetExtensions(pathPart.ToString());
    }

    public FileName(string pathPart) : this(Clerk.GetBaseName(pathPart), Clerk.GetExtensions(pathPart)) { }

    public FileName ToFileName() => this;
    public PathPart ToPathPart() => new(Value, MustRatify.No);

    public string ComputeValue() {
        return $"{BaseName}{string.Join("", Extensions)}";
    }

    public void Deconstruct(out FileNamePart baseName, out ImmutableArray<FileExtension> extensions) {
        baseName   = BaseName;
        extensions = Extensions;
    }
}