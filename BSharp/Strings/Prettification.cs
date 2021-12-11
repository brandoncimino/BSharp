using System;

using FowlFever.BSharp.Strings.Json;

using JetBrains.Annotations;

using System.Diagnostics.Contracts;

using FowlFever.BSharp.Optional;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Strings {
    [PublicAPI]
    public static class Prettification {
        internal const           string              DefaultNullPlaceholder = "⛔";
        internal static readonly IPrettifierDatabase Prettifiers            = PrettifierDatabase.GetDefaultPrettifiers();


        internal static readonly OptionalPrettifierFinder[] Finders = {
            PrettifierFinders.FindExactPrettifier,
            PrettifierFinders.FindToStringOverridePrettifier,
            PrettifierFinders.FindGenericallyTypedPrettifier,
            PrettifierFinders.FindInheritedPrettifier
        };


        [Obsolete]
        public static PrettificationSettings DefaultPrettificationSettings {
            get => PrettificationSettings.DefaultSettings;
            [CanBeNull] set => PrettificationSettings.DefaultSettings = value;
        }


        public static PrettificationSettings ResolveSettings(PrettificationSettings? settings) {
            return settings ?? PrettificationSettings.DefaultSettings;
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

        [Pure]
        public static string Prettify(this object? cinderella) {
            return cinderella.Prettify(default);
        }

        [Pure]
        public static string Prettify(this object? cinderella, PrettificationSettings? settings) {
            settings = ResolveSettings(settings);

            settings.TraceWriter.Info(() => $"👸 Prettifying [{cinderella?.GetType().Name}]");

            if (cinderella == null) {
                return settings.NullPlaceholder.Value ?? "";
            }

            var prettifier = PrettifierFinders.FindPrettifier(
                Prettifiers,
                cinderella.GetType(),
                settings,
                Finders
            );

            return prettifier
                .IfPresentOrElse(
                    it => it.PrettifySafely(cinderella, settings),
                    () => LastResortPrettifier(cinderella, settings)
                );
        }


        internal static string LastResortPrettifier(object? cinderella, PrettificationSettings? settings) {
            settings ??= new PrettificationSettings();

            settings.TraceWriter.Verbose(() => $"⛑ Using the LAST RESORT prettifier for [{cinderella?.GetType()}]: {nameof(Convert.ToString)}!");

            return Convert.ToString(cinderella);
        }
    }
}