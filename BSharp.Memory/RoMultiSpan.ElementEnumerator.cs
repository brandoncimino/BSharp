using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    #region Element Enumerator

    /// <inheritdoc cref="ElementEnumerator"/>
    /// <returns>a new <see cref="ElementEnumerator"/></returns>
    public ElementEnumerator EnumerateElements() => new(this);

    /// <summary>
    /// "Flattens" all of the elements from each consecutive <see cref="ReadOnlySpan{T}"/> in a <see cref="RoMultiSpan{T}"/>.
    /// </summary>
    public ref struct ElementEnumerator {
        /// <summary>
        /// The original <see cref="RoMultiSpan{T}"/> that is being iterated over.
        /// </summary>
        public RoMultiSpan<T> Source => _spanerator.Source;
        private SpanEnumerator             _spanerator;
        private ReadOnlySpan<T>.Enumerator _elementEnumerator = default;
        public  T                          Current => _elementEnumerator.Current;

        public ElementEnumerator(RoMultiSpan<T> spans) {
            _spanerator = spans.GetEnumerator();
        }

        public bool MoveNext() {
            if (_elementEnumerator.MoveNext()) {
                return true;
            }

            if (_spanerator.MoveNext()) {
                _elementEnumerator = _spanerator.Current.GetEnumerator();
                return MoveNext();
            }

            return false;
        }

        public ElementEnumerator GetEnumerator() => this;
    }

    #endregion
}