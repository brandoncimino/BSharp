using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Clerical;

/// <summary>
/// Efficiently represents a <see cref="Range"/> of the characters in a <see cref="string"/>.
/// </summary>
/// <remarks>
/// In many scenarios, it is preferable to use a <see cref="ReadOnlySpan{T}"/> instead of a a <see cref="Substring"/>.
/// </remarks>
/// <param name="Source">the original <see cref="string"/></param>
/// <param name="Start">the index in the <paramref name="Source"/> where the substring starts</param>
/// <param name="Length">the number of characters in the substring</param>
public readonly record struct Substring([field: MaybeNull] string Source, int Start, int Length) {
    public Substring(string source, int start) : this(source, start, source.Length - start) { }

    /// <summary>
    /// Retrieves a <see cref="char"/> from this substring <see cref="AsSpan"/>.
    /// </summary>
    /// <inheritdoc cref="ReadOnlySpan{T}.this"/>
    public char this[int index] => AsSpan()[index];

    /// <returns>the <see cref="System.MemoryExtensions.AsSpan(string?, int, int)"/> from the <see cref="Source"/> that represents this <see cref="Substring"/></returns>
    public ReadOnlySpan<char> AsSpan() => Source.AsSpan(Start, Length);

    public bool IsDefault => Source is null;

    public static implicit operator Substring(string             str)       => new(str, 0, str.Length);
    public static implicit operator ReadOnlySpan<char>(Substring substring) => substring.AsSpan();

    public override string ToString() {
        return AsSpan().ToString();
    }

    /// <summary>
    /// Creates a <see cref="Substring"/> from an <see cref="System.MemoryExtensions.AsSpan(string?)"/> and the <see cref="string"/> it came from.
    /// </summary>
    /// <param name="span">the <see cref="char"/>s that the <see cref="Substring"/> should contain</param>
    /// <param name="source">the <see cref="string"/> that the <paramref name="span"/> came from</param>
    /// <param name="substring">the created <see cref="Substring"/>, if we were successful</param>
    /// <returns>true if we were able to successfully create a <see cref="Substring"/></returns>
    public static bool TryCreateFromSpan(ReadOnlySpan<char> span, [NotNullWhen(true)] string? source, out Substring substring) {
        if (source.AsSpan().Overlaps(span, out var offset)) {
            substring = new Substring(source!, offset, span.Length);
            return true;
        }

        substring = default;
        return false;
    }

    /// <summary>
    /// <inheritdoc cref="TryCreateFromSpan"/>
    /// </summary>
    /// <param name="span">the <see cref="char"/>s that the <see cref="Substring"/> should contain</param>
    /// <param name="source">the <see cref="string"/> that the <paramref name="span"/> came from</param>
    /// <returns>the created <see cref="Substring"/></returns>
    /// <exception cref="ArgumentException">if the <paramref name="span"/> doesn't <see cref="System.MemoryExtensions.Overlaps{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})">overlap</see> with the <paramref name="source"/></exception>
    public static Substring CreateFromSpan(ReadOnlySpan<char> span, string source) {
        if (TryCreateFromSpan(span, source, out var result)) {
            return result;
        }

        throw new ArgumentException("The given span didn't overlap with the reference string!");
    }
}