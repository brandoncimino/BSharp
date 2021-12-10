using System;
using System.Linq;

using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Json;
using BrandonUtils.Standalone.Strings.Prettifiers;

namespace BrandonUtils.Standalone.Strings {
    public class Prettifier<T> : IPrettifier<T> {
        private Func<T?, PrettificationSettings?, string> PrettificationFunction { get; }

        public Type PrettifierType { get; }

        public Type PrimaryKey => PrettifierType;

        #region Constructors

        public Prettifier(Func<T?, string> prettifierFunc) : this((it, settings) => prettifierFunc.Invoke(it)) { }

        public Prettifier(Func<T?, PrettificationSettings?, string> prettifierFunc) {
            PrettificationFunction = prettifierFunc;
            PrettifierType         = PrettificationTypeSimplifier.SimplifyType(typeof(T), new PrettificationSettings());
        }

        #endregion

        #region Prettifier Implementation

        public string Prettify(T? cinderella, PrettificationSettings? settings = default) {
            return PrettificationFunction.Invoke(cinderella, settings);
        }

        public string PrettifySafely(T? cinderella, PrettificationSettings? settings = default) {
            return PrettifySafely(cinderella as object, settings);
        }

        public string Prettify(object? cinderella, PrettificationSettings? settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            settings.TraceWriter.Verbose(() => $"⚠ DANGEROUSLY prettifying [{cinderella?.GetType().Name}]");

            if (cinderella == null) {
                return settings.NullPlaceholder.Value;
            }
            else if (PrettifierType.IsGenericType) {
                return PrettifyGeneric(cinderella, settings);
            }
            else {
                return Prettify(TrySlipper(cinderella), settings);
            }
        }

        public string PrettifySafely(object? cinderella, PrettificationSettings? settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            settings.TraceWriter.Verbose(() => $"🦺 SAFELY prettifying [{cinderella?.GetType().Name}]");

            try {
                return Prettify(cinderella, settings);
            }
            catch (Exception e) {
                var str = $"🧨 Error during prettification of [{cinderella?.GetType().Name}]{cinderella}:\n{e})";
                Console.WriteLine(str);
                return Prettification.LastResortPrettifier(cinderella, settings);
            }
        }

        public bool CanPrettify(Type cinderellaType) {
            throw new NotImplementedException("NOT YET FINISHED");
            if (PrettifierType.IsGenericType == false) {
                return PrettifierType.IsAssignableFrom(cinderellaType);
            }

            if (cinderellaType.IsConstructedGenericType) {
                cinderellaType = cinderellaType.GetGenericTypeDefinition();
            }

            var checks = new Func<bool>[] {
                () => PrettifierType == cinderellaType,
                () => PrettifierType.IsInterface && cinderellaType.Implements(PrettifierType),
                () => PrettifierType.IsClass     && cinderellaType.IsSubclassOf(PrettifierType),
                () => PrettifierType.IsAssignableFrom(cinderellaType),
                () => PrettifierType.IsAssignableFrom(cinderellaType.MakeGenericType())
            };

            return checks.Any(it => it.Invoke());
        }

        #endregion

        #region Helpers

        private T TrySlipper(object cinderella) {
            if (cinderella is T princess) {
                return princess;
            }

            throw new InvalidCastException($"Couldn't prettify [{cinderella.GetType().PrettifyType(default)}]{cinderella} because it wasn't the right type, {PrettifierType.PrettifyType(default)}!");
        }


        private string PrettifyGeneric(object? cinderella, PrettificationSettings? settings) {
            settings ??= Prettification.DefaultPrettificationSettings;

            settings.TraceWriter.Verbose(() => $"🕵️ Using generic prettification for [{cinderella?.GetType()}");

            if (cinderella?.GetType().IsGenericType != true) {
                throw new ArgumentException($"Can't use generic prettification for type [{cinderella?.GetType()}] because it isn't a generic type!");
            }

            var cinderellaTypes = cinderella.GetType().GetGenericArguments();
            var madeGeneric     = PrettificationFunction.Method.GetGenericMethodDefinition().MakeGenericMethod(cinderellaTypes);
            var result          = madeGeneric.Invoke(this, new object[] { cinderella, settings });
            return Convert.ToString(result);
        }

        #endregion


        public override string ToString() {
            return $"{GetType().Name} for [{PrettifierType.Name}] via [{PrettificationFunction.Method.Name}]";
        }
    }
}