using System;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings {
    public interface IPrettifier : IPrimaryKeyed<Type> {
        /// <value>the <see cref="Type"/> that this <see cref="IPrettifier{T}"/> can <see cref="Prettify"/>.</value>
        Type PrettifierType { get; }
        Type IPrimaryKeyed<Type>.PrimaryKey => PrettifierType;

        bool CanPrettify(Type type) => PrettifierType.IsAssignableFrom(type);

        /// <inheritdoc cref="Prettify{T}"/>
        [Obsolete("Use the generic Prettify<T> instead")]
        string Prettify(object? cinderella, PrettificationSettings? settings = default) => Prettify<object>(cinderella, settings);

        /// <summary>
        /// Returns a pretty <see cref="string"/> representation of <paramref name="cinderella"/> <b>IF</b> <paramref name="cinderella"/> is of the <see cref="PrettifierType"/>.
        ///
        /// If <paramref name="cinderella"/> is null, returns <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="cinderella">the <see cref="object"/> to <see cref="Prettify"/></param>
        /// <param name="settings">an optional <see cref="PrettificationSettings"/> instance</param>
        /// <returns>a pretty <see cref="string"/> representation of <paramref name="cinderella"/></returns>
        /// <exception cref="InvalidCastException">if <paramref name="cinderella"/> is not of the <see cref="PrettifierType"/></exception>
        string Prettify<T>(T? cinderella, PrettificationSettings? settings = default);
    }

    /**
     * <inheritdoc cref="IPrettifier"/>
     */
    public interface IPrettifier<in T> : IPrettifier {
        /**
         * <inheritdoc cref="IPrettifier.Prettify"/>
         */
        string Prettify(T? cinderella, PrettificationSettings? settings = default);

        Type IPrettifier.PrettifierType => typeof(T);

        string IPrettifier.Prettify<T1>(T1? cinderella, PrettificationSettings? settings)
            where T1 : default {
            return cinderella switch {
                null => settings.Resolve().NullPlaceholder,
                T t  => Prettify(t, settings),
                _    => throw Reject.UnhandledSwitchType(cinderella),
            };
        }
    }
}