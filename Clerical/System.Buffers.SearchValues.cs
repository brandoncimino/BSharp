#if !NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


// ReSharper disable once CheckNamespace
namespace System.Buffers;

public static class SearchValues {
    public static SearchValues<char> Create(ReadOnlySpan<char> values) => new(values);
    public static SearchValues<byte> Create(ReadOnlySpan<byte> values) => new(values);
}

/// <summary>
/// A super-naive facsimile of <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Buffers.SearchValues-1?view=net-8.0">.NET 8.0's SearchValues</a>
/// that allows code that uses the .NET 8 class to be backwards-compatible.
/// </summary>
public sealed class SearchValues<T> {
    private readonly ImmutableArray<T> _values;
    public           bool              Contains(T value) => _values.Contains(value);
    public static implicit operator ReadOnlySpan<T>(SearchValues<T> searchValues) => searchValues._values.AsSpan();
    
    internal SearchValues(ReadOnlySpan<T> values) {
        _values = ImmutableArray.Create(values);
    }
}
#endif