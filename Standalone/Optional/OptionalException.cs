using System;

using BrandonUtils.Standalone.Strings.Prettifiers;

namespace BrandonUtils.Standalone.Optional {
    internal static class OptionalException {
        public static InvalidOperationException IsEmptyException<T>(IOptional<T> self) {
            return new InvalidOperationException($"Unable to retrieve the {nameof(self.Value)} from the {self.GetType().PrettifyType(default)} because it is empty!");
        }
    }
}