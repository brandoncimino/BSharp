using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    public static Span<T> Shuffle<T>(this Span<T> stuff, Random generator) {
        for (int swaps = 0; swaps < stuff.Length - 1; swaps++) {
            var unswappedCount     = stuff.Length - swaps;
            var randomChoice       = generator.Next(unswappedCount);
            var lastUnswappedIndex = unswappedCount - 1;
            (stuff[randomChoice], stuff[lastUnswappedIndex]) = (stuff[lastUnswappedIndex], stuff[randomChoice]);
        }

        return stuff;
    }
}