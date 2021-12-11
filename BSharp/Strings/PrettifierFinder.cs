using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings {
    [CanBeNull]
    public delegate Optional<IPrettifier> OptionalPrettifierFinder(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings);

    public delegate IPrettifier PrettifierFinder(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings);

    public static class PrettifierFinders {
        //  public static readonly PrettifierFinder ExactPrettifier      = (db, t, s) => db.Find(t);
        //  public static readonly PrettifierFinder FromPrettifiable     = (db, t, s) => t.Implements(typeof(IPrettifiable)) ? PrettifierDatabase.PrettifiablePrettifier : null;
        //  public static readonly PrettifierFinder FromToString         = (db, t, s) => t.GetToStringOverride() != null ? PrettifierDatabase.ToStringPrettifier : null;
        //  public static readonly PrettifierFinder ConsentingPrettifier = (db, t, s) => db.Find(it => it.CanPrettify(t));


        internal static OptionalPrettifierFinder[] GetDefaultFinders() => new OptionalPrettifierFinder[] {
            FindExactPrettifier,
            FindToStringOverridePrettifier,
            FindGenericallyTypedPrettifier,
            FindInheritedPrettifier
        };

        [System.Diagnostics.Contracts.Pure]
        internal static Optional<IPrettifier> FindPrettifier(
            IPrettifierDatabase     prettifierDatabase,
            Type                    type,
            PrettificationSettings? settings,
            [InstantHandle]
            IEnumerable<OptionalPrettifierFinder> finders
        ) {
            settings = Prettification.ResolveSettings(settings);
            settings.TraceWriter.Verbose(() => $"🔎 Attempting to find an {nameof(IPrettifier)} for the type {type.Name}");
            var simplified = PrettificationTypeSimplifier.SimplifyType(type, settings);

            var prettifier = Optional.Optional.FirstWithValue<OptionalPrettifierFinder, IPrettifier>(
                finders,
                prettifierDatabase,
                simplified,
                settings
            );

            settings.TraceWriter.Info(
                () => prettifier.IfPresentOrElse(
                    it => $"⛳ Found: {it}",
                    () => "🐸 No prettifier found!"
                ),
                2
            );

            return prettifier;
        }

        internal static Optional<IPrettifier> FindGenericallyTypedPrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindGenericallyTypedPrettifier)}({type.Name})", 1);

            return Optional.Optional.FirstWithValue<OptionalPrettifierFinder, IPrettifier>(
                new OptionalPrettifierFinder[] {
                    FindExactGenericPrettifier,
                    FindMatchingGenericPrettifier
                },
                prettifierDatabase,
                type,
                settings
            );
        }

        private static Optional<IPrettifier> FindExactGenericPrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            return type.IsGenericType == false ? default : FindExactPrettifier(prettifierDatabase, type.GetGenericTypeDefinition(), settings);
        }

        internal static Optional<IPrettifier> FindMatchingGenericPrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            IPrettifier found = type.IsGenericType ? prettifierDatabase.Find(it => GenericTypesMatch(it.PrettifierType, type)) : default;
            return Optional.Optional.OfNullable(found);
        }

        internal static bool GenericTypesMatch(Type t1, Type t2) {
            if (t1.IsGenericType == false || t2.IsGenericType == false) {
                return false;
            }

            return t1.GetGenericTypeDefinition().IsAssignableFrom(t2.GetGenericTypeDefinition());
        }

        [System.Diagnostics.Contracts.Pure]
        private static Type MakeGenericOfObjects(Type type) {
            if (!type.IsGenericType) {
                throw new ArgumentException("Must be a generic type or definition!", nameof(type));
            }

            var genArgCount = type.GetGenericArguments().Length;
            return type.GetGenericTypeDefinition().MakeGenericType(genArgCount.Repeat(typeof(object)).ToArray());
        }

        [System.Diagnostics.Contracts.Pure]
        internal static Optional<IPrettifier> FindExactPrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindExactPrettifier)}({type.Name})", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(type));
        }

        [System.Diagnostics.Contracts.Pure]
        internal static Optional<IPrettifier> FindToStringOverridePrettifier(IPrettifierDatabase? prettifierDatabase, Type type, PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindToStringOverridePrettifier)}({type.Name})", 1);
            settings.TraceWriter.Warning(() => $"{nameof(FindToStringOverridePrettifier)} is currently DISABLED!");
            return default;
        }

        internal static Optional<IPrettifier> FindInterfacePrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindInterfacePrettifier)}", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(it => it.PrettifierType.IsInterface && type.Implements(it.PrettifierType)));
        }

        [System.Diagnostics.Contracts.Pure]
        internal static Optional<IPrettifier> FindInheritedPrettifier(IPrettifierDatabase prettifierDatabase, Type type, PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindInheritedPrettifier)}({type.Name})", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(it => it.PrettifierType.IsAssignableFrom(type)));
        }
    }
}