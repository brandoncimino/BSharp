using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    public T this[ElementCoord coord] => this[coord.Span][coord.Element];

    /// <summary>
    /// Represents the "coordinate" of a <see cref="RoMultiSpan{T}.GetElement(int)"/> in a <see cref="RoMultiSpan{T}"/>, consisting of a <see cref="RoMultiSpan{T}.GetSpan"/> index
    /// and an index <i>within</i> that span.
    /// </summary>
    public readonly ref struct ElementCoord {
        /// <summary>
        /// A <see cref="RoMultiSpan{T}.GetSpan"/> index.
        /// </summary>
        [NonNegativeValue]
        public int Span { get; init; }

        /// <summary>
        /// The index of an element <i>within</i> the <see cref="ReadOnlySpan{T}"/> that <see cref="Span"/> refers to. 
        /// </summary>
        [NonNegativeValue]
        public int Element { get; init; }

        public ElementCoord(int span, int element) {
            Span    = span;
            Element = element;
        }

        public void Deconstruct(out int span, out int element) {
            span    = Span;
            element = Element;
        }
    }
}