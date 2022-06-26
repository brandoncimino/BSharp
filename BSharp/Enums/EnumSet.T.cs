using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// A special type of <see cref="HashSet{T}"/> specifically meant for use with <see cref="Enum"/>s.
    ///
    /// TODO: Update to extend (or be based on) <see cref="System.Collections.Immutable.ImmutableHashSet{T}"/> and implement <see cref="IImmutableSet{T}"/>.
    /// TODO: Experiment with using a <c>struct</c> for this, which would give us access to a <c>default</c> value.
    ///     This would really only make sense if the <c>default</c> value wasn't an empty <see cref="EnumSet{T}(T[])"/>,
    ///     since we can already write that cleanly as <c>new()</c>. It seems more <b><i>practical</i></b> to me to make the <c>default</c>
    ///     as <see cref="EnumSet.OfAllValues{T}"/> - but would that be more <b><i>logical</i></b>?
    ///
    ///     - In programming, the default value for <see cref="HashSet{T}"><![CDATA[HashSet<int>]]></see> is going to be an empty set
    ///     - In math, the default value for <a href="https://en.wikipedia.org/wiki/Natural_number">ℕ</a> <i>(natural numbers, aka integers)</i>
    ///
    ///     CONCLUSION: Everything, I think. If it makes more sense, I can name it "Enum Filter", but I think that "EnumSet" is fine.
    /// </summary>
    /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
    [PublicAPI]
    public class EnumSet<T> : HashSet<T>, ICollection<T>
        where T : struct, Enum {
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