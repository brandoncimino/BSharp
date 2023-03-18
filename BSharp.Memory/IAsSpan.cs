using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Indicates that this object can be safely represented as a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <inheritdoc cref="ReadOnlySpan{T}"/>
public interface IAsReadOnlySpan<T> {
    public ReadOnlySpan<T> AsReadOnlySpan();
}

/// <summary>
/// Indicates that this object can be manipulated as a <see cref="Span{T}"/>.
/// </summary>
/// <inheritdoc cref="Span{T}"/>
public interface IAsSpan<T> : IAsReadOnlySpan<T> {
    public Span<T>                     AsSpan();
    ReadOnlySpan<T> IAsReadOnlySpan<T>.AsReadOnlySpan() => AsSpan();
}