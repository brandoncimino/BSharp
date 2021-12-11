using System;

using FowlFever.BSharp.Strings.Prettifiers;

namespace FowlFever.BSharp.Optional {
    internal static class OptionalException {
        public static InvalidOperationException IsEmptyException<T>(IOptional<T> self) {
            return new InvalidOperationException($"Unable to retrieve the {nameof(self.Value)} from the {self.GetType().PrettifyType(default)} because it is empty!");
        }
    }
}