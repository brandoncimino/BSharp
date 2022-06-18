using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Randomization;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

[PublicAPI]
public static class CollectionRandomizationExtensions {
    /// <param name="collection">this <see cref="ICollection{T}"/></param>
    /// <param name="range">the <see cref="Range"/> of possible values</param>
    /// <param name="generator">an optional <see cref="System.Random"/> instance </param>
    /// <typeparam name="T">the type of the elements in this <see cref="ICollection{T}"/></typeparam>
    /// <exception cref="ArgumentOutOfRangeException">if the <see cref="Range"/> is outside of the <see cref="ICollection{T}"/></exception>
    private static void _RandomDoc<T>(this ICollection<T> collection, Range range, Random? generator = default) {
        // 📝 This method exists only to inherit the docs for other methods in this class!
    }

    /// <summary>
    /// <inheritdoc cref="Brandom.Index"/>
    /// </summary>
    /// <inheritdoc cref="_RandomDoc{T}"/>
    [Pure]
    public static int RandomIndex<T>(this ICollection<T> collection, Range range, Random? generator = default)
        => generator.Index(collection.Count, range);

    /// <summary>
    /// Retrieves a random element from this <see cref="ICollection{T}"/> inside of the given <see cref="Range"/>.
    /// </summary>
    /// <inheritdoc cref="_RandomDoc{T}"/>
    /// <returns>a <typeparamref name="T"/> element from this <see cref="ICollection{T}"/> inside of the <see cref="Range"/></returns>
    [Pure]
    public static T Random<T>(this ICollection<T> collection, Range range, Random? generator = default)
        => collection.ElementAt(collection.RandomIndex(range, generator));

    /// <summary>
    /// Retrieves a random element from this <see cref="ICollection{T}"/>.
    /// </summary>
    /// <remarks>
    /// This is equivalent to calling <see cref="Random{T}(System.Collections.Generic.ICollection{T},System.Range,System.Random?)"/>
    /// with <see cref="Range.All"/>.
    /// </remarks>
    /// <param name="collection">this <see cref="ICollection{T}"/></param>
    /// <param name="generator">an optional <see cref="System.Random"/> instance</param>
    /// <typeparam name="T"><inheritdoc cref="_RandomDoc{T}"/></typeparam>
    /// <returns>a random <typeparamref name="T"/> element from this <see cref="ICollection{T}"/></returns>
    [Pure]
    public static T Random<T>(this ICollection<T> collection, Random? generator = default) {
        return collection.Random(Range.All);
    }

    /// <summary>
    /// Similar to <see cref="Random{T}(System.Collections.Generic.ICollection{T},System.Random?)"/>, but <b><see cref="ICollection{T}.Remove"/>s the randomly selected item</b>.
    /// </summary>
    /// <param name="collection">the original <see cref="ICollection{T}"/></param>
    /// <param name="generator">the <see cref="System.Random"/> instance to generate random values with. Defaults to <see cref="Brandom.Gen"/></param>
    /// <typeparam name="T">the type of the elements in the original <see cref="ICollection{T}"/></typeparam>
    /// <returns>a <see cref="Random{T}(System.Collections.Generic.ICollection{T},System.Random?)"/> entry from <paramref name="collection"/></returns>
    public static T GrabRandom<T>(this ICollection<T> collection, Random? generator = default) {
        var randomEntry = collection.Random(generator);

        if (collection.Remove(randomEntry)) {
            return randomEntry;
        }

        throw new BrandonException($"Unable to remove a random entry from the {collection.GetType().Name}!");
    }

    /// <summary>
    /// Randomizes all of the entries in <paramref name="toBeRandomized"/>.
    /// </summary>
    /// <remarks>
    /// This returns <see langword="void"/> to match the signature of <see cref="List{T}.Sort()"/>.
    /// </remarks>
    /// <param name="toBeRandomized">the <see cref="ICollection{T}"/> that <i>will be modified</i></param>
    /// <param name="generator">an optional <see cref="System.Random"/> instance</param>
    /// <typeparam name="T">the type of the entries in <paramref name="toBeRandomized"/></typeparam>
    internal static void RandomizeEntries<T>(this ICollection<T> toBeRandomized, Random? generator = default) {
        if (toBeRandomized == null) {
            throw new ArgumentNullException(nameof(toBeRandomized));
        }

        var backupList = toBeRandomized.ToList();
        toBeRandomized.Clear();

        while (backupList.Any()) {
            toBeRandomized.Add(GrabRandom(backupList, generator));
        }
    }

    /// <summary>
    /// Randomizes the order of the entries in <paramref name="source"/>.
    /// </summary>
    /// <remarks>
    /// This is named <c>"Randomize"</c> to match <see cref="Enumerable.Reverse{TSource}"/>.
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="randomizer"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source, Random? randomizer = default) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        var ls = source.ToList();
        ls.RandomizeEntries(randomizer);
        return ls;
    }

    /// <summary>
    /// TODO: I would like it if this wasn't limited to <see cref="List{T}"/>, but that would require 2 type parameters...
    /// </summary>
    /// <param name="oldList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static IList<T> RandomCopy<T>(this List<T> oldList) {
        var copy = oldList.ToList();
        copy.RandomizeEntries();
        return copy;
    }
}