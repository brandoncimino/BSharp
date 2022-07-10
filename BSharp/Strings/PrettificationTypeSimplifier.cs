using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Converts some <see cref="Type"/>s into ones that are easier for <see cref="Prettification"/> to handle.
    /// </summary>
    internal static class PrettificationTypeSimplifier {
        internal class SimplifiedType {
            public readonly  Type                    Original;
            public           Type                    Simplified => _simplified.Value;
            private readonly Lazy<Type>              _simplified;
            private readonly PrettificationSettings? Settings;

            public SimplifiedType(Type original, PrettificationSettings? settings = default) {
                Original    = original;
                _simplified = new Lazy<Type>(Simplify);
                Settings    = settings;
            }

            private Type Simplify() {
                return SimplifyType(Original, Settings);
            }

            public override string ToString() {
                return $"{Original.Name} 🤏 {Simplified.Name}";
            }
        }

        /// <summary>
        /// When we encounter one of these <see cref="IDictionary{TKey,TValue}.Keys"/>, we return the corresponding value and <b>stop simplifying</b>.
        /// </summary>
        private static readonly ReadOnlyDictionary<Type, Type> SimplestTypes = new ReadOnlyDictionary<Type, Type>(
            new Dictionary<Type, Type>() {
                [typeof(IReadOnlyDictionary<,>)] = typeof(IDictionary),
                [typeof(IDictionary<,>)]         = typeof(IDictionary),
                [typeof(Enum)]                   = typeof(Enum),
                [typeof(Type)]                   = typeof(Type),
                // ReSharper disable once PossibleMistakenCallToGetType.2
                [typeof(Type).GetType()] = typeof(Type),
                [typeof(IPrettifiable)]  = typeof(IPrettifiable),
                [typeof(object)]         = typeof(object),
            }
        );

        /// <summary>
        /// Converts <see cref="Type"/>s into ones that are easier for <see cref="Prettification"/> to handle.
        /// </summary>
        /// <param name="type">the original <see cref="Type"/></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Pure]
        internal static Type SimplifyType(Type type, PrettificationSettings? settings) {
            settings = Prettification.ResolveSettings(settings);
            return SimplifyTypeSwitch(type, settings);
        }

        private static Type SimplifyTypeSwitch(Type type, PrettificationSettings settings, int recurCount = 0) {
            if (SimplestTypes.ContainsKey(type)) {
                var simplest = SimplestTypes[type];
                settings.TraceWriter.Verbose(() => $"🦠 Could not simplify {type.Name} past {simplest.Name}!", recurCount + 2);
                return simplest;
            }

            var simplified = type switch {
                { IsEnum: true }                                => typeof(Enum),
                { IsPrimitive: true }                           => typeof(object),
                { } when type.Implements(typeof(IPrettifiable)) => typeof(IPrettifiable),
                { IsConstructedGenericType: true }              => type.GetGenericTypeDefinition(),
                _                                               => type,
            };

            if (simplified == type) {
                settings.TraceWriter.Verbose(() => $"🦠 {type.Name} simplified to the same type {simplified.Name}; returning fully simplified {simplified.Name}", recurCount + 2);
                return simplified;
            }
            else if (recurCount > 30) {
                throw new BrandonException($"REACHED RECUR LIMIT: {nameof(simplified)}: {simplified} != {nameof(type)}: {type}!");
            }
            else {
                settings.TraceWriter.Verbose(() => $"🤏 original {type.Name} simplified to {simplified.Name}; recurring...", recurCount + 2);
                return SimplifyTypeSwitch(simplified, settings, ++recurCount);
            }
        }
    }
}