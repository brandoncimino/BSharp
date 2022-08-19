using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;

using JetBrains.Annotations;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Lets you fill a <see cref="Span{T}"/> in a manner somewhat like a <see cref="List"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanBuilder<T> {
    public enum BuilderState : byte { Unallocated = default, Full, Fillable }

    public readonly BuilderState State => this switch {
        { _span.IsEmpty: true } => BuilderState.Unallocated,
        { _isFull: true }       => BuilderState.Full,
        _                       => BuilderState.Fillable
    };

    private readonly bool _isFull => _spanPos >= _span.Length;

    [NonNegativeValue] internal const int DefaultBufferLength = 4;

    private                    Span<T> _span = default;
    public                     int     Capacity  => _span.Length;
    public                     int     UsedSoFar => _spanPos;
    [NonNegativeValue] private int     _spanPos = 0;

    /// <summary>
    /// The current contents of this <see cref="SpanBuilder{T}"/>.
    /// </summary>
    public readonly ReadOnlySpan<T> Span => _span[.._spanPos];

    internal T Peek(int index) {
        return _span[index];
    }

    public SpanBuilder(Span<T> allocation) {
        _span = allocation;
    }

    public bool TryAdd(T item) {
        if (State != BuilderState.Fillable) {
            return false;
        }

        Add(item);
        return true;
    }

    public void Add(T item) {
        RequireState(BuilderState.Fillable);

        _span[_spanPos] =  item;
        _spanPos        += 1;
    }

    public ReadOnlySpan<T> Reset(Span<T> newAllocation = default) {
        var old = Span;
        _span    = newAllocation;
        _spanPos = default;
        return old;
    }

    public readonly void RequireState(BuilderState requisite, [CallerMemberName] string? caller = default) {
        if (State != requisite) {
            throw new InvalidOperationException($"💣 {caller}: {nameof(SpanBuilder<T>)}.{nameof(State)} {State} != {requisite}");
        }
    }

    public void Allocate(Span<T> allocation) {
        RequireState(BuilderState.Unallocated);
        _span = allocation;
    }

#if DEBUG && NET6_0_OR_GREATER
    internal IRenderable Describe() {
        var table = new Table() {
            Title  = new TableTitle(nameof(SpanBuilder<T>)),
            Border = TableBorder.Simple,
        };

        table.AddColumns("", "");
        table.AddLabelled(State);
        table.AddLabelled(Capacity);
        table.AddLabelled(UsedSoFar);

        for (int i = 0; i < Capacity; i++) {
            table.AddLabelled(RenderCell(i), i);
        }

        return table;
    }

    internal IRenderable RenderCell(int index) {
        if (index >= Capacity) {
            return "not allocated".EscapeSpectre(new Style(Color.DarkSlateGray1, decoration: Decoration.Italic));
        }

        if (index < 0) {
            return "out-of-bounds".EscapeSpectre(new Style(Color.Red));
        }

        if (index < _spanPos) {
            return (_span[index]?.OrNullPlaceholder()).EscapeSpectre();
        }

        throw Reject.Unreachable();
    }
#endif
}