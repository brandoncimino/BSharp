using System.Collections.Immutable;

using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Implementors;

namespace FowlFever.Clerical.Validated.Composed;

public readonly record struct ExtensionGroup : IEquatable<IEnumerable<FileExtension>>, IHasString {
    private readonly ImmutableArray<FileExtension> _extensions;

    public ExtensionGroup(IEnumerable<FileExtension> extensions) {
        _extensions = extensions.ToImmutableArray();
    }

    public ImmutableArray<FileExtension>.Enumerator GetEnumerator() {
        return _extensions.GetEnumerator();
    }

    public int TotalLength => _extensions.Select(it => it.Value.Length).Sum();

    public bool Equals(IEnumerable<FileExtension> other)                          => _extensions.SequenceEqual(other);
    public bool Equals(string?                    other)                          => Value.Equals(other);
    public bool Equals(IHas<string?>?             other)                          => Equals(other?.Value);
    public bool Equals<T>(T?                      other) where T : IHas<string?>? => Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public          string Value      => string.Concat(_extensions);
    public override string ToString() => Value;
}