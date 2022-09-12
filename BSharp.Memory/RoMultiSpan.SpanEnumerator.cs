using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    #region Span Enumerator

    /// <inheritdoc cref="SpanEnumerator"/>
    /// <returns>a new <see cref="SpanEnumerator"/></returns>
    /// <remarks>
    /// The <see cref="GetEnumerator"/> method exists to enable <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement"><c>foreach</c> loops</a>.
    /// If you find the need to call it explicitly, you should probably call <see cref="EnumerateSpans"/> instead, which is more explicit to prevent confusion with <see cref="EnumerateElements"/>. 
    /// </remarks>
    public SpanEnumerator GetEnumerator() => new(this);

    /// <inheritdoc cref="SpanEnumerator"/>
    /// <returns>a new <see cref="SpanEnumerator"/></returns>
    public SpanEnumerator EnumerateSpans() => GetEnumerator();

    /// <summary>
    /// Enumerates the individual <see cref="ReadOnlySpan{T}"/>s in a <see cref="RoMultiSpan{T}"/>.
    /// </summary>
    public ref struct SpanEnumerator {
        /// <summary>
        /// The original <see cref="RoMultiSpan{T}"/> that is being iterated over.
        /// </summary>
        public readonly RoMultiSpan<T> Source;
        private          int                        _current = -1;
        private readonly ReadOnlySpanFunc<T, bool>? _where;

        public readonly ReadOnlySpan<T> Current =>
            this switch {
                { _current: < 0 }    => throw NotStartedException(),
                { IsFinished: true } => throw AlreadyFinishedException(),
                _                    => Source[_current]
            };

        public bool IsFinished => _current >= Source.SpanCount;

        public SpanEnumerator(RoMultiSpan<T> source, [RequireStaticDelegate] ReadOnlySpanFunc<T, bool>? where = default) {
            Source = source;
            _where = where;
        }

        public bool MoveNext() {
            _current += 1;

            if (_current >= Source.SpanCount) {
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

    #endregion

    #region Spanq

    /// <param name="predicate">a function that takes determines which <see cref="ReadOnlySpan{T}"/>s we want to keep</param>
    /// <returns>a new <see cref="RoMultiSpan{T}"/> containing the <see cref="ReadOnlySpan{T}"/>s for whom the given <paramref name="predicate"/> returns <c>true</c></returns>
    public RoMultiSpan<T> Where(ReadOnlySpanFunc<T, bool> predicate) => new SpanEnumerator(this, predicate).ToMultiSpan();

    /// <summary>
    /// Transforms each <see cref="ReadOnlySpan{T}"/> in this <see cref="RoMultiSpan{T}"/> into a <typeparamref name="TOut"/>, returning the results as a new <see cref="Array"/>.
    /// </summary>
    /// <param name="selector">transforms a <see cref="ReadOnlySpan{T}"/> into a <typeparamref name="TOut"/></param>
    /// <typeparam name="TOut">the output type</typeparam>
    /// <returns>a new <see cref="Array"/> of <typeparamref name="TOut"/></returns>
    public TOut[] Select<TOut>(ReadOnlySpanFunc<T, TOut> selector) {
        var results = new TOut[SpanCount];

        for (int i = 0; i < SpanCount; i++) {
            results[i] = selector(this[i]);
        }

        return results;
    }

    /// <summary>
    /// Transforms each <see cref="ReadOnlySpan{T}"/> <i>(along with its index)</i> in this <see cref="RoMultiSpan{T}"/> into a <typeparamref name="TOut"/>, returning the results as a new <see cref="Array"/>.
    /// </summary>
    /// <param name="selectorWithIndex">transforms a <see cref="ReadOnlySpan{T}"/> + its index into a <typeparamref name="TOut"/></param>
    /// <typeparam name="TOut">the output type</typeparam>
    /// <returns>a new <see cref="Array"/> of <typeparamref name="TOut"/></returns>
    public TOut[] Select<TOut>(ReadOnlySpanFunc<T, int, TOut> selectorWithIndex) {
        var results = new TOut[SpanCount];

        for (int i = 0; i < SpanCount; i++) {
            results[i] = selectorWithIndex(this[i], i);
        }

        return results;
    }

    #endregion
}