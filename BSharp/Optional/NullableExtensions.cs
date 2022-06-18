using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional {
    [PublicAPI]
    public static class NullableExtensions {
        [Pure]
        public static IEnumerable<T> AsEnumerable<T>(this T? nullableValue)
            where T : struct {
            return nullableValue.HasValue ? Enumerable.Repeat(nullableValue.Value, 1) : Enumerable.Empty<T>();
        }

        [Pure]
        public static Optional<T> ToOptional<T>(this T? nullableValue) {
            return nullableValue == null ? Optional.Empty<T>() : Optional.Of(nullableValue);
        }

        [Pure]
        public static Optional<T> ToOptional<T>(this T? nullableValue)
            where T : struct {
            return nullableValue.HasValue ? Optional.Of(nullableValue.Value) : default;
        }
    }
}