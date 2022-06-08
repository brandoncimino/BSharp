using System;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Prettifiers;

namespace FowlFever.BSharp.Strings {
    public class Prettifier<T> : IPrettifier<T> {
        private Func<T, PrettificationSettings, string> PrettificationFunction { get; }

        public Type PrettifierType { get; }

        Type IPrimaryKeyed<Type>.PrimaryKey => PrettifierType;

        #region Constructors

        public Prettifier(Func<T, string> prettifierFunc) : this((it, settings) => prettifierFunc.Invoke(it)) { }

        public Prettifier(Func<T, PrettificationSettings, string> prettifierFunc) {
            PrettificationFunction = prettifierFunc;
            PrettifierType         = PrettificationTypeSimplifier.SimplifyType(typeof(T), new PrettificationSettings());
        }

        #endregion

        #region Prettifier Implementation

        public string Prettify(T? cinderella, PrettificationSettings? settings = default) {
            settings = settings.Resolve();
            return cinderella == null
                       ? settings.Resolve().NullPlaceholder
                       : PrettificationFunction.Invoke(cinderella, settings);
        }

        string IPrettifier.Prettify(object? cinderella, PrettificationSettings? settings) {
            settings = settings.Resolve();

            return PrettifierType.IsGenericType
                       ? PrettifyGeneric(cinderella, settings)
                       : Prettify(TrySlipper(cinderella), settings);
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

        private T TrySlipper(object? cinderella) {
            if (cinderella is T princess) {
                return princess;
            }

            throw new InvalidCastException($"Couldn't prettify [{cinderella?.GetType().PrettifyType(default)}]{cinderella} because it wasn't the right type, {PrettifierType.PrettifyType(default)}!");
        }

        private string PrettifyGeneric(object? cinderella, PrettificationSettings settings) {
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