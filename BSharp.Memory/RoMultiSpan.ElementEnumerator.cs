using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    public ref struct ElementEnumerator {
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
    }
}