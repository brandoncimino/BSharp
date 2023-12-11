using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;

using JetBrains.Annotations;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Lets you fill a <see cref="Span{T}"/> in a somewhat <see cref="System.Collections.Generic.Stack{T}"/>-like manner, producing a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <remarks>
/// For whatever reason, if the <see cref="SpanBuilder{T}"/> was initialized without an explicit <see cref="Span{T}"/> allocation, you can never <see cref="Allocate"/> it:
/// <code><![CDATA[
/// //❌ Doesn't work 
/// var builder = default(SpanBuilder<char>);
/// builder.Allocate(stackalloc char[3]);
///    
/// //❌ Doesn't work
/// var builder = new SpanBuilder<char>(default);
/// builder.Allocate(stackalloc char[3]);
///    
/// //✅ Works
/// var builder = new SpanBuilder<char>(stackalloc char[0]);
/// builder.Allocate(stackalloc char[3]);
/// 
/// ]]></code>
/// </remarks>
/// <typeparam name="T">the type of the entries in the <see cref="_span"/></typeparam>
public ref struct SpanBuilder<T> {
    public readonly SpanBuilderState State => this switch {
        { _span.IsEmpty: true } => SpanBuilderState.Unallocated,
        { _isFull: true }       => SpanBuilderState.Full,
        _                       => SpanBuilderState.Fillable
    };

    private readonly bool _isFull => SpanPos >= _span.Length;

    [NonNegativeValue] internal const int DefaultBufferLength = 4;

    private                   Span<T> _span = default;
    public                    int     Capacity => _span.Length;
    [NonNegativeValue] public int     SpanPos  { get; private set; } = 0;

    /// <summary>
    /// The current contents of this <see cref="SpanBuilder{T}"/>.
    /// </summary>
    public readonly ReadOnlySpan<T> Span => _span[..SpanPos];

    public SpanBuilder(Span<T> allocation) {
        _span = allocation;
    }

    public bool TryAdd(T item) {
        if (State != SpanBuilderState.Fillable) {
            return false;
        }

        Add(item);
        return true;
    }

    public void Add(T item) {
        RequireState(SpanBuilderState.Fillable);

        _span[SpanPos] =  item;
        SpanPos        += 1;
    }

    /// <summary>
    /// Completes this <see cref="SpanBuilder{T}"/>, settings its <see cref="_span"/> to <see cref="Span{T}.Empty"/> and <see cref="SpanPos"/> to <c>0</c>.
    /// </summary>
    /// <param name="newAllocation"></param>
    /// <returns></returns>
    public ReadOnlySpan<T> Build(Span<T> newAllocation = default) {
        var old = Span;
        _span   = newAllocation;
        SpanPos = default;
        return old;
    }

    internal readonly void RequireState(SpanBuilderState requisite, [CallerMemberName] string? caller = default) {
        if (State != requisite) {
            throw new InvalidOperationException($"💣 {caller}: {nameof(SpanBuilder<T>)}.{nameof(State)} {State} != {requisite}");
        }
    }

    public void Allocate(Span<T> allocation) {
        RequireState(SpanBuilderState.Unallocated);
        _span = allocation;
    }

#if DEBUG && NET6_0_OR_GREATER
    internal IRenderable Describe() {
        var table = new Table {
            Title       = new TableTitle(nameof(SpanBuilder<T>)),
            Border      = TableBorder.Simple,
            ShowHeaders = false
        };

        table.AddColumns("", "");
        table.AddLabelled(State);
        table.AddLabelled(Capacity);
        table.AddLabelled(SpanPos);

        for (int i = 0; i < Capacity; i++) {
            var numLabel = new Paragraph()
                           .Append("[")
                           .Append(i.ToString(), new Style(Color.Orange1))
                           .Append("]")
                           .Justify(Justify.Right);
            table.AddRow(numLabel, RenderCell(i));
        }

        return table;
    }

    private IRenderable RenderCell(int index) {
        if (index >= Capacity) {
            return "not allocated".EscapeSpectre(new Style(Color.DarkSlateGray1, decoration: Decoration.Italic));
        }

        if (index < 0) {
            return "out-of-bounds".EscapeSpectre(new Style(Color.Red));
        }

        if (index >= SpanPos) {
            return "not yet filled".EscapeSpectre(new Style(Color.Green));
        }

        return (_span[index]?.OrNullPlaceholder()).EscapeSpectre(new Style(Color.Blue));

        throw Reject.Unreachable($"{nameof(index)}: {index}, {nameof(Capacity)}: {Capacity}, {nameof(SpanPos)}: {SpanPos}");
    }
#endif
}

/// <summary>
/// The state of a <see cref="SpanBuilder{T}"/>.
/// </summary>
public enum SpanBuilderState : byte {
    /// <summary>
    /// The <see cref="SpanBuilder{T}"/>'s <see cref="SpanBuilder{T}._span"/> is <see cref="Span{T}.Empty"/>.
    /// </summary>
    Unallocated = default,
    /// <summary>
    /// All of the <see cref="SpanBuilder{T}"/>'s <see cref="SpanBuilder{T}.Capacity"/> has been used.
    /// </summary>
    Full,
    /// <summary>
    /// The <see cref="SpanBuilder{T}"/> has non-0 <see cref="SpanBuilder{T}.Capacity"/> that hasn't been used.
    /// </summary>
    Fillable
}