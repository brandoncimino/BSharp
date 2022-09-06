using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    /// <inheritdoc cref="SpanEnumerator"/>
    /// <returns>a new <see cref="SpanEnumerator"/></returns>
    public SpanEnumerator GetEnumerator() => new(this);

    public RoMultiSpan<T> Where(ReadOnlySpanFunc<T, bool> predicate) => new SpanEnumerator(this, predicate).ToMultiSpan();

    /// <summary>
    /// Enumerates the individual <see cref="ReadOnlySpan{T}"/>s in a <see cref="RoMultiSpan{T}"/>.
    /// </summary>
    public ref struct SpanEnumerator {
        private readonly RoMultiSpan<T>             _source;
        private          int                        _current = -1;
        private readonly ReadOnlySpanFunc<T, bool>? _where;

        public readonly ReadOnlySpan<T> Current =>
            this switch {
                { _current: < 0 }    => throw NotStartedException(),
                { IsFinished: true } => throw AlreadyFinishedException(),
                _                    => _source[_current]
            };

        public bool IsFinished => _current >= _source.SpanCount;

        public SpanEnumerator(RoMultiSpan<T> source, [RequireStaticDelegate] ReadOnlySpanFunc<T, bool>? where = default) {
            _source = source;
            _where  = where;
        }

        public bool MoveNext() {
            _current += 1;

            if (_current >= _source.SpanCount) {
                return false;
            }

            if (_where?.Invoke(Current) ?? true) {
                return true;
            }

            return MoveNext();
        }

        public RoMultiSpan<T> ToMultiSpan() {
            if (IsFinished) {
                AlreadyFinishedException();
            }

            var spans = new RoMultiSpan<T>();

            while (MoveNext()) {
                spans = spans.Add(Current);
            }

            return spans;
        }

        #region Exceptions

        internal static InvalidOperationException NotStartedException() {
            return new InvalidOperationException($"Enumeration of this {nameof(SpanEnumerator)} has not started yet! Please call {nameof(MoveNext)}.");
        }

        internal static InvalidOperationException AlreadyFinishedException() {
            throw new InvalidOperationException($"Enumeration of this {nameof(SpanEnumerator)} is already finished!");
        }

        #endregion
    }
}