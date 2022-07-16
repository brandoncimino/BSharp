using System.Runtime.CompilerServices;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace Ratified;

public static partial class Ratifier<T> {
    private record FailableRatifier([RequireStaticDelegate] Func<T?, IFailable> FailableFactory, string? Description) : IRatifier<T> {
        public string Description { get; } = Description ?? "";

        IFailable IRatifier<T>._TryRatify(T? target, string? targetName, string? requiredBy) {
            var failable = FailableFactory.Invoke(target);
            if (Description.IsBlank()) {
                return failable;
            }

            var newDescription = string.Join(" // ", Description, failable.Description);
            return new Failable(failable, newDescription);
        }
    }

    public static IRatifier<T> Create(
        [RequireStaticDelegate]
        Func<T?, IFailable> failableFactory,
        [CallerArgumentExpression("failableFactory")]
        string? description = default
    ) {
        return new FailableRatifier(failableFactory, description);
    }
}