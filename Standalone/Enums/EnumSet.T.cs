using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
    /// <summary>
    /// A special type of <see cref="HashSet{T}"/> specifically meant for use with <see cref="Enum"/>s.
    /// </summary>
    /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
    [PublicAPI]
    public class EnumSet<T> : HashSet<T>, ICollection<T> where T : struct, Enum {
        #region Constructors

        public EnumSet(params T[] enumValues) : base(enumValues) { }

        public EnumSet(params IEnumerable<T>[] setsToBeUnionized) : base(setsToBeUnionized.SelectMany(it => it)) { }

        #region Inherited Constructors

        public EnumSet() { }
        public EnumSet(IEnumerable<T>       collection) : base(collection) { }
        protected EnumSet(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion Inherited Constructors

        #endregion Constructors

        #region MustContain

        /// <summary>
        /// Throws an <see cref="EnumNotInSetException{T}"/> if this doesn't contain <b>all</b> of the <see cref="expectedValues"/>.
        ///
        /// In other words, throws an exception unless this <see cref="HashSet{T}.IsSupersetOf"/> <paramref name="expectedValues"/>.
        /// </summary>
        /// <param name="expectedValues">this invocation's must-have <typeparamref name="T"/> values</param>
        /// <exception cref="EnumNotInSetException{T}"></exception>
        public void MustContain(params T[] expectedValues) {
            if (!IsSupersetOf(expectedValues)) {
                var mustContainMessage = BuildMustContainMessage(expectedValues);
                throw new EnumNotInSetException<T>(this, expectedValues, $"The {GetType().PrettifyType(default)} didn't contain all of the {nameof(expectedValues)}!\n{mustContainMessage}");
            }
        }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(IEnumerable<T> expectedValues) {
            MustContain(expectedValues.ToArray());
        }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(IEnumerable<T> expectedValues, Func<Exception> exceptionProvider) {
            if (!IsSupersetOf(expectedValues)) {
                throw exceptionProvider.Invoke();
            }
        }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(IEnumerable<T> expectedValues, Func<EnumNotInSetException<T>, Exception> exceptionTransformer) {
            try {
                MustContain(expectedValues);
            }
            catch (EnumNotInSetException<T> e) {
                throw exceptionTransformer.Invoke(e);
            }
        }

        // /**
        //  * <inheritdoc cref="MustContain(T[])"/>
        //  */
        //
        // public EnumSet<T> MustContain(T expectedValue) {
        //     if (!Contains(expectedValue)) {
        //         throw new EnumNotInSetException<T>(this, expectedValue);
        //     }
        //
        //     return this;
        // }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(T expectedValue, Func<Exception> exceptionProvider) {
            if (!Contains(expectedValue)) {
                throw exceptionProvider.Invoke();
            }
        }

        /**
         * <inheritdoc cref="MustContain(T[])"/>
         */
        public void MustContain(T expectedValue, Func<EnumNotInSetException<T>, Exception> exceptionTransformer) {
            try {
                MustContain(expectedValue);
            }
            catch (EnumNotInSetException<T> e) {
                throw exceptionTransformer.Invoke(e);
            }
        }

        private string BuildMustContainMessage(T[] valuesThatShouldBeThere) {
            PrettificationSettings prettySettings = TypeNameStyle.Full;

            var badValues = valuesThatShouldBeThere.Except(this);
            var mapStuff = new Dictionary<object, object>() {
                [GetType().PrettifyType(prettySettings)] = this,
                [nameof(valuesThatShouldBeThere)]        = valuesThatShouldBeThere,
                ["Disallowed values"]                    = badValues
            };
            return mapStuff.Prettify(prettySettings);
        }

        #endregion

        /// <summary>
        /// Creates a <b>new</b> <see cref="EnumSet{T}"/> containing
        /// </summary>
        /// <returns></returns>
        public EnumSet<T> Copy() {
            return new EnumSet<T>(this);
        }

        public ReadOnlyEnumSet<T> AsReadOnly() {
            throw new NotImplementedException();
        }

        public bool ShouldBeReadOnly;

        bool ICollection<T>.IsReadOnly => ShouldBeReadOnly;

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException("can't add, ok?");
        }
    }
}