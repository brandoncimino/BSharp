using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// Contains utility and extension methods for collections, such as <see cref="IDictionary{TKey,TValue}" /> and <see cref="IList{T}" />.
/// </summary>
[PublicAPI]
public static partial class CollectionUtils {
    /// <summary>
    /// Retrieves and <see cref="ICollection{T}.Remove"/>s the <see cref="Enumerable.ElementAt{TSource}"/> <paramref name="index"/>
    /// </summary>
    /// <remarks>
    /// TODO: Revisit this method - it needs better null handling, for instance
    /// </remarks>
    /// <param name="collection"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Grab<T>(this ICollection<T> collection, int index) {
        var item = collection.ElementAt(index);
        collection.Remove(item);
        return item;
    }

    /// <summary>
    /// <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/>s an entry from this <see cref="IDictionary{TKey,TValue}"/>, returning the removed <see cref="TValue"/>.
    /// </summary>
    /// <remarks>
    /// This works identically to <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/>, except that it returns the actual removed value instead of a <see cref="bool"/>.
    /// </remarks>
    /// <param name="dictionary">this <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="key">the <see cref="KeyValuePair{TKey,TValue}.Key"/> that you'd like to remove</param>
    /// <typeparam name="TKey">the type of the <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TValue">the type of the <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>the removed <typeparamref name="TValue"/>, if any; otherwise, <c>null</c></returns>
    public static TValue? Grab<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
        if (dictionary.TryGetValue(key, out var val) && dictionary.Remove(key)) {
            return val;
        }

        return default;
    }

    [Pure]
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static List<T> ListOf<T>(params T[] stuff) {
        return new List<T>(stuff);
    }

    /// <summary>
    /// Equivalent to <see cref="Enumerable.OrderBy{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/> with each entry as the key.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="comparer">an optional <see cref="IComparer{T}"/> for instances of <typeparamref name="T"/>. Defaults to <see cref="Comparer.Default"/></param>
    /// <typeparam name="T">the type of each element</typeparam>
    /// <returns>an <see cref="IOrderedEnumerable{TElement}"/></returns>
    [Pure]
    [LinqTunnel]
    public static IOrderedEnumerable<T> Sorted<T>([NoEnumeration] this IEnumerable<T> source, IComparer<T>? comparer = default) => source.OrderBy(it => it, comparer);

    #region Copying

    /// <summary>
    /// Splits <paramref name="collection"/> into multiple <see cref="List{T}"/>s, whose sizes are determined by <paramref name="subGroups"/>.
    /// </summary>
    /// <param name="collection">The original <see cref="ICollection{T}"/></param>
    /// <param name="subGroups">The size of each resulting <see cref="List{T}"/>, in order</param>
    /// <typeparam name="T">The <see cref="Type"/> <typeparamref name="T"/> of <paramref name="collection"/></typeparam>
    /// <returns>A jagged <see cref="List{T}"/> of <see cref="List{T}"/>s, containing all of the elements of <paramref name="collection"/>, in order.</returns>
    /// <exception cref="Enumerable.Sum(System.Collections.Generic.IEnumerable{int})">if the <see cref="Enumerable"/> of <paramref name="subGroups"/> does not equal the <see cref="ICollection{T}"/> of <paramref name="collection"/></exception>
    public static List<List<T>> SplitCollection<T>(this ICollection<T> collection, ICollection<int> subGroups) {
        if (subGroups.Sum() != collection.Count) {
            throw new ArgumentOutOfRangeException(
                nameof(subGroups),
                $"The provided sub-groups do not total ({subGroups.Sum()}) the length of the collection ({collection.Count})!"
            );
        }

        var splitGroups  = new List<List<T>>();
        int currentIndex = 0;
        foreach (var subGroupSize in subGroups) {
            splitGroups.Add(collection.ToList().GetRange(currentIndex, subGroupSize));
            currentIndex += subGroupSize;
        }

        return splitGroups;
    }

    #region Dictionary Inversion

    public static IDictionary<TValue_Original, TKey_Original> Inverse_Internal<TKey_Original, TValue_Original>(IDictionary<TKey_Original, TValue_Original> dictionary) {
        //to make for specific and explicit error messages, we check for known failures conditions ahead of time - specifically, duplicate keys results in an "ArgumentException" which is the parent class of lots of other things, making it not very clear when we catch it...
        if (!dictionary.Values.IsSingleton()) {
            throw new ArgumentException($"The provided {dictionary.GetType().Name}'s {nameof(dictionary.Values)} contained one or more duplicates, so they couldn't be used as keys!");
        }

        if (dictionary.Any(pair => pair.Value == null)) {
            throw new ArgumentNullException($"The provided {dictionary.GetType().Name} contained a null {nameof(KeyValuePair<object, object>.Value)}, which can't be used as a {nameof(KeyValuePair<object, object>.Key)}!");
        }

        var inverted = dictionary.ToDictionary(pair => pair.Value, pair => pair.Key);

        if (dictionary is ReadOnlyDictionary<TKey_Original, TValue_Original>) {
            return new ReadOnlyDictionary<TValue_Original, TKey_Original>(inverted);
        }

        return new Dictionary<TValue_Original, TKey_Original>(inverted);
    }

    /// <inheritdoc cref="Inverse_Internal{TKey_Original,TValue_Original}"/>
    public static Dictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this Dictionary<TKey_Original, TValue_Original> dictionary) {
        return (Dictionary<TValue_Original, TKey_Original>)Inverse_Internal(dictionary);
    }

    /// <inheritdoc cref="Inverse_Internal{TKey_Original,TValue_Original}"/>
    public static ReadOnlyDictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this ReadOnlyDictionary<TKey_Original, TValue_Original> readOnlyDictionary) {
        return (ReadOnlyDictionary<TValue_Original, TKey_Original>)Inverse_Internal(readOnlyDictionary);
    }

    #endregion

    #endregion

    /// <summary>
    ///     Similarly to <see cref="List{T}.ForEach" />, this performs <paramref name="action" /> against each
    ///     <b>
    ///         <see cref="KeyValuePair{TKey,TValue}" />
    ///     </b>
    ///     in <paramref name="dictionary" />.
    /// </summary>
    /// <example>
    ///     Which variation of <see cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})">ForEach</see> is called depends on the first time the <paramref name="action" />'s <see cref="Delegate.Target" /> parameter is accessed.
    /// <p />For example, given:
    /// <code><![CDATA[
    /// Dictionary dictionary = new Dictionary<string, string>();
    /// ]]></code>
    /// Then the following would treat <b><i><c>it</c></i></b> as a <see cref="KeyValuePair{TKey,TValue}" />:
    /// <code><![CDATA[
    /// dictionary.ForEach(it => Console.WriteLine(it.Key);
    /// ]]></code>
    /// While the following would treat <b><i><c>it</c></i></b> as a <see cref="string" />:
    /// <code><![CDATA[
    /// dictionary.ForEach(it => Console.WriteLine(it.Length));
    /// ]]></code>
    /// While the following would <b>fail to compile</b>, due to attempting to treating <c>it</c> as both a <see cref="KeyValuePair{TKey,TValue}" /> and a <see cref="string" />:
    /// <code><![CDATA[
    /// dictionary.ForEach(it => Console.WriteLine($"{it.Key}, {it.Length}"));
    /// ]]></code>
    /// And the following would <b>fail to compile</b>, due to being ambiguous:
    /// <code><![CDATA[
    /// dictionary.ForEach(it => Console.WriteLine(it));
    /// ]]></code>
    /// </example>
    /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}" /> you would like to iterate over.</param>
    /// <param name="action">The <see cref="Action" /> that will be performed against each <see cref="KeyValuePair{TKey,TValue}" /> in <paramref name="dictionary" />.</param>
    /// <typeparam name="TKey">The type of <paramref name="dictionary" />'s <see cref="IDictionary{TKey,TValue}.Keys" /></typeparam>
    /// <typeparam name="TVal">The type of <paramref name="dictionary" />'s <see cref="IDictionary{TKey,TValue}.Values" /></typeparam>
    /// <seealso cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{T2})" />
    /// <seealso cref="List{T}.ForEach" />
    [ContractAnnotation("dictionary:null => stop")]
    [ContractAnnotation("action:null => stop")]
    public static void ForEach<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, [InstantHandle] Action<KeyValuePair<TKey, TVal>> action) {
        foreach (var pair in dictionary) {
            action.Invoke(pair);
        }
    }

    /// <summary>
    /// Similarly to <see cref="List{T}.ForEach"/>, this performs <paramref name="action"/> against each <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/> in this <paramref name="dictionary"/>.
    /// </summary>
    /// <remarks>
    /// This operates the same as <see cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})"/>, except that it "deconstructs" the <see cref="KeyValuePair{TKey,TValue}"/> into a separate <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </remarks>
    /// <param name="dictionary">the original <see cref="Dictionary{TKey,TValue}"/></param>
    /// <param name="action">the <see cref="Action{T1,T2}"/> executed against each <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <typeparam name="TKey">the type of <see cref="Dictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the type of <see cref="Dictionary{TKey,TValue}.Values"/></typeparam>
    [ContractAnnotation("dictionary:null => stop")]
    [ContractAnnotation("action:null => stop")]
    public static void ForEach<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, [InstantHandle] Action<TKey, TVal> action) {
        foreach (var pair in dictionary) {
            action.Invoke(pair.Key, pair.Value);
        }
    }

    /// <inheritdoc cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})"/>
    /// <summary>
    ///     Similarly to <see cref="List{T}.ForEach" />, this performs <paramref name="action" /> against each <b><see cref="KeyValuePair{TKey,TValue}.Value" /></b> in <paramref name="dictionary" />.
    /// </summary>
    [ContractAnnotation("dictionary:null => stop")]
    [ContractAnnotation("action:null => stop")]
    public static void ForEach<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, [InstantHandle] Action<TVal> action) {
        dictionary.Values.ToList().ForEach(action);
    }

    /// <summary>
    /// Calls <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/> against <paramref name="enumerable"/>.
    /// </summary>
    /// <remarks>
    /// Null <see cref="IEnumerable{T}"/>s are treated as <see cref="Enumerable.Empty{TResult}"/>.
    /// <br/>
    /// Corresponds roughly to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/stream/Collectors.html#joining--">.joining()</a> collector.
    /// </remarks>
    /// <param name="enumerable"></param>
    /// <param name="separator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string JoinString<T>(this IEnumerable<T?>? enumerable, string? separator = "") {
        return string.Join(separator, enumerable.OrEmpty());
    }

    /// <summary>
    /// An extension method version of <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/> that joins using the <c>\n</c> line break.
    /// </summary>
    /// <param name="enumerable">the <see cref="IEnumerable{T}"/> whose entries will be joined</param>
    /// <param name="label">an optional label applied to the <b>first line</b> of each entry</param>
    /// <param name="indent">an optional <see cref="string"/> prepended to each line</param>
    /// <typeparam name="T">the type of each <see cref="IEnumerable{T}"/> entry </typeparam>
    /// <returns>the result of <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/></returns>
    public static string JoinLines<T>(
        this IEnumerable<T> enumerable,
        string?             label  = default,
        string?             indent = default
    ) {
        return enumerable.SelectMany(it => it.ToStringLines().IndentWithLabel(label))
                         .Select(it => it.Prefix(indent))
                         .JoinString("\n");
    }

    /// <summary>
    /// Similar to <see cref="JoinLines{T}"/>, except that this method will recur onto any entries in the <see cref="IEnumerable{T}"/>
    /// which are themselves <see cref="IEnumerable{T}"/>s - essentially "flattening" the result.
    /// </summary>
    /// <example>
    /// This method endeavors to return meaningful <see cref="object.ToString"/> representations of individual entries, rather than useless <c>System.Object[]</c> nonsense:
    /// <code>
    /// var jaggedArray = new object[]{
    ///     1,
    ///     new string[]{ "a", "b", "c" },
    ///     new object[]{
    ///         "x",
    ///         new string[]{ "y","z" }
    ///     }
    /// }
    /// </code>
    /// Will produce:
    /// <code>
    /// 1
    /// a
    /// b
    /// c
    /// x
    /// y
    /// z
    /// </code>
    ///
    /// </example>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string JoinLinesFlatly<T>(this IEnumerable<T> enumerable) {
        return string.Join("\n", enumerable.ToStringLines());
    }

    /// <summary>
    /// Returns true if <paramref name="enumerable"/> contains only unique elements, as determined via <see cref="Enumerable.Distinct{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsSingleton<T>(this IEnumerable<T> enumerable) {
        var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
        return enumerable1.Length == enumerable1.Distinct().Count();
    }

    /// <summary>
    /// Computes the element-wise difference between each element of <paramref name="listToCompute"/>.
    /// </summary>
    /// <remarks>
    /// Each element in <paramref name="listToCompute"/> is subtracted from the element <b>following it</b>, i.e.:
    /// <code><![CDATA[
    /// Input:             [3, 1, 4]
    /// Calculations:      1-3   4-1
    /// Result:             [2, 3]
    /// ]]></code>
    /// </remarks>
    /// <param name="listToCompute"></param>
    /// <returns>A <see cref="List{T}"/> (with <see cref="List{T}.Count"/> equal to <paramref name="listToCompute"/>.<see cref="List{T}.Count"/> - 1) containing the pair-wise difference between each element of <paramref name="listToCompute"/></returns>
    public static List<int> GetListDiff(this List<int> listToCompute) {
        var diff = new List<int>();
        for (int i = 0; i < listToCompute.Count - 1; i++) {
            diff.Add(listToCompute[i + 1] - listToCompute[i]);
        }

        return diff;
    }

    #region Dictionary Joining

    /// <summary>
    /// Removes all of the <paramref name="keysToRemove"/> from <paramref name="original"/> via <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/>.
    /// </summary>
    /// <remarks>
    /// An error is <b>not</b> thrown when a key from <paramref name="keysToRemove"/> isn't found in <paramref name="original"/>.
    /// </remarks>
    /// <param name="original"></param>
    /// <param name="keysToRemove"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> original, IEnumerable<TKey> keysToRemove) {
        foreach (var key in keysToRemove) {
            original.Remove(key);
        }

        return original;
    }

    /// <summary>
    /// Calls <see cref="RemoveAll{TKey,TValue}(System.Collections.Generic.IDictionary{TKey,TValue},System.Collections.Generic.IEnumerable{TKey})"/> using <paramref name="dictionaryWithKeysToRemove"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/>.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="dictionaryWithKeysToRemove"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> dictionaryWithKeysToRemove) {
        return RemoveAll(original, dictionaryWithKeysToRemove.Keys);
    }

    /// <summary>
    /// The method by which <see cref="CollectionUtils.JoinDictionaries{TKey,TValue}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IDictionary{TKey,TValue}},FowlFever.BSharp.Collections.CollectionUtils.ConflictResolution)"/> will handle conflicts caused by duplicate <see cref="IDictionary{TKey,TValue}.Keys"/>.
    /// </summary>
    public enum ConflictResolution {
        /// <summary>
        /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, an exception will be thrown.
        /// </summary>
        Fail,
        /// <summary>
        /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, prefer to use the original's <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </summary>
        FavorOriginal,
        /// <summary>
        /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, prefer to use the new <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </summary>
        FavorNew
    }

    /// <summary>
    /// Returns a <see cref="IDictionary{TKey,TValue}"/> containing:
    /// <li>The <see cref="IDictionary{TKey,TValue}.Keys"/> that exist in both <paramref name="original"/> and <paramref name="additional"/>.</li>
    /// <li>The <see cref="IDictionary{TKey,TValue}.Values"/> for those keys from either <paramref name="original"/> or <paramref name="additional"/>, depending on <paramref name="conflictResolution"/>.</li>
    /// </summary>
    /// <remarks>
    /// The <see cref="IDictionary{TKey,TValue}.Keys"/> of the result will always be in the order they appeared <b>inside of <paramref name="original"/></b>, regardless of <paramref name="conflictResolution"/>.
    /// </remarks>
    /// <param name="original"></param>
    /// <param name="additional"></param>
    /// <param name="conflictResolution">The <see cref="ConflictResolution"/> to decide which <see cref="IDictionary{TKey,TValue}.Values"/> are returned.</param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="ConflictResolution.Fail">If <see cref="ConflictResolution"/> or an unknown <see cref="InvalidEnumArgumentException"/> is passed.</exception>
    [Pure]
    public static Dictionary<TKey, TValue> Overlap<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> additional, ConflictResolution conflictResolution) {
        var overlappingKeys = original.Keys.Where(additional.ContainsKey);

        switch (conflictResolution) {
            case ConflictResolution.Fail:
                throw new InvalidEnumArgumentException($"When specifically requesting {nameof(Overlap)}, you can't have a {nameof(ConflictResolution)} method of {ConflictResolution.Fail}! How would that make any sense?");
            case ConflictResolution.FavorOriginal:
                return overlappingKeys.ToDictionary(key => key, key => original[key]);
            case ConflictResolution.FavorNew:
                return overlappingKeys.ToDictionary(key => key, key => additional[key]);
            default:
                throw BEnum.InvalidEnumArgumentException(nameof(ConflictResolution), conflictResolution);
        }
    }

    /// <summary>
    /// Updates any <see cref="IDictionary{TKey,TValue}.Keys"/> in <paramref name="original"/> with their <see cref="KeyValuePair{TKey,TValue}.Value"/> from <paramref name="newValues"/>.
    /// <p/>
    /// <see cref="IDictionary{TKey,TValue}.Keys"/> found in <paramref name="newValues"/> but <b>not</b> in <paramref name="original"/> are ignored.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="newValues"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> UpdateFrom<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> newValues) {
        newValues.ForEach(
            pair => {
                if (original.ContainsKey(pair.Key)) {
                    original[pair.Key] = pair.Value;
                }
            }
        );
        return original;
    }

    /// <summary>
    /// Joins <b>all</b> of the given <paramref name="dictionaries"/> together via the given <see cref="ConflictResolution"/> method, returning a <b>new <see cref="IDictionary{TKey,TValue}"/></b>.
    /// </summary>
    /// <param name="dictionaries"></param>
    /// <param name="conflictResolution"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If <paramref name="conflictResolution"/> is <see cref="ConflictResolution.Fail"/> and there are duplicate <see cref="IDictionary{TKey,TValue}.Keys"/> in the <paramref name="dictionaries"/>.</exception>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    [Pure]
    public static Dictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
        IEnumerable<IDictionary<TKey, TValue>> dictionaries,
        ConflictResolution                     conflictResolution = ConflictResolution.Fail
    ) {
        var dicList = dictionaries.ToList();
        var allKeys = dicList.SelectMany(dic => dic.Keys).ToList();

        if (!allKeys.IsSingleton()) {
            switch (conflictResolution) {
                case ConflictResolution.Fail:
                    throw new ArgumentException($"Could not join the dictionaries because they had duplicate keys and the {nameof(conflictResolution)} method was set to {ConflictResolution.Fail}");
                case ConflictResolution.FavorNew:
                    dicList.Reverse();
                    break;
                case ConflictResolution.FavorOriginal:
                    //don't need to do nu'n
                    break;
                default:
                    throw BEnum.InvalidEnumArgumentException(nameof(conflictResolution), conflictResolution);
            }
        }

        return allKeys.Distinct()
                      .ToDictionary(
                          key => key,
                          key => dicList
                              .First(it => it.ContainsKey(key) && it[key] != null)[key]
                      );
    }

    /// <summary>
    /// Joins <paramref name="original"/> and <paramref name="additional"/> together via the given <see cref="ConflictResolution"/> method, returning a <b>new <see cref="IDictionary{TKey,TValue}"/></b>.
    /// TODO: Replace <see cref="IDictionary{TKey,TValue}"/> with <c><![CDATA[T : IDictionary<TKey, TValue>]]></c>
    /// </summary>
    /// <remarks>
    /// The order of the <see cref="IDictionary{TKey,TValue}.Keys"/> in the result will always be:
    /// <li><b>All</b> of the <see cref="IDictionary{TKey,TValue}.Keys"/> from <paramref name="original"/> (including any overlap).</li>
    /// <li>The <b>unique</b> <see cref="IDictionary{TKey,TValue}.Keys"/> from <see cref="additional"/>.</li>
    /// </remarks>
    /// <param name="original"></param>
    /// <param name="additional"></param>
    /// <param name="conflictResolution"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If <paramref name="conflictResolution"/> is <see cref="ConflictResolution.Fail"/> and there are duplicate <see cref="IDictionary{TKey,TValue}.Keys"/> between <paramref name="original"/> and <paramref name="additional"/>.</exception>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    [Pure]
    public static IDictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
        this IDictionary<TKey, TValue> original,
        IDictionary<TKey, TValue>      additional,
        ConflictResolution             conflictResolution = ConflictResolution.Fail
    ) {
        return JoinDictionaries(new[] { original, additional }, conflictResolution);
    }

    [Pure]
    public static IDictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
        this   IDictionary<TKey, TValue>   original,
        params IDictionary<TKey, TValue>[] additional
    ) {
        return JoinDictionaries(additional.Prepend(original));
    }

    [Pure]
    public static TValue FirstValue<TKey, TValue>(
        this IEnumerable<Dictionary<TKey, TValue>> dictionaries,
        TKey                                       key
    ) {
        return dictionaries.First(dic => dic.ContainsKey(key))[key];
    }

    /// <summary>
    /// A simplified version of <see cref="Enumerable.GroupBy{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/> that groups entries by equality.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static IDictionary<T, int> Group<T>(this IEnumerable<T> enumerable) {
        return enumerable.GroupBy(it => it).ToDictionary(it => it.Key, it => it.Count());
    }

    #endregion

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/>'s <typeparamref name="T"/> as a <see cref="Type"/>.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// var ls = new List<int>{ 1, 2, 3 };
    ///
    /// Assert.That(ls.ItemType() == typeof(int));
    /// ]]></code>
    /// </example>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static Type ItemType<T>(this IEnumerable<T?>? enumerable) {
        return typeof(T);
    }

    /// <summary>
    /// Shorthand for not-<see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// </summary>
    /// <param name="enumerable">this <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the <see cref="ItemType{T}"/></typeparam>
    /// <returns>the inverse of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></returns>
    /// TODO: Experiment on whether it makes sense to have a special version of <see cref="IsEmpty{T}"/> as an <see cref="IOptional{T}"/> extension method, which would return the inverse of <see cref="IOptional{T}.HasValue"/>. The problem is that this method causes ambiguity with the <see cref="IEnumerable{T}"/> version of <see cref="IOptional{T}"/>
    [Pure]
    public static bool IsEmpty<T>([NotNullWhen(false)] [InstantHandle] this IEnumerable<T>? enumerable) {
        return enumerable == null || !enumerable.Any();
    }

    /// <summary>
    /// A less hideous alias for <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static bool IsNotEmpty<T>([NotNullWhen(true)] [InstantHandle] this IEnumerable<T>? enumerable) {
        return enumerable != null && enumerable.Any();
    }

    /// <summary>
    /// Negation of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// </summary>
    /// <inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource}, Func{TSource,bool})"/>
    /// <returns>true if <b>none</b> of the items in <paramref name="source"/> satisfy <paramref name="predicate"/></returns>
    [Pure]
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
        return !source.Any(predicate);
    }

    #region Containment

    /// <summary>
    /// Returns true if the <see cref="IEnumerable{T}"/> contains <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> of the <paramref name="others"/>.
    /// </summary>
    /// <param name="enumerable">the containing stuff</param>
    /// <param name="others">the stuff it might contain</param>
    /// <typeparam name="T">the <see cref="Type"/> of stuff</typeparam>
    /// <returns>true if the <see cref="IEnumerable{T}"/> contains <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> of the <paramref name="others"/></returns>
    [Pure]
    public static bool ContainsAny<T>([InstantHandle] this IEnumerable<T> enumerable, [InstantHandle] IEnumerable<T> others) {
        return others.Any(enumerable.Contains);
    }

    /**
         * <inheritdoc cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    public static bool ContainsAny<T>(
        [InstantHandle]
        this IEnumerable<T> enumerable,
        T          another,
        T          andAnother,
        params T[] andMore
    ) {
        return ContainsAny(enumerable, andMore.Prepend(andAnother).Prepend(another));
    }

    /// <summary>
    /// Inverse of <see cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    /// </summary>
    /// <inheritdoc cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T?},System.Collections.Generic.IEnumerable{T?})"/>
    /// <returns>true if <see cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T?},System.Collections.Generic.IEnumerable{T?})"/> returns false</returns>
    [Pure]
    public static bool ContainsNone<T>(this IEnumerable<T> enumerable, IEnumerable<T> others) {
        return others.None(enumerable.Contains);
    }

    /**
         * <inheritdoc cref="ContainsNone{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    public static bool ContainsNone<T>(this IEnumerable<T> enumerable, params T[] others) {
        return ContainsNone(enumerable, (IEnumerable<T>)others);
    }

    /// <summary>
    /// Returns true if the <see cref="IEnumerable{T}"/> <see cref="Enumerable.Contains{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/> each of the given items.
    /// </summary>
    /// <param name="superset">the set that you are checking</param>
    /// <param name="subset">the items that might be in the <paramref name="superset"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static bool ContainsAll<T>([InstantHandle] this IEnumerable<T> superset, [InstantHandle] IEnumerable<T> subset) {
        return subset.All(superset.Contains);
    }

    /**
         * <inheritdoc cref="ContainsAll{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    public static bool ContainsAll<T>([InstantHandle] this IEnumerable<T> enumerable, params T[] others) {
        return ContainsAll(enumerable, others.AsEnumerable());
    }

    /// <summary>
    /// Returns true if the first <see cref="IEnumerable{T}"/> is a <a href="https://en.wikipedia.org/wiki/Superset">superset</a> of the second <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <remarks>
    /// This method delegates to <see cref="ISet{T}.IsSupersetOf"/>, if possible; otherwise, it delegates to <see cref="ContainsAll{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>. 
    /// </remarks>
    /// <param name="superset">the <b>larger</b> set</param>
    /// <param name="subset">the <b>smaller</b> set</param>
    /// <typeparam name="T">the <see cref="Type"/> of each individual item</typeparam>
    /// <returns>true if the first <see cref="IEnumerable{T}"/> is a <a href="https://en.wikipedia.org/wiki/Superset">superset</a> of the second <see cref="IEnumerable{T}"/></returns>
    /// <seealso cref="ContainsAll{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    /// <seealso cref="IsSubsetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    [Pure]
    public static bool IsSupersetOf<T>([InstantHandle] this IEnumerable<T> superset, [InstantHandle] IEnumerable<T> subset) {
        if (superset is ISet<T> set) {
            return set.IsSupersetOf(subset);
        }

        return ContainsAll(superset, subset);
    }

    /**
         * <inheritdoc cref="IsSupersetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    public static bool IsSupersetOf<T>([InstantHandle] this IEnumerable<T> superset, params T[] subset) => superset.IsSupersetOf(subset.AsEnumerable());

    /// <summary>
    /// Inverse of <see cref="IsSupersetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>.
    /// </summary>
    /// <inheritdoc cref="IsSupersetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    /// <returns>inverse of <see cref="IsSupersetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/></returns>
    [Pure]
    public static bool IsNotSupersetOf<T>(this IEnumerable<T?> superset, IEnumerable<T?> subset) {
        return !IsSupersetOf(superset, subset);
    }

    /// <summary>
    /// Returns true if the first <see cref="IEnumerable{T}"/> is a <a href="https://en.wikipedia.org/wiki/Subset">subset</a> of the second <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="subset">the <b>smaller</b> set</param>
    /// <param name="superset">the <b>larger</b> set</param>
    /// <typeparam name="T">the <see cref="Type"/> of each individual item</typeparam>
    /// <returns>true if the first <see cref="IEnumerable{T}"/> is a <a href="https://en.wikipedia.org/wiki/Subset">subset</a> of the second <see cref="IEnumerable{T}"/></returns>
    /// <remarks>This method delegates to <see cref="ISet{T}.IsSubsetOf"/>, if possible; otherwise, it delegates to <see cref="ContainsAll{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/></remarks>
    [Pure]
    public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> superset) {
        if (subset is ISet<T> set) {
            return set.IsSubsetOf(superset);
        }

        return ContainsAll(superset, subset);
    }

    /// <summary>
    /// Inverse of <see cref="IsSubsetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    /// </summary>
    /// <inheritdoc cref="IsSubsetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
    /// <returns>inverse of <see cref="IsSubsetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/></returns>
    [Pure]
    public static bool IsNotSubsetOf<T>(this IEnumerable<T?> subset, IEnumerable<T?> superset) => !IsSubsetOf(superset, subset);

    /**
         * <inheritdoc cref="IsSubsetOf{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    public static bool IsSubsetOf<T>(this IEnumerable<T> subset, params T[] superset) => subset.IsSubsetOf(superset.AsEnumerable());

    /// <summary>
    /// A simple inverse of <see cref="Enumerable.Contains{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/>.
    /// </summary>
    /// <param name="source">A sequence in which to locate a value.</param>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <returns>the inverse of <see cref="Enumerable.Contains{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/></returns>
    [Pure]
    public static bool DoesNotContain<T>([InstantHandle] this IEnumerable<T?> source, T? value) {
        return !source.Contains(value);
    }

    /// <summary>
    /// A simple inverse of <see cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T?},System.Collections.Generic.IEnumerable{T?})"/>.
    /// </summary>
    /// <param name="source">the collection being checked</param>
    /// <param name="unwanted">the things we don't want in <paramref name="source"/></param>
    /// <typeparam name="T">the type of the items in the collections</typeparam>
    /// <returns>the inverse of <see cref="ContainsAny{T}(System.Collections.Generic.IEnumerable{T?},System.Collections.Generic.IEnumerable{T?})"/></returns>
    [Pure]
    public static bool DoesNotContainAny<T>([InstantHandle] this IEnumerable<T?> source, IEnumerable<T?> unwanted) {
        return !source.ContainsAny(unwanted);
    }

    #endregion

    /// <summary>
    /// TODO: Is it correct for this to be an <see cref="ICollection{T}"/> extension, rather than <see cref="IEnumerable{T}"/>?
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="newItem"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool AddIfMissing<T>(
        this ICollection<T> collection,
        T                   newItem
    ) {
        if (collection.Contains(newItem)) {
            return false;
        }

        collection.Add(newItem);
        return true;
    }

    [LinqTunnel]
    [ContractAnnotation("source:null => stop")]
    public static IEnumerable<T?> Union<T>(
        this IEnumerable<T?> source,
        T?                   newItem
    ) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Union(Enumerable.Repeat(newItem, 1));
    }

    [LinqTunnel]
    public static IEnumerable<T> Union<T>(
        this   IEnumerable<T> source,
        params T[]            others
    ) {
        return source.Union(others.AsEnumerable());
    }

    /// <summary>
    /// Both <see cref="Enumerable.Append{TSource}"/>s and <see cref="Enumerable.Prepend{TSource}"/>s <paramref name="bookend"/> to <paramref name="source"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="bookend">the <typeparamref name="T"/> element to be both <see cref="Enumerable.Append{TSource}">appended</see> and <see cref="Enumerable.Prepend{TSource}">prepended</see></param>
    /// <typeparam name="T">the type of the elements of <paramref name="source"/></typeparam>
    /// <returns>a new sequence that begins <b>and</b> ends with <paramref name="bookend"/></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T?> Bookend<T>(this IEnumerable<T?> source, T? bookend) {
        return source
               .Prepend(bookend)
               .Append(bookend);
    }

    [Pure]
    [LinqTunnel]
    public static IEnumerable<T?> Bookend<T>(this IEnumerable<T?> source, T? prefix, T? suffix) {
        return source
               .Prepend(prefix)
               .Append(suffix);
    }

    #region AppendNonNull

    /// <summary>
    /// <see cref="Enumerable.Append{TSource}"/>s a <typeparamref name="T"/> element to <paramref name="source"/> if it isn't <c>null</c>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="valueThatMightBeNull">the additional element to maybe add</param>
    /// <typeparam name="T">the type of the elements of <paramref name="source"/></typeparam>
    /// <returns>a new sequence that ends with <paramref name="valueThatMightBeNull"/> if it wasn't <c>null</c></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T?> AppendNonNull<T>(
        this IEnumerable<T?>? source,
        T?                    valueThatMightBeNull
    ) {
        source = source.EmptyIfNull();
        return valueThatMightBeNull == null ? source : source.Append(valueThatMightBeNull);
    }

    /// <summary>
    /// <see cref="Enumerable.Concat{TSource}"/>s all of the <see cref="NonNull{T}(System.Collections.Generic.IEnumerable{T})"/> entries in <paramref name="additionalValuesThatMightBeNull"/> to <paramref name="source"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="additionalValuesThatMightBeNull">a sequence of values that might be <c>null</c></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> AppendNonNull<T>(
        this IEnumerable<T> source,
        IEnumerable<T?>?    additionalValuesThatMightBeNull
    ) {
        source = source.EmptyIfNull();
        return additionalValuesThatMightBeNull == null ? source : source.Concat(additionalValuesThatMightBeNull.NonNull());
    }

    /**
         * <inheritdoc cref="AppendNonNull{T}(System.Collections.Generic.IEnumerable{T},T)"/>
         */
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> AppendNonNull<T>(
        this IEnumerable<T>? source,
        T?                   valueThatMightBeNull
    )
        where T : struct {
        source = source.EmptyIfNull();
        return valueThatMightBeNull.HasValue == false ? source : source.Append(valueThatMightBeNull.Value);
    }

    /**
         * <inheritdoc cref="AppendNonNull{T}(System.Collections.Generic.IEnumerable{T},IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> AppendNonNull<T>(
        this IEnumerable<T>? source,
        IEnumerable<T?>?     additionalValuesThatMightBeNull
    )
        where T : struct {
        return source.EmptyIfNull().Concat(additionalValuesThatMightBeNull.NonNull());
    }

    #endregion

    #region PrependNonNull

    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> PrependNonNull<T>(
        this IEnumerable<T> source,
        T?                  valueThatMightBeNull
    ) {
        return valueThatMightBeNull == null ? source : source.Prepend(valueThatMightBeNull);
    }

    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> PrependNonNull<T>(
        this IEnumerable<T> source,
        IEnumerable<T?>     additionalValuesThatMightBeNull
    ) {
        return additionalValuesThatMightBeNull.NonNull().Concat(source);
    }

    #endregion

    #region AddNonNull

    /// <summary>
    /// <see cref="ICollection{T}.Add"/>s <paramref name="valueThatMightBeNull"/> to <paramref name="source"/> if it isn't <c>null</c>.
    /// </summary>
    /// <remarks>
    /// From Brandon on 12/10/2021: maybe this could benefit (in the future) from covariant return types?
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="valueThatMightBeNull"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TElements"></typeparam>
    /// <returns></returns>
    public static TSource AddNonNull<TSource, TElements>(this TSource source, TElements? valueThatMightBeNull)
        where TSource : ICollection<TElements> {
        if (valueThatMightBeNull != null) {
            source.Add(valueThatMightBeNull);
        }

        return source;
    }

    /**
         * <inheritdoc cref="AddNonNull{TSource,TElements}(TSource,TElements)"/>
         */
    public static TSource AddNonNull<TSource, TElements>(this TSource source, TElements? valueThatMightBeNull)
        where TSource : ICollection<TElements>
        where TElements : struct {
        if (valueThatMightBeNull != null) {
            source.Add(valueThatMightBeNull.Value);
        }

        return source;
    }

    /// <summary>
    /// Actually <see cref="ICollection{T}.Add"/>s all of the non-null items from <paramref name="additionalValuesThatMightBeNull"/> to <paramref name="source"/>.
    /// </summary>
    /// <param name="source">the <see cref="ICollection{T}"/> being added to</param>
    /// <param name="additionalValuesThatMightBeNull">the items that we might want to add to <paramref name="source"/></param>
    /// <typeparam name="TSource">the <see cref="ICollection{T}"/> type of <paramref name="source"/></typeparam>
    /// <typeparam name="TElements">the type of each entry in <paramref name="source"/></typeparam>
    /// <returns>the original <paramref name="source"/></returns>
    public static TSource AddNonNull<TSource, TElements>(this TSource source, IEnumerable<TElements?>? additionalValuesThatMightBeNull)
        where TSource : ICollection<TElements> {
        if (additionalValuesThatMightBeNull == null) {
            return source;
        }

        additionalValuesThatMightBeNull.NonNull().ForEach(source.Add);
        return source;
    }

    /// <summary>
    /// Adds entries that <see cref="Nullable{T}.HasValue"/> to <paramref name="source"/>.
    /// </summary>
    /// <param name="source">the original <see cref="ICollection{T}"/> of a <see cref="ValueType"/></param>
    /// <param name="nullableValues">a sequence of <see cref="Nullable{T}"/> values that will might add to <paramref name="source"/></param>
    /// <typeparam name="TSource">the <see cref="ICollection{T}"/> type of <paramref name="source"/></typeparam>
    /// <typeparam name="TElements">the <see cref="ValueType"/> of <paramref name="source"/>'s entries (which are <see cref="Nullable{T}"/> in <paramref name="nullableValues"/>)</typeparam>
    /// <returns>the original <paramref name="source"/></returns>
    public static TSource AddNonNull<TSource, TElements>(this TSource source, [ItemNotNull] IEnumerable<TElements?>? nullableValues)
        where TSource : ICollection<TElements>
        where TElements : struct {
        if (nullableValues == null) {
            return source;
        }

        nullableValues.NonNull().ForEach(source.Add);
        return source;
    }

    #endregion

    #region NonNull

    /// <summary>
    /// Returns only the non-<c>null</c> entries from <paramref name="source"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the type of the items in <paramref name="source"/></typeparam>
    /// <returns>a new sequence containing only the non-<c>null</c> entries from <paramref name="source"/></returns>
    [ItemNotNull]
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T?>? source) {
        return (source == null ? Enumerable.Empty<T>() : source.Where(it => it != null))!;
    }

    /**
         * <inheritdoc cref="NonNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         * <remarks>
         * This method specifically operates on <see cref="IEnumerable{T}"/>s of <see cref="Nullable{T}"/>s.
         *
         * The entries in the output will be "un-boxed", i.e. <typeparamref name="T"/> rather than <c>T?</c>
         * </remarks>
         */
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T?>? source)
        where T : struct {
        return source == null ? Enumerable.Empty<T>() : source.Where(it => it.HasValue).Select(it => it!.Value);
    }

    /// <summary>
    /// Casts an <see cref="IEnumerable{T}"/> of <see cref="ValueType"/>s as <see cref="System.Nullable{T}"/>s.
    /// </summary>
    /// <param name="source">an <see cref="IEnumerable{T}"/> of <see cref="ValueType"/>s</param>
    /// <typeparam name="T">a <see cref="ValueType"/></typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> of <see cref="System.Nullable{T}"/>s</returns>
    public static IEnumerable<T?> Nullable<T>(this IEnumerable<T> source)
        where T : struct => source.Cast<T?>();

    #region NonBlank

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// <a href="https://stackoverflow.com/a/60205019">stackoverflow</a>
    /// </remarks>
    /// <param name="lines"></param>
    /// <returns></returns>
    /// <seealso cref="NonEmpty"/>
    public static IEnumerable<string> NonBlank(this IEnumerable<string?>? lines) {
        return (lines == null ? Enumerable.Empty<string>() : lines.Where(it => it.IsNotBlank()))!;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if any of the <paramref name="lines"/> <see cref="StringUtils.IsBlank"/>.
    /// </summary>
    /// <param name="lines">the <see cref="string"/>s being validated</param>
    /// <returns>a collection of non-nullable <see cref="string"/>s</returns>
    /// <exception cref="ArgumentNullException">if any of the <paramref name="lines"/> <see cref="StringUtils.IsBlank"/></exception>
    public static IEnumerable<string> MustNotContainBlank(this IEnumerable<string?> lines) {
        return lines.Select(it => it.IsNotBlank() ? it! : throw new ArgumentNullException(nameof(lines), $"Contained a blank string: {lines.Prettify()}"));
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// <a href="https://stackoverflow.com/a/60205019">stackoverflow</a>
    /// </remarks>
    /// <param name="lines"></param>
    /// <returns></returns>
    /// <seealso cref="NonBlank"/>
    public static IEnumerable<string> NonEmpty(this IEnumerable<string?>? lines) {
        return (lines == null ? Enumerable.Empty<string>() : lines.Where(it => it.IsNotEmpty()))!;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if any of the <paramref name="lines"/> <see cref="StringUtils.IsEmpty"/>.
    /// </summary>
    /// <param name="lines">the <see cref="string"/>s being validated</param>
    /// <returns>a collection of non-nullable <see cref="string"/>s</returns>
    /// <exception cref="ArgumentNullException">if any of the <paramref name="lines"/> <see cref="StringUtils.IsEmpty"/></exception>
    public static IEnumerable<string> MustNotContainEmpty(this IEnumerable<string?> lines) {
        return lines.Select(it => it.IsNotEmpty() ? it! : throw new ArgumentNullException(nameof(lines), $"Contained an empty string: {lines.Prettify()}"));
    }

    #endregion

    #endregion

    #region Finding

    /// <remarks>
    /// This is similar to the built-in <see cref="Enumerable.First{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>, however,
    /// this method will return <see cref="Optional.Empty{T}"/> when <paramref name="source"/> is null or empty instead of
    /// throwing an exception.
    /// </remarks>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="predicate">a function used to filter the entries in <paramref name="source"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>an <see cref="Optional{T}"/> containing the <typeparamref name="T"/> value that matched the <paramref name="predicate"/></returns>
    public static Optional<T?> FindFirst<T>(
        [InstantHandle]
        this IEnumerable<T?>? source,
        [InstantHandle]
        Func<T?, bool>? predicate = default
    ) {
        source = predicate == default ? source.EmptyIfNull() : source.EmptyIfNull().Where(predicate);
        return source.Take(1).ToOptional();
    }

    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns>an <see cref="Optional{T}"/> containing the <typeparamref name="TValue"/> of <paramref name="key"/> if <paramref name="source"/> <see cref="IDictionary{TKey,TValue}.ContainsKey"/>; otherwise, an empty <see cref="Optional"/></returns>
    [Pure]
    public static Optional<TValue> Find<TKey, TValue>(
        this IDictionary<TKey, TValue>? source,
        TKey                            key
    ) {
        source = source.OrEmpty();
        return source.ContainsKey(key) ? Optional.Optional.Of(source[key]) : default(Optional<TValue>);
    }

    /// <inheritdoc cref="Find{TKey,TValue}(System.Collections.Generic.IDictionary{TKey,TValue},TKey)"/>
    [Pure]
    public static Optional<TValue> Find<TKey, TValue>(
        [ItemCanBeNull]
        this KeyedCollection<TKey, TValue>? source,
        TKey key
    ) {
        if (source == null) {
            return default;
        }

        return source.Contains(key) ? source[key] : default(Optional<TValue>);
    }

    #region Finding indexes

    /// TODO: alternate names could be: FirstIndexWhere, FirstIndexSatisfying
    /// TODO: This can probably be made into some fancy thing that doesn't _necessarily_ enumerate <paramref name="source"/>, likely using <see cref="IEnumerable{T}.GetEnumerator"/> or something like that
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>the index of the first entry in the <paramref name="source"/> that satisfies the <paramref name="predicate"/>; or null if none was found</returns>
    public static int? FirstIndexOf<T>([InstantHandle] this IEnumerable<T>? source, [InstantHandle] Func<T, bool> predicate) {
        var sourceArray = source.EmptyIfNull().ToArray();
        for (int i = 0; i < sourceArray.Length; i++) {
            if (predicate.Invoke(sourceArray[i])) {
                return i;
            }
        }

        return null;
    }

    #endregion

    #region TakeLast

    /// <summary>
    /// Combines <see cref="Enumerable.TakeWhile{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/> and <see cref="Enumerable.Last{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="takePredicate">the condition used to <see cref="Enumerable.TakeWhile{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/></param>
    /// <typeparam name="T">the type of the items in <paramref name="source"/></typeparam>
    /// <returns><see cref="Enumerable.TakeWhile{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/>.<see cref="Enumerable.Last{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></returns>
    [Pure]
    [ContractAnnotation("source:null => stop")]
    [ContractAnnotation("takePredicate:null => stop")]
    public static T TakeLast<T>(
        [InstantHandle]
        this IEnumerable<T> source,
        Func<T, bool> takePredicate
    ) {
        return source.TakeWhile(takePredicate).Last();
    }

    #endregion JustBefore

    #endregion Finding

    #region EmptyIfNull

    /// <param name="source">the <see cref="IEnumerable{T}"/> that might be null</param>
    /// <typeparam name="T">the type of the elements in <paramref name="source"/></typeparam>
    /// <returns><paramref name="source"/>, or an <see cref="Enumerable.Empty{TResult}"/> if <paramref name="source"/> was null</returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> EmptyIfNull<T>([NoEnumeration] this IEnumerable<T>? source) {
        return source ?? Enumerable.Empty<T>();
    }

    /**
         * <inheritdoc cref="EmptyIfNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source) {
        return source.EmptyIfNull();
    }

    /**
         * <inheritdoc cref="EmptyIfNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static T[] EmptyIfNull<T>(this T[]? source) {
        return source ?? Array.Empty<T>();
    }

    /**
         * <inheritdoc cref="EmptyIfNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static T[] OrEmpty<T>(this T[]? source) {
        return source.EmptyIfNull();
    }

    /**
         * <inheritdoc cref="EmptyIfNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static IDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(this IDictionary<TKey, TValue>? source) {
        return source ?? new Dictionary<TKey, TValue>();
    }

    /**
         * <inheritdoc cref="EmptyIfNull{T}(System.Collections.Generic.IEnumerable{T})"/>
         */
    [Pure]
    [LinqTunnel]
    public static IDictionary<TKey, TValue> OrEmpty<TKey, TValue>(this IDictionary<TKey, TValue>? source) {
        return source.EmptyIfNull();
    }

    #endregion

    #region Intersection

    [ItemNotNull]
    public static IEnumerable<T> Intersection<T>(
        this IEnumerable<T>     source,
        IEnumerable<T>          second,
        params IEnumerable<T>[] additional
    ) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        if (second == null) {
            throw new ArgumentNullException(nameof(second));
        }

        if (additional == null) {
            throw new ArgumentNullException(nameof(additional));
        }

        return additional.Prepend(source).Prepend(second).Intersection();
    }

    [ItemNotNull]
    public static IEnumerable<T> Intersection<T>(
        [InstantHandle]
        this IEnumerable<T> source,
        [InstantHandle]
        IEnumerable<IEnumerable<T>> others
    ) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        if (others == null) {
            throw new ArgumentNullException(nameof(others));
        }

        return others.Prepend(source).Intersection();
    }

    [ItemNotNull]
    public static IEnumerable<T> Intersection<T>([InstantHandle] this IEnumerable<IEnumerable<T>> sources) {
        if (sources == null) {
            throw new ArgumentNullException(nameof(sources));
        }

        return sources.Aggregate((soFar, next) => soFar.Intersect(next)).Distinct();
    }

    #endregion

    #region AsReadOnly

    /// <summary>
    /// Returns a <see cref="ReadOnlyCollection{T}"/> wrapper around this <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IList{T}"/></param>
    /// <typeparam name="T">the type of the elements of <paramref name="source"/></typeparam>
    /// <returns>a <see cref="ReadOnlyCollection{T}"/> wrapper around this <see cref="IList{T}"/></returns>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> source) {
        return new ReadOnlyCollection<T>(source);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyDictionary{TKey,TValue}"/> wrapper around this <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <typeparam name="TKey">the type of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TValue">the type of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a <see cref="ReadOnlyDictionary{TKey,TValue}"/> wrapper around this <see cref="IDictionary{TKey,TValue}"/></returns>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> source) {
        return new ReadOnlyDictionary<TKey, TValue>(source);
    }

    #endregion

    #region ToHashSet

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = default) {
        return new HashSet<T>(source, comparer);
    }

    #endregion

    #region Dictionaries After Dark

    /// <summary>
    /// Shorthand to go from an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s back to an <see cref="IDictionary{TKey,TValue}"/> via <see cref="Enumerable.ToDictionary{TSource,TKey,TValue}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey},System.Func{TSource,TValue})"/>
    /// </summary>
    /// <param name="source">a collection of <see cref="KeyValuePair{TKey,TValue}"/>s</param>
    /// <typeparam name="TKey">the type of <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the type of <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a new <see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<TKey, TVal> ToDictionary<TKey, TVal>([InstantHandle] this IEnumerable<KeyValuePair<TKey, TVal>> source) {
        return source.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    /// <summary>
    /// Converts a non-generic <see cref="IDictionary"/> to an <see cref="IDictionary{TKey,TValue}"/> of <see cref="object"/>s.
    /// </summary>
    /// <remarks>
    /// This essentially uses <see cref="Enumerable.Cast{TResult}"/> on both the <see cref="IDictionary.Keys"/> and <see cref="IDictionary.Values"/>.
    /// </remarks>
    /// <param name="dictionary">the original, non-generic <see cref="IDictionary"/></param>
    /// <returns>a generic <see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<object, object> ToGeneric(this IDictionary dictionary) {
        var keys = dictionary.Keys.Cast<object>().ToArray();
        var vals = dictionary.Values.Cast<object>().ToArray();
        return keys.Select((k, i) => new KeyValuePair<object, object>(k, vals[i])).ToDictionary();
    }

    /// <summary>
    /// Similar to <see cref="NonNull{T}(System.Collections.Generic.IEnumerable{T})"/>, but checks for null <see cref="KeyValuePair{TKey,TValue}"/>.<see cref="KeyValuePair{TKey,TValue}.Value"/>s in a <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVal"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TVal> NonNull<TKey, TVal>(this IDictionary<TKey, TVal> source) {
        return source.Where(it => it.Value != null).ToDictionary();
    }

    /// <summary>
    /// Performs a <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})"/> that only transforms the <see cref="IDictionary{TKey,TValue}.Values"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="selector">the <see cref="Func{TOld,TNew}"/> applied to each <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <typeparam name="TKey">the type of the <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TOld">the type of the original <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <typeparam name="TNew">the type of the new <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a new <see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<TKey, TNew> SelectValues<TKey, TOld, TNew>(this IDictionary<TKey, TOld> source, Func<TOld, TNew> selector) {
        return source.ToDictionary(
            it => it.Key,
            it => selector(it.Value)
        );
    }

    /// <summary>
    /// A variation of the built-int <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>
    /// that "deconstructs" the <see cref="KeyValuePair{TKey,TValue}"/> to a separate <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="selector">the <see cref="Func{TResult}"/> that transforms each <see cref="KeyValuePair{TKey,TValue}"/></param>
    /// <typeparam name="TKey">the <see cref="Type"/> of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TOld">the <see cref="Type"/> of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <typeparam name="TNew">the <see cref="Type"/> of the <b>new</b> <see cref="IDictionary.Values"/></typeparam>
    /// <returns>a new <see cref="Dictionary{TKey,TValue}"/></returns>
    public static Dictionary<TKey, TNew> ToDictionary<TKey, TOld, TNew>(this IDictionary<TKey, TOld> source, Func<TKey, TOld, TNew> selector) {
        return source.ToDictionary(
            kvp => kvp.Key,
            kvp => selector(kvp.Key, kvp.Value)
        );
    }

    /// <summary>
    /// A variation of the built-in <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})"/>
    /// that separates the <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/> before sending them to the <paramref name="selector"/> <see cref="Func{T1,T2,TResult}"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="selector">the <see cref="Func{T1,T2,TResult}"/> that performs a transformation against each <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <typeparam name="TKey">the type of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the type of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <typeparam name="TNew">the type of the new <see cref="IEnumerable{T}"/></typeparam>
    /// <returns>a new <see cref="IEnumerable{T}"/> containing the results of the <paramref name="selector"/></returns>
    /// <seealso cref="ForEach{TKey,TVal}(System.Collections.Generic.IDictionary{TKey,TVal},System.Action{TKey,TVal})"/>
    [Pure]
    public static IEnumerable<TNew> Select<TKey, TVal, TNew>(this IDictionary<TKey, TVal> source, Func<TKey, TVal, TNew> selector) {
        return source.Select(kvp => selector.Invoke(kvp.Key, kvp.Value));
    }

    /// <summary>
    /// Returns a new <see cref="IDictionary{TKey,TValue}"/> containing only the elements of <paramref name="source"/> whose <see cref="KeyValuePair{TKey,TValue}.Value"/> satisfies the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="predicate">the <see cref="Func{T,Boolean}"/> used to test each <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <typeparam name="TKey">the type of the <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the type of the <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a new <see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<TKey, TVal> WhereValues<TKey, TVal>(this IDictionary<TKey, TVal> source, Func<TVal, bool> predicate) {
        return source.Where(it => predicate(it.Value)).ToDictionary();
    }

    #endregion

    /// <summary>
    /// Lazily performs an <see cref="Action"/> on each item on <paramref name="source"/> <b>when it is actually enumerated</b>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="action">some code performed on each entry</param>
    /// <typeparam name="T">the type of entries in the source</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> containing the same elements as <paramref name="source"/></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> Peek<T>([NoEnumeration] this IEnumerable<T> source, Action<T> action) {
        return source.Select(
            it => {
                action.Invoke(it);
                return it;
            }
        );
    }

    public static IEnumerable<T> WriteLine<T>([NoEnumeration] this IEnumerable<T> source) => source.Peek(it => Console.WriteLine(it));

    /// <summary>
    /// Lazy tests a <paramref name="predicate"/> against each item in <paramref name="source"/> <b>when it is actually enumerated</b>, and throws an exception if the <paramref name="predicate"/> returns false.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="exceptionSupplier"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> Must<T>([ItemNotNull] this IEnumerable<T> source, Func<T, bool> predicate, Func<T, Exception>? exceptionSupplier = default) {
        return source.Peek(
            it => {
                if (predicate.Invoke(it) == false) {
                    throw exceptionSupplier?.Invoke(it) ?? new ArgumentException($"{source.GetType().Prettify()} entry {it.Prettify()} did not satisfy the predicate {predicate.Prettify()}!");
                }
            }
        );
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> containing <paramref name="self"/>.
    ///
    /// 📝 NOTE: If <paramref name="self"/> is null, <see cref="Enumerable.Empty{TResult}"/> is returned instead.
    /// </summary>
    /// <remarks>
    /// This is essentially an extension method version of <see cref="Enumerable.Repeat{TResult}"/>.
    /// </remarks>
    /// <param name="self">the original item</param>
    /// <param name="repetitions">the number of times to <see cref="Enumerable.Repeat{TResult}"/> <paramref name="self"/></param>
    /// <typeparam name="T">the type of <paramref name="self"/></typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> containing <paramref name="self"/>, if <paramref name="self"/> is not null; otherwise, <see cref="Enumerable.Empty{TResult}"/></returns>
    [Pure]
    [ItemNotNull]
    public static IEnumerable<T> WrapInEnumerable<T>(this T? self, [NonNegativeValue] int repetitions = 1) {
        return Enumerable.Repeat(self!, (self != null).ToInt());
    }

    #region As{x}

    [Pure] public static IList<T> AsList<T>(this  IEnumerable<T> source) => source as IList<T> ?? source.ToList();
    [Pure] public static T[]      AsArray<T>(this IEnumerable<T> source) => source as T[]      ?? source.ToArray();

    #endregion

    /// <summary>
    /// <i>(Efficiently?)</i> Returns the <see cref="Enumerable.Single{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> element of the source
    /// "safely", returning <c>null</c> if there are 0 or 2+ elements.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the type of the individual elements</typeparam>
    /// <returns>the single <typeparamref name="T"/>, if there is exactly one; otherwise <c>null</c></returns>
    /// <remarks>
    /// This should only enumerate at most 2 entries of the <paramref name="source"/>.
    /// </remarks>
    public static T? FindSingle<T>(this IEnumerable<T> source) {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext()) {
            return default;
        }

        return !enumerator.MoveNext() ? enumerator.Current : default;
    }
}