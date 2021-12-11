using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

using FowlFever.BSharp.Randomization;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Collections {
    [PublicAPI]
    public static class CollectionRandomizationExtensions {
        /// <param name="collection"></param>
        /// <param name="generator">the <see cref="System.Random"/> instance to generate random values with. Defaults to <see cref="Brandom.Gen"/></param>
        /// <typeparam name="T">The type of the <see cref="Collection{T}"/></typeparam>
        /// <returns>a random <see cref="Enumerable.ElementAt{TSource}"/> from the given <paramref name="collection"/>.</returns>
        [Pure]
        [ContractAnnotation("collection:null => stop")]
        public static T Random<T>(this ICollection<T> collection, Random? generator = default) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            generator ??= Brandom.Gen;

            return collection.Count switch {
                1 => collection.Single(),
                0 => throw new IndexOutOfRangeException($"Attempted to select a {nameof(Random)} element, but the given collection was empty!"),
                _ => collection.ElementAt(generator.Next(0, collection.Count))
            };
        }

        /// <summary>
        /// Similar to <see cref="Random{T}"/>, but <b><see cref="ICollection{T}.Remove"/>s the randomly selected item</b>.
        /// </summary>
        /// <param name="collection">the original <see cref="ICollection{T}"/></param>
        /// <param name="generator">the <see cref="System.Random"/> instance to generate random values with. Defaults to <see cref="Brandom.Gen"/></param>
        /// <typeparam name="T">the type of the elements in the original <see cref="ICollection{T}"/></typeparam>
        /// <returns>a <see cref="Random{T}"/> entry from <paramref name="collection"/></returns>
        [ContractAnnotation("collection:null => stop")]
        public static T? GrabRandom<T>(this ICollection<T?> collection, Random? generator = default) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            generator ??= new Random();

            var randomEntry = collection.Random(generator);
            collection.Remove(randomEntry);
            return randomEntry;
        }

        /// <summary>
        /// Randomizes all of the entries in <paramref name="toBeRandomized"/>.
        /// </summary>
        /// <remarks>
        /// This returns <see langword="void"/> to match the signature of <see cref="List{T}.Sort()"/>.
        /// </remarks>
        /// <param name="toBeRandomized">the <see cref="ICollection{T}"/> that <i>will be modified</i></param>
        /// <typeparam name="T">the type of the entries in <paramref name="toBeRandomized"/></typeparam>
        [ContractAnnotation("toBeRandomized:null => stop")]
        internal static void RandomizeEntries<T>(this ICollection<T?> toBeRandomized) {
            if (toBeRandomized == null) {
                throw new ArgumentNullException(nameof(toBeRandomized));
            }

            var backupList = toBeRandomized.Copy();
            toBeRandomized.Clear();

            while (backupList.Any()) {
                toBeRandomized.Add(GrabRandom(backupList));
            }
        }

        /// <summary>
        /// Randomizes the order of the entries in <paramref name="source"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="randomizer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [ContractAnnotation("source:null => stop")]
        [Pure]
        [LinqTunnel]
        public static IEnumerable<T?> Randomize<T>(this IEnumerable<T?> source, Random? randomizer = default) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var ls = source.ToList();
            ls.RandomizeEntries();
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
            var copy = oldList.Copy();
            copy.RandomizeEntries();
            return copy;
        }
    }
}