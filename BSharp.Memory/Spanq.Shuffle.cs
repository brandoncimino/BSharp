using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    /// <summary>
    /// Randomizes the order of the elements in this <see cref="Span{T}"/> using a <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle">Fisher-Yates shuffle</a>.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="generator">the <see cref="Random"/> number generator used</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <remarks>
    /// This returns <see cref="System.Void"/> to line up with built-in methods like <see cref="MemoryExtensions.Sort{T}(System.Span{T})"/>.
    /// </remarks>
    public static void Shuffle<T>(this Span<T> span, Random generator) {
        for (int swaps = 0; swaps < span.Length - 1; swaps++) {
            var unswappedCount     = span.Length - swaps;
            var randomChoice       = generator.Next(unswappedCount);
            var lastUnswappedIndex = unswappedCount - 1;
            (span[randomChoice], span[lastUnswappedIndex]) = (span[lastUnswappedIndex], span[randomChoice]);
        }
    }
}