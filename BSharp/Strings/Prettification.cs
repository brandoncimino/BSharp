using System;
using System.Collections.Immutable;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// TODO: Implement some kind of "caching", with some kind of stateful "PrettyCache" object so we don't have to re-prettify the same object multiple times within a single .Prettify() call
    /// </summary>
    [PublicAPI]
    public static class Prettification {
        internal const           string              DefaultNullPlaceholder = "⛔";
        internal static readonly IPrettifierDatabase Prettifiers            = PrettifierDatabase.GetDefaultPrettifiers();

        internal static ImmutableArray<OptionalPrettifierFinder> Finders { get; set; } = PrettifierFinders.DefaultFinders.ToImmutableArray();

        [Obsolete] public static PrettificationSettings DefaultPrettificationSettings => PrettificationSettings.Default;

        [Obsolete]
        public static PrettificationSettings ResolveSettings(PrettificationSettings? settings) {
            return settings.Resolve();
        }

        public static void RegisterPrettifier(IPrettifier prettifier) {
            Prettifiers.Register(prettifier);
        }

        public static IPrettifier? UnregisterPrettifier(Type prettifierType) {
            return Prettifiers.Deregister(prettifierType);
        }

        #region Finding Prettifiers

        #region Generics

        #endregion

        #endregion

        [Pure] public static string Prettify<T>(this T? cinderella) => cinderella.Prettify(default);

        [Obsolete($"Use the generic {nameof(Prettify)}<T> instead")]
        [Pure]
        public static string Prettify(this object? cinderella) => cinderella.Prettify(default);

        [Pure]
        public static string Prettify<T>(this T? cinderella, PrettificationSettings? settings) {
            settings = settings.Resolve();

            settings.TraceWriter.Info(() => $"👸 Prettifying [{cinderella?.GetType().Name}]");

            if (cinderella == null) {
                return settings.NullPlaceholder;
            }

            var prettifier = PrettifierFinders.FindPrettifier(
                Prettifiers,
                cinderella.GetType(),
                settings,
                Finders
            );

            return prettifier
                .IfPresentOrElse(
                    // TODO: Refactor this to prevent boxing into `object`
                    it => _Safely<object>(cinderella, it.Prettify, settings),
                    () => LastResortPrettifier(cinderella, settings)
                );
        }

        [Obsolete($"Use the generic {nameof(Prettify)}<T> instead")]
        [Pure]
        public static string Prettify(this object? cinderella, PrettificationSettings? settings) => cinderella.Prettify<object>(settings);

        private static string _Safely<T>(T? cinderella, Func<T?, PrettificationSettings?, string> prettifyFunc, PrettificationSettings? settings) {
            settings = settings.Resolve();
            try {
                return prettifyFunc(cinderella, settings);
            }
            catch (Exception e) {
                settings.TraceWriter.Error(() => $"🧨 Error during prettification of [{cinderella?.GetType().Name}]{cinderella}!", exception: e);
                return LastResortPrettifier(cinderella, settings);
            }
        }

        internal static string LastResortPrettifier(object? cinderella, PrettificationSettings? settings) {
            settings = settings.Resolve();
            settings.TraceWriter.Verbose(() => $"⛑ Using the LAST RESORT prettifier for [{cinderella?.GetType()}]: {nameof(Convert.ToString)}!");
            // 📎 The documentation for `Convert.ToString()` describes is as returning "" if the input is `null`, but I don't think that's actually correct...
            return Convert.ToString(cinderella) ?? DefaultNullPlaceholder;
        }
    }
}