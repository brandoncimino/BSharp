using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Randomization;

[PublicAPI]
public static partial class Brandom {
    public static readonly Random Gen = new Random();

    /// <returns>a random <see cref="double"/> in the range of <c>[0..1]</c>, i.e. <c><![CDATA[0 <= x <= 1]]></c></returns>
    [Pure]
    public static double NextDoubleInclusive(this Random? generator) {
        const int nextIntMax = int.MaxValue - 1;
        return (double)generator.OrDefault().Next() / nextIntMax;
    }

    /// <param name="generator"></param>
    /// <returns>a random sign (either 1 or -1)</returns>
    [Pure]
    public static int Sign(this Random? generator) {
        return generator.OrDefault().Next(2) == 0 ? -1 : 1;
    }

    #region Near

    /// <summary>
    /// Returns a random value within <see cref="center"/> +- <see cref="radius"/>, <b>inclusive</b>.
    /// </summary>
    /// <param name="center">the center of the random range</param>
    /// <param name="radius">the size +- <see cref="center"/> to select a value from, <b>inclusively</b></param>
    /// <param name="generator">the <see cref="Random"/> instance that will generate numbers. Defaults to <see cref="Gen"/></param>
    /// <returns>a <see cref="double"/> that is up to <paramref name="radius"/> away from <paramref name="center"/></returns>
    [Pure]
    public static double Near(double center, double radius, Random? generator = default) {
        return generator.Near(center, radius);
    }

    /**
         * <inheritdoc cref="Near(double,double,System.Random)"/>
         */
    [Pure]
    public static double Near(this Random? generator, double center, double radius) {
        return center + generator.Double(-radius, radius);
    }

    #endregion

    #region Weighted random

    /// <param name="weightedList">of collection of possible items and their weights</param>
    /// <typeparam name="T">the type of the entries in the <paramref name="weightedList"/></typeparam>
    /// <returns>the <see cref="Enumerable.Sum(System.Collections.Generic.IEnumerable{decimal})"/> of the <paramref name="weightedList"/> weights</returns>
    /// <exception cref="ArgumentOutOfRangeException">if any of the weights are <c><![CDATA[< 0]]></c></exception>
    private static double GetTotalWeight<T>([InstantHandle] IEnumerable<(T choice, double weight)> weightedList) {
        double total = 0;
        foreach (var (_, weight) in weightedList) {
            if (weight.IsNegative()) {
                throw new ArgumentOutOfRangeException($"{nameof(weightedList)}.{weight}", weight, "Must be >= 0!");
            }

            total += weight;
        }

        return total;
    }

    /// <summary>
    /// Returns a <typeparamref name="T"/> item from <paramref name="weightedChoices"/> where each choice's likelihood is based on it's weight proportional to the total weight of all choices.
    /// </summary>
    /// <param name="generator">a <see cref="Random"/> instance. Defaults to <see cref="Gen"/> if null or omitted</param>
    /// <param name="weightedChoices">a collection of <typeparamref name="T"/> choices and their weights. <p><b>⚠ NOTE: ⚠</b></p>All of the weights must be >= 0!</param>
    /// <typeparam name="T">the type of the <paramref name="weightedChoices"/></typeparam>
    /// <returns>a random <typeparamref name="T"/> item from <paramref name="weightedChoices"/></returns>
    /// <exception cref="ArgumentNullException">if <paramref name="weightedChoices"/> is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">if any of the <paramref name="weightedChoices"/>' weights is negative</exception>
    /// <exception cref="BrandonException">if we fail to select any of the <paramref name="weightedChoices"/></exception>
    public static T FromWeightedList<T>(
        this            Random?                                generator,
        [InstantHandle] IEnumerable<(T choice, double weight)> weightedChoices
    ) {
        if (weightedChoices == null) {
            throw new ArgumentNullException(nameof(weightedChoices));
        }

        weightedChoices = weightedChoices.ToArray();

        if (weightedChoices.IsEmpty()) {
            throw new ArgumentNullException(nameof(weightedChoices));
        }

        var    totalWeight  = GetTotalWeight(weightedChoices);
        var    targetWeight = generator.Double(totalWeight);
        double soFar        = 0;

        foreach (var (item, weight) in weightedChoices) {
            soFar += weight;
            if (soFar >= targetWeight) {
                return item;
            }
        }

        throw new BrandonException($"Unable to select an entry from the weighted list! ({nameof(targetWeight)} = {targetWeight}, from {weightedChoices.Prettify()}");
    }

    public static T? FromWeightedList<T>(this Random? generator, [InstantHandle] IEnumerable<(T choice, int weight)> weightedChoices) {
        return generator.FromWeightedList(weightedChoices.Select(it => (it.choice, it.weight.ToDouble())));
    }

    /// <summary>
    /// Returns a <see cref="KeyValuePair{TKey,TValue}.Key"/> from the <paramref name="weightedChoices"/>,
    /// where each choice is weighted by its <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </summary>
    /// <param name="weightedChoices">an <see cref="IDictionary{TKey,TValue}"/> where the <see cref="IDictionary{TKey,TValue}.Keys"/> are the possible choices and the <see cref="IDictionary{TKey,TValue}.Values"/> are their weights. <p><b>⚠ NOTE: ⚠</b></p>All of the weights must be >= 0!</param>
    /// <param name="generator">a <see cref="Random"/> instance. Defaults to <see cref="Gen"/> if null or omitted.</param>
    /// <typeparam name="T">the outcome's <see cref="Type"/>.</typeparam>
    /// <returns>a random <see cref="KeyValuePair{TKey,TValue}.Key"/> from <see cref="weightedChoices"/>.</returns>
    /// <exception cref="ArgumentNullException">if <paramref name="weightedChoices"/> is null or empty</exception>
    /// <exception cref="ArgumentNullException">if any of the <paramref name="weightedChoices"/>' <see cref="IDictionary{TKey,TValue}.Keys"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">if any of the <paramref name="weightedChoices"/>' <see cref="IDictionary{TKey,TValue}.Values"/> <see cref="Mathb.IsNegative(double)"/></exception>
    /// <exception cref="BrandonException">if we fail to select an entry <i>(this should not occur under normal circumstances)</i></exception>
    public static T FromWeightedList<T>(this Random? generator, IDictionary<T, double> weightedChoices) {
        return generator.FromWeightedList(weightedChoices.Select(it => (it.Key, it.Value)))!;
    }

    public static T FromWeightedList<T>(this Random? generator, IDictionary<T, int> weightedChoices) {
        return generator.FromWeightedList(weightedChoices.Select(it => (it.Key, it.Value)))!;
    }

    /// <summary>
    /// Applies <paramref name="weightSelector"/> to each of the <paramref name="choices"/> and then selects one at random based on their relative weights.
    /// </summary>
    /// <param name="generator">a <see cref="Random"/> instance. Defaults to <see cref="Gen"/> if null or omitted</param>
    /// <param name="choices">a collection of <typeparamref name="T"/> choices to pick from</param>
    /// <param name="weightSelector">a <see cref="Func{TResult}"/> to determine the relative weight of each of the <paramref name="choices"/>. <p><b>⚠ NOTE: ⚠</b></p>All of the weights must be >= 0!</param>
    /// <typeparam name="T">the type of <paramref name="choices"/></typeparam>
    /// <returns>a random <typeparamref name="T"/> entry from <paramref name="choices"/></returns>
    /// <exception cref="ArgumentNullException">if <paramref name="choices"/> is null or empty</exception>
    /// <exception cref="ArgumentNullException">if <paramref name="weightSelector"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">if any of the results of <paramref name="weightSelector"/> are <c><![CDATA[< 0]]></c></exception>
    public static T? FromWeightedList<T>(
        this            Random?         generator,
        [InstantHandle] IEnumerable<T?> choices,
        Func<T, double>                 weightSelector
    ) {
        if (weightSelector == null) {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        choices = choices.ToArray();

        if (choices.IsEmpty()) {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        return generator.FromWeightedList(choices.Select(it => (it, weightSelector.Invoke(it))));
    }

    #endregion

    internal static Random OrDefault(this Random? generator) {
        return generator ?? Gen;
    }

    #region Primitives

    /// <param name="generator"></param>
    /// <returns>a random <see cref="bool"/> (true or false)</returns>
    [Pure]
    public static bool Bool(this Random? generator) => generator.OrDefault().Next(2) == 0;

    public static int      Int(this      Random? generator, int      min, int      max) => generator.OrDefault().Next(min, max);
    public static long     Long(this     Random? generator, long     min, long     max) => (min, max).LerpInt(generator.NextDoubleInclusive());
    public static float    Float(this    Random? generator, float    min, float    max) => (min, max).Lerp(generator.NextDoubleInclusive().ToFloat());
    public static double   Double(this   Random? generator, double   min, double   max) => (min, max).Lerp(generator.NextDoubleInclusive());
    public static decimal  Decimal(this  Random? generator, decimal  min, decimal  max) => (min, max).Lerp(generator.NextDoubleInclusive().ToDecimal());
    public static TimeSpan TimeSpan(this Random? generator, TimeSpan min, TimeSpan max) => (min, max).Lerp(generator.NextDoubleInclusive());
    public static DateTime DateTime(this Random? generator, DateTime min, DateTime max) => (min, max).Lerp(generator.NextDoubleInclusive());
    public static int      Int(this      Random? generator, int      max) => generator.Int(0, max);
    public static long     Long(this     Random? generator, long     max) => generator.Long(0, max);
    public static float    Float(this    Random? generator, float    max) => generator.Float(0, max);
    public static double   Double(this   Random? generator, double   max) => generator.Double(0, max);
    public static decimal  Decimal(this  Random? generator, decimal  max) => generator.Decimal(0, max);
    public static TimeSpan TimeSpan(this Random? generator, TimeSpan max) => generator.TimeSpan(default, max);
    public static DateTime DateTime(this Random? generator, DateTime max) => generator.DateTime(default, max);

    /**
         * <inheritdoc cref="NextDoubleInclusive"/>
         * <remarks>Shorthand for <see cref="NextDoubleInclusive"/></remarks>
         */
    public static double Double(this Random? generator) => generator.NextDoubleInclusive();

    #endregion

    #region Ranges and Indices

    /// <summary>
    /// Generates an <see cref="int"/> index inside of the given <see cref="Range"/> for a collection of size <paramref name="length"/>.
    /// </summary>
    /// <param name="generator">a <see cref="Random"/> instance. Defaults to <see cref="Gen"/></param>
    /// <param name="length">the (length / count / size) of the collection that the <see cref="Range"/> will be used with</param>
    /// <param name="range">the <see cref="Range"/> we'd like to be between</param>
    /// <returns>a valid <see cref="int"/> index inside of the <see cref="Range"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="length"/> is negative</exception>
    /// <exception cref="ArgumentOutOfRangeException">if the <see cref="Range"/> is outside of the <paramref name="length"/>'s scope</exception>
    public static int Index(this Random? generator, [NonNegativeValue] int length, Range range) {
        Must.BePositive(length);
        var (o, l) = range.GetOffsetAndLength(length);
        return o + generator.Int(l);
    }

    #endregion

    #region Enums

    /// <param name="generator">a <see cref="Random"/> generator. Defaults to <see cref="Gen"/></param>
    /// <param name="includeAliases">if <c>true</c>, then duplicate values with the same value but different <see cref="Enum.GetName"/></param>
    /// <typeparam name="T">the type of <see cref="Enum"/></typeparam>
    /// <returns>a random <typeparamref name="T"/> value</returns>
    public static T Enum<T>(this Random? generator, bool includeAliases = false) where T : Enum {
        var source = includeAliases ? BEnum.GetAllValues<T>() : BEnum.GetValues<T>();
        return source.Random(generator);
    }

    #endregion
}