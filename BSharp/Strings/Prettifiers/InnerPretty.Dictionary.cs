using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Tabler;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal static partial class InnerPretty {
        private static Type InferType(IEnumerable stuff) {
            try {
                return ReflectionUtils.CommonType(stuff.Cast<object>().Select(it => it.GetType()));
            }
            catch {
                return typeof(object);
            }
        }

        /// <summary>
        /// TODO: Parameterize the "column width style" and provide this as an option
        /// </summary>
        /// <param name="unlimitedKeyWidth"></param>
        /// <param name="unlimitedValWidth"></param>
        /// <param name="columnSeparator"></param>
        /// <param name="widthLimit"></param>
        /// <returns></returns>
        /// <exception cref="BrandonException"></exception>
        private static (int keyWidth, int valWidth) CalculateWidths(int unlimitedKeyWidth, int unlimitedValWidth, string columnSeparator, int widthLimit) {
            widthLimit -= columnSeparator.Length;

            if (unlimitedKeyWidth + unlimitedValWidth < widthLimit) {
                return (unlimitedKeyWidth, unlimitedValWidth);
            }

            var (keyWidth, valWidth) = Mathb.Apportion(unlimitedKeyWidth, unlimitedValWidth, widthLimit);

            // checking my work
            if (keyWidth + valWidth != widthLimit) {
                throw new BrandonException($"[{nameof(keyWidth)}] {keyWidth} + [{nameof(valWidth)}] {valWidth} != [{widthLimit}] {widthLimit}!");
            }

            return (keyWidth, valWidth);
        }

        /**
         * <remarks>I would have this operate on <see cref="KeyedCollection{TKey,TItem}"/>, but unfortunately, <see cref="KeyedCollection{TKey,TItem}.GetKeyForItem"/> is <c>protected</c>.</remarks>
         */
        internal static string PrettifyKeyedList<TKey, TValue>(KeyedList<TKey, TValue> keyedList, PrettificationSettings? settings = default) {
            return PrettifyDictionary3_Generic(keyedList.ToDictionary(), settings);
        }

        internal static string PrettifyDictionary3_Generic<TKey, TVal>(IDictionary<TKey, TVal> dictionary, PrettificationSettings? settings = default) {
            return Table.Of(dictionary).Prettify(settings);
        }

        internal static string PrettifyDictionary3(IDictionary dictionary, PrettificationSettings? settings = default) {
            return Table.Of(
                            InferType(dictionary.Keys),
                            InferType(dictionary.Values),
                            dictionary.ToGeneric()
                        )
                        .Prettify(settings);
        }
    }
}