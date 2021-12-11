using System;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    [PublicAPI]
    public static class ValidationExtensions {
        [ContractAnnotation("null => stop")]
        public static T MustNotBeNull<T>(this T? obj) {
            return obj ?? throw new ArgumentNullException(nameof(obj), $"Parameter of type {typeof(T).Prettify()} must not be null!");
        }


        public static T MustBeNumeric<T>(this T? obj) {
            return obj?.IsNumber() == true ? obj : throw new ArgumentException($"Parameter of type {obj?.GetType() ?? typeof(T)} must be a numeric type!");
        }
    }
}