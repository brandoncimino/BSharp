using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;

using JetBrains.Annotations;

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


        private static object GetHeader(
            [ItemCanBeNull]
            IEnumerable values,
            Type?                  valueType,
            string                 fallback,
            PrettificationSettings settings
        ) {
            return settings.HeaderStyle.Value switch {
                HeaderStyle.None      => fallback,
                HeaderStyle.TypeNames => valueType ?? InferType(values),
                _                     => throw BEnum.InvalidEnumArgumentException(nameof(settings.HeaderStyle), settings.HeaderStyle.Value)
            };
        }

        private static (object keyHeader, object valHeader) GetHeaders(
            IDictionary             dictionary,
            PrettificationSettings? settings
        ) {
            settings ??= Prettification.DefaultPrettificationSettings;
            return (
                       GetHeader(dictionary.Keys,   default, "Key",   settings),
                       GetHeader(dictionary.Values, default, "Value", settings)
                   );
        }

        private static (object keyHeader, object valHeader) GetHeaders<TKey, TVal>(
            IDictionary<TKey, TVal> dictionary,
            PrettificationSettings? settings
        ) {
            settings ??= Prettification.DefaultPrettificationSettings;
            return (
                       GetHeader(dictionary.Keys,   typeof(TKey), "Key",   settings),
                       GetHeader(dictionary.Values, typeof(TVal), "Value", settings)
                   );
        }

        /// <summary>
        /// TODO: Update this to work with an arbitrary number of columns
        /// </summary>
        /// <param name="dictionary">the <see cref="IDictionary"/> to be prettified</param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        public static string PrettifyDictionary(IDictionary dictionary, PrettificationSettings settings = default) {
            var keys = dictionary.Keys.Cast<object>().ToList();
            var vals = dictionary.Values.Cast<object>().ToList();

            return PrettifyDictionary(keys, vals, settings);
        }


        public static string PrettifyDictionary2(IDictionary dictionary, PrettificationSettings? settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            var keys = dictionary.Keys.Cast<object>();
            var vals = dictionary.Values.Cast<object>();

            var (keyHeader, valHeader) = GetHeaders(dictionary, settings);

            return PrettifyDictionary2(
                (keys, keyHeader),
                (vals, valHeader),
                settings
            );
        }

        private static string PrettifyDictionary(IList<object> keys, IList<object> vals, PrettificationSettings settings) {
            // get the types of the columns, to use as headers
            var keyType = InferType(keys);
            var valType = InferType(vals);
            return PrettifyDictionary(keys, vals, keyType, valType, settings);
        }

        private class Col {
            public object?             Header;
            public IEnumerable<object> Cells;

            public IEnumerable<string> GetLines(PrettificationSettings settings) {
                settings ??= Prettification.DefaultPrettificationSettings;
                settings =   settings.JsonClone();
                settings.PreferredLineStyle.Set(LineStyle.Single);

                var prettyCells = Cells.Select(it => it.Prettify(settings)).ToList();

                if (Header != null) {
                    var headerStr     = Header.Prettify(settings);
                    int longestCell   = prettyCells.LongestLine();
                    int longestLine   = longestCell.Max(headerStr.Length);
                    var separatorLine = (settings.TableHeaderSeparator.Value ?? " ").Fill(longestLine);
                    return new[] {
                        headerStr,
                        separatorLine
                    }.Concat(prettyCells);
                }

                return prettyCells;
            }
        }

        private class Dic {
            public readonly Col Keys = new Col();
            public readonly Col Vals = new Col();

            public IEnumerable<string> GetLines(PrettificationSettings? settings) {
                settings ??= Prettification.DefaultPrettificationSettings.JsonClone();
                // calculate the various widths
                var keyLines = Keys.GetLines(settings).ToArray();
                var valLines = Vals.GetLines(settings).ToArray();

                var (keyWidth, valWidth) = CalculateWidths(keyLines.LongestLine(), valLines.LongestLine(), settings.TableColumnSeparator, settings.LineLengthLimit);

                var limitedKeyLines = keyLines.Select(it => it.Truncate(keyWidth)).ToArray();
                var limitedValLines = valLines.Select(it => it.Truncate(valWidth)).ToArray();

                return limitedKeyLines.Select((keyLine, i) => $"{keyLine}{settings.TableColumnSeparator}{limitedValLines[i]}");
            }
        }


        [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
        private static string PrettifyDictionary2(
            (IEnumerable<object> cells, object header) keys,
            (IEnumerable<object> cells, object header) vals,
            PrettificationSettings?                    settings
        ) {
            if (keys.cells == null || keys.header == null) {
                Console.WriteLine($"🔑s were null!! {keys}");
            }

            if (vals.cells == null || vals.header == null) {
                Console.WriteLine($"📱s were null!! {vals}");
            }

            return new Dic {
                    Keys = {
                        Cells  = keys.cells,
                        Header = keys.header
                    },
                    Vals = {
                        Cells  = vals.cells,
                        Header = vals.header
                    }
                }.GetLines(settings)
                 .JoinLines();
        }

        /// <summary>
        /// The basis of the <see cref="InnerPretty"/> methods for dictionaries.
        /// </summary>
        /// <remarks>
        /// Rather than operating on an actual <see cref="IDictionary{TKey,TValue}"/>, this takes in two <see cref="List{T}"/>s and explicit column headers.
        /// </remarks>
        /// <param name="keys"></param>
        /// <param name="vals"></param>
        /// <param name="keyHeader"></param>
        /// <param name="valHeader"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="BrandonException"></exception>
        private static string PrettifyDictionary(
            IList<object?>          keys,
            IList<object?>          vals,
            Optional<object>        keyHeader,
            Optional<object>        valHeader,
            PrettificationSettings? settings
        ) {
            settings ??= Prettification.DefaultPrettificationSettings;

            // add the headers
            keys.Insert(0, keyHeader);
            vals.Insert(0, valHeader);

            var keyLines = keys.Select(it => it.Prettify(LineStyle.Single)).ToList();
            var valLines = vals.Select(it => it.Prettify(LineStyle.Single)).ToList();

            string headerSeparator = settings.TableHeaderSeparator;

            if (keyLines == null) {
                throw new NullReferenceException($"{nameof(keyLines)} is null");
            }

            if (valLines == null) {
                throw new NullReferenceException($"{nameof(valLines)} is null");
            }

            var (keyWidth, valWidth) = CalculateWidths(keyLines.LongestLine(), valLines.LongestLine(), settings.TableColumnSeparator, settings.LineLengthLimit);

            // add the separators
            keyLines.Insert(1, headerSeparator.Fill(keyWidth));
            valLines.Insert(1, headerSeparator.Fill(valWidth));

            //NOTE: There is actually a version of select that uses the index! O_O
            var lines = keyLines.Select((keyLine, i) => PrettifyPair((keyLine, keyWidth), (valLines[i], valWidth), settings))
                                .Bookend("\n")
                                .ToList();
            return lines.JoinLines();
        }

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

        private static string PrettifyPair((object value, int width) a,     (object value, int width) b,     PrettificationSettings settings) => $"{PrettifyCell(a.value, a.width, settings)}{settings.TableColumnSeparator}{PrettifyCell(b.value, b.width, settings)}";
        private static string PrettifyCell(object                    value, int                       width, PrettificationSettings settings) => value.Prettify(settings).ForceToLength(width);

        /**
         * <remarks>I would have this operate on <see cref="KeyedCollection{TKey,TItem}"/>, but unfortunately, <see cref="KeyedCollection{TKey,TItem}.GetKeyForItem"/> is <c>protected</c>.</remarks>
         */
        internal static string PrettifyKeyedList<TKey, TValue>(KeyedList<TKey, TValue> keyedList, PrettificationSettings settings = default) {
            IDictionary dictionary = keyedList.ToDictionary();
            return PrettifyDictionary(
                dictionary.Keys.Cast<object>().ToList(),
                dictionary.Values.Cast<object>().ToList(),
                typeof(TKey),
                typeof(TValue),
                settings
            );
        }

        /// <summary>
        /// The generic version of <see cref="PrettifyDictionary"/>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="settings"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static string PrettifyDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, PrettificationSettings settings) {
            return PrettifyDictionary(
                dictionary.Keys.Cast<object>().ToList(),
                dictionary.Values.Cast<object>().ToList(),
                typeof(TKey),
                typeof(TValue),
                settings
            );
        }


        internal static string PrettifyDictionary2<TKey, TValue>(IDictionary<TKey, TValue> dictionary, PrettificationSettings? settings = default) {
            return PrettifyDictionary2(
                (dictionary.Keys.Cast<object>(), typeof(TKey)),
                (dictionary.Values.Cast<object>(), typeof(TValue)),
                settings
            );
        }
    }
}