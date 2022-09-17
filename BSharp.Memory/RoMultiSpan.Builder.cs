using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    #region Builder

    public Builder ToBuilder() => new(this);

    public static Builder CreateBuilder() => new();

    /// <summary>
    /// A mutable <see cref="RoMultiSpan{T}"/> that can then be "locked in" using <see cref="Build"/>.
    /// </summary>
    public ref struct Builder {
        private ReadOnlySpan<T> _a;
        private ReadOnlySpan<T> _b;
        private ReadOnlySpan<T> _c;
        private ReadOnlySpan<T> _d;
        private ReadOnlySpan<T> _e;
        private ReadOnlySpan<T> _f;
        private ReadOnlySpan<T> _g;
        private ReadOnlySpan<T> _h;

        [ValueRange(0, RoMultiSpan.MaxSpans)] private int _count = default;

        /// <summary>
        /// The number of <see cref="ReadOnlySpan{T}"/>s that have been <see cref="Add"/>-ed to the <see cref="Builder"/>.
        /// <ul>
        /// <li>If <see cref="Count"/> is <i>increased</i>, then it will be as though <see cref="ReadOnlySpan{T}.Empty"/> spans had been <see cref="Add"/>ed.</li>
        /// <li>If <see cref="Count"/> is <i>decreased</i>, then spans will be <see cref="Remove()"/>d.</li>
        /// </ul>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">if <see cref="Count"/> is &lt; 0 or >= <see cref="RoMultiSpan.MaxSpans"/></exception>
        [ValueRange(0, RoMultiSpan.MaxSpans)]
        public int Count {
            readonly get => _count;
            set {
                if (value == _count) {
                    return;
                }

                if (value is < 0 or > RoMultiSpan.MaxSpans) {
                    throw new ArgumentOutOfRangeException(nameof(Count), value, $"must be within 0..{RoMultiSpan.MaxSpans}");
                }

                if (value < _count) {
                    for (int i = value; i < _count; i++) {
                        this[i] = default;
                    }
                }

                _count = value;
            }
        }

        [ValueRange(0, RoMultiSpan.MaxSpans)] public readonly int RemainingSpans => RoMultiSpan.MaxSpans - Count;

        public ReadOnlySpan<T> this[[ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex] {
            readonly get => GetSpan(spanIndex);
            set => SetSpan(spanIndex, value);
        }

        /// <summary>
        /// Creates a new <see cref="Builder"/> with a <see cref="Count"/> of 0.
        /// </summary>
        public Builder() {
            _a    = default;
            _b    = default;
            _c    = default;
            _d    = default;
            _e    = default;
            _f    = default;
            _g    = default;
            _h    = default;
            Count = default;
        }

        public Builder(RoMultiSpan<T> sourceSpans) {
            _a    = sourceSpans._a;
            _b    = sourceSpans._b;
            _c    = sourceSpans._c;
            _d    = sourceSpans._d;
            _e    = sourceSpans._e;
            _f    = sourceSpans._f;
            _g    = sourceSpans._g;
            _h    = sourceSpans._h;
            Count = sourceSpans.SpanCount;
        }

        private readonly ReadOnlySpan<T> GetSpan([ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex) {
            Count.RequireIndex(spanIndex);

            return spanIndex switch {
                0 => _a,
                1 => _b,
                2 => _c,
                3 => _d,
                4 => _e,
                5 => _f,
                6 => _g,
                7 => _h,
                _ => throw _Unreachable()
            };
        }

        /// <summary>
        /// Sets the value of the one of the <see cref="ReadOnlySpan{T}"/>s in this <see cref="Builder"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="spanIndex"/> must already be within the <see cref="Count"/> - in other words, <see cref="SetSpan"/> cannot <see cref="Add"/> items, only modify them.
        /// </remarks>
        /// <param name="spanIndex">the index of the <see cref="ReadOnlySpan{T}"/> being modified. Must be within <c>0..</c><see cref="Count"/></param>
        /// <param name="value">the new <see cref="ReadOnlySpan{T}"/></param>
        /// <returns>this <see cref="Builder"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="spanIndex"/> isn't within <c>0..</c><see cref="Count"/></exception>
        public Builder SetSpan([ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex, ReadOnlySpan<T> value) {
            Count.RequireIndex(spanIndex);

            switch (spanIndex) {
                case 0:
                    _a = value;
                    break;
                case 1:
                    _b = value;
                    break;
                case 2:
                    _c = value;
                    break;
                case 3:
                    _d = value;
                    break;
                case 4:
                    _e = value;
                    break;
                case 5:
                    _f = value;
                    break;
                case 6:
                    _g = value;
                    break;
                case 7:
                    _h = value;
                    break;
                default: throw _Unreachable();
            }

            return this;
        }

        #region Add

        private Builder _Add(ReadOnlySpan<T> span) {
            Count           += 1;
            this[Count - 1] =  span;
            return this;
        }

        /// <summary>
        /// Adds a new <see cref="ReadOnlySpan{T}"/> to this <see cref="Builder"/>.
        /// </summary>
        /// <param name="span">the new <see cref="ReadOnlySpan{T}"/></param>
        /// <returns>this <see cref="Builder"/></returns>
        /// <exception cref="InvalidOperationException">if the builder already has a <see cref="Count"/> of <see cref="RoMultiSpan.MaxSpans"/></exception>
        public Builder Add(ReadOnlySpan<T> span) {
            Count.RequireSpace();
            return _Add(span);
        }

        /// <summary>
        /// Adds multiple <see cref="ReadOnlySpan{T}"/>s to this <see cref="Builder"/>.
        /// </summary>
        /// <param name="spans">the <see cref="ReadOnlySpan{T}"/>s being added</param>
        /// <returns>this <see cref="Builder"/></returns>
        /// <exception cref="InvalidOperationException">if <see cref="Count"/> + <see cref="RoMultiSpan{T}.SpanCount"/> would exceed <see cref="RoMultiSpan.MaxSpans"/></exception>
        public Builder AddRange(RoMultiSpan<T> spans) {
            Count.RequireSpace(spans.Count);

            foreach (var span in spans) {
                _Add(span);
            }

            return this;
        }

        /// <summary>
        /// Similar to <see cref="SetSpan"/>, but potentially increases <see cref="Count"/> to make room for <paramref name="spanIndex"/>.
        /// </summary>
        /// <param name="spanIndex">the index of <see cref="ReadOnlySpan{T}"/> being modified</param>
        /// <param name="value">the new <see cref="ReadOnlySpan{T}"/></param>
        /// <returns>this <see cref="Builder"/></returns>
        public Builder SafeSet([ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex, ReadOnlySpan<T> value) {
            if (Count < spanIndex + 1) {
                Count = spanIndex + 1;
            }

            return SetSpan(spanIndex, value);
        }

        #endregion

        #region Remove

        /// <summary>
        /// <i>(👀 element)</i>: Move 1 position back <i>(toward 0)</i> in line.
        /// <p/>
        /// <i>(👀 index)</i>: Grab the value of the next <i>(away from 0)</i> element. If it doesn't exist, take <see cref="ReadOnlySpan{T}.Empty"/>.
        /// </summary>
        /// <param name="index">the element that was removed, causing everything <i>after</i> it to need to move</param>
        /// <returns>this <see cref="Builder"/></returns>
        public Builder RemoveAt([ValueRange(0, RoMultiSpan.MaxSpans)] int index) {
            // starting from `i`, shift everything back 1 spot
            for (int i = index; i < Count; i++) {
                var setSpan = i + 1 < Count ? this[i + 1] : default;
                this[i] = setSpan;
            }

            Count -= 1;
            return this;
        }

        public Builder RemoveAt([ValueRange(0, RoMultiSpan.MaxSpans)] int index, out ReadOnlySpan<T> removed) {
            removed = this[index];
            return RemoveAt(index);
        }

        public Builder RemoveAt(Index index)                              => RemoveAt(index.GetOffset(Count));
        public Builder RemoveAt(Index index, out ReadOnlySpan<T> removed) => RemoveAt(index.GetOffset(Count), out removed);

        /// <summary>
        /// Removes the <b>last</b> entry from this <see cref="Builder"/>.
        /// </summary>
        /// <returns>this <see cref="Builder"/></returns>
        /// <exception cref="InvalidOperationException">if the <see cref="Builder"/> is empty</exception>
        public Builder Remove() {
            if (Count <= 0) {
                throw _EmptyException(TypeName);
            }

            return RemoveAt(Count - 1);
        }

        /// <inheritdoc cref="Remove()"/>
        /// <param name="removed">set to the removed <see cref="ReadOnlySpan{T}"/></param>
        public Builder Remove(out ReadOnlySpan<T> removed) {
            removed = this[^1];
            return Remove();
        }

        #endregion

        /// <returns>a new <see cref="RoMultiSpan{T}"/> with the contents of this <see cref="Builder"/></returns>
        [Pure]
        public readonly RoMultiSpan<T> Build() => new() {
            SpanCount = Count,
            _a        = _a,
            _b        = _b,
            _c        = _c,
            _d        = _d,
            _e        = _e,
            _f        = _f,
            _g        = _g,
            _h        = _h,
        };

        private const string TypeName = $"{nameof(RoMultiSpan<T>)}.{nameof(Builder)}";

        public static implicit operator RoMultiSpan<T>(Builder builder) => builder.Build();
        public static implicit operator Builder(RoMultiSpan<T> spans)   => spans.ToBuilder();
    }

    #endregion
}