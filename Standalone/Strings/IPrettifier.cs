using System;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Strings {
    public interface IPrettifier : IPrimaryKeyed<Type> {
        /// <value>the <see cref="Type"/> that this <see cref="IPrettifier{T}"/> can <see cref="Prettify"/>.</value>

        Type PrettifierType { get; }

        bool CanPrettify(Type type);

        /// <summary>
        /// Returns a pretty <see cref="string"/> representation of <paramref name="cinderella"/> <b>IF</b> <paramref name="cinderella"/> is of the <see cref="PrettifierType"/>.
        ///
        /// If <paramref name="cinderella"/> is null, returns <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="cinderella">the <see cref="object"/> to <see cref="Prettify"/></param>
        /// <param name="settings">an optional <see cref="PrettificationSettings"/> instance</param>
        /// <returns>a pretty <see cref="string"/> representation of <paramref name="cinderella"/></returns>
        /// <exception cref="InvalidCastException">if <paramref name="cinderella"/> is not of the <see cref="PrettifierType"/></exception>
        /// <seealso cref="PrettifySafely"/>
        string Prettify(object? cinderella, PrettificationSettings? settings = default);

        /**
         * <summary>Attempts to <see cref="Prettify"/> <paramref name="cinderella"/>, falling back to <see cref="Convert.ToString(object)"/> if an <see cref="Exception"/> occurs.</summary>
         * <inheritdoc cref="Prettify"/>
         */
        string PrettifySafely(object? cinderella, PrettificationSettings? settings = default);
    }

    /**
     * <inheritdoc cref="IPrettifier"/>
     */
    public interface IPrettifier<in T> : IPrettifier {
        /**
         * <inheritdoc cref="IPrettifier.Prettify"/>
         */
        string Prettify(T? cinderella, PrettificationSettings? settings = default);

        /**
         * <inheritdoc cref="IPrettifier.PrettifySafely"/>
         */
        string PrettifySafely(T? cinderella, PrettificationSettings? settings = default);
    }
}