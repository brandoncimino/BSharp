using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Optional {
    [PublicAPI]
    public static class NullableExtensions {
        [Pure]
        public static IEnumerable<T> AsEnumerable<T>(this T? nullableValue) where T : struct {
            return nullableValue.HasValue ? Enumerable.Repeat(nullableValue.Value, 1) : Enumerable.Empty<T>();
        }

        [Pure]
        public static Optional<T> ToOptional<T>(this T? nullableValue) where T : struct {
            return nullableValue.HasValue ? Optional.Of(nullableValue.Value) : default;
        }

        [Pure]
        public static T? ToNullable<T>(this IOptional<T> optional) where T : struct {
            return optional.OrElse(default);
        }

        [Pure]
        public static T? ToNullable<T>(this IOptional<T?> optional) where T : struct {
            return optional.OrElse(default);
        }
    }
}