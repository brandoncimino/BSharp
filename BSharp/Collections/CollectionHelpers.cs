using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

internal static class CollectionHelpers {
    /// <summary>
    /// Attempts to use the nice <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Linq.Enumerable.TryGetNonEnumeratedCoun">TryGetNonEnumeratedCount</a> from .NET 6+. If it isn't available, does a simple check for <see cref="ICollection{T}"/>
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="count">set to the number of items in <see cref="source"/> if we could get it without enumerating; otherwise, set to -1</param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns>true if we could get the count of items without enumerating the <paramref name="source"/></returns>
    /// <remarks>
    /// We don't really need to worry about boxing the <see cref="IEnumerable{T}"/> <paramref name="source"/> here when it's an <see cref="ImmutableArray{T}"/> because, if we had an <see cref="ImmutableArray{T}"/>, we'd already have the count.
    /// This method is only relevant when we don't know what <paramref name="source"/> is, which means it's already been boxed into an <see cref="IEnumerable{T}"/>.
    /// </remarks>
    public static bool TryGetCount<T>([NoEnumeration] this IEnumerable<T> source, out int count) {
#if NET6_0_OR_GREATER
        return System.Linq.Enumerable.TryGetNonEnumeratedCount(source, out count);
#else
        count = source switch {
            System.Collections.Generic.ICollection<T> c => c.Count,
            System.Collections.ICollection c            => c.Count,
            IReadOnlyCollection<T> c                    => c.Count,
            _                                           => -1
        };

        return count != -1;
#endif
    }
}