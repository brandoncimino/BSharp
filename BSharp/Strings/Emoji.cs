using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Some simple methods to refer to different emoji.
///
/// TODO: there's GOT to be a library for this stuff...
/// </summary>
public static class Emoji {
    private static readonly string[] NumberEmoji = {
        "0️⃣",
        "1️⃣",
        "2️⃣",
        "3️⃣",
        "4️⃣",
        "5️⃣",
        "6️⃣",
        "7️⃣",
        "8️⃣",
        "9️⃣",
    };

    /// <param name="digit">an <see cref="int"/> from 0-9</param>
    /// <returns>the corresponding emoji number from 0️⃣ - 9️⃣</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string Number([ValueRange(0, 9)] int digit) => digit switch {
        >= 0 and < 10 => NumberEmoji[digit],
        _             => throw new ArgumentOutOfRangeException(nameof(digit), digit, "must be a single digit (0-9)"),
    };
}