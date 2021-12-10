using System.Collections.Generic;

namespace BrandonUtils.Standalone.Collections {
    /// <summary>
    /// A class created exclusively to hold the <see cref="Of{T}"/> method, which mimics Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/List.html#of-E...-">List.of()</a>.
    /// </summary>
    /// <remarks>
    /// If this causes import conflicts and stuff when typing, the class can be renamed to <c>"Lists"</c>.
    /// </remarks>
    public static class List {
        /// <summary>
        /// Mimics Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/List.html#of-E...-">List.of()</a>, because I keep typing that constantly.
        /// </summary>
        public static List<T> Of<T>(params T[] entries) {
            return new List<T>(entries);
        }
    }
}