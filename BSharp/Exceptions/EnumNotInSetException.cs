using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions {
    [Obsolete("Please use " + nameof(EnumNotInSetException) + " instead")]
    public class EnumNotInSubsetException<T> : EnumNotInSetException<T> where T : Enum {
        public EnumNotInSubsetException(ICollection<T> superset, IEnumerable<T> expectedValues = null, string messagePrefix = null, Exception innerException = null) : base(superset, expectedValues, messagePrefix, innerException) { }
        public EnumNotInSubsetException(ICollection<T> superset, T              invalidValue,          string messagePrefix = null, Exception innerException = null) : base(superset, invalidValue, messagePrefix, innerException) { }
    }

    public class EnumNotInSetException : InvalidEnumArgumentException {
        public EnumNotInSetException(
            IEnumerable<object> superset,
            IEnumerable<object> expectedValues,
            string?             messagePrefix  = default,
            Exception?          innerException = default
        )
            : base(BuildMessage(superset, expectedValues, messagePrefix), innerException) { }


        private static string BuildMessage(
            IEnumerable<object>  superset,
            IEnumerable<object>? valuesThatShouldBeThere,
            string?              messagePrefix
        ) {
            var prettySettings = PrettificationSettings.Default with { TypeLabelStyle = TypeNameStyle.Full };

            var badValues = valuesThatShouldBeThere?.Except(superset);

            return new Dictionary<object, object>() {
                    [superset.GetType().PrettifyType(prettySettings)] = superset,
                    [nameof(valuesThatShouldBeThere)]                 = valuesThatShouldBeThere,
                    ["Disallowed values"]                             = badValues
                }.Prettify(prettySettings)
                 .SplitLines()
                 .PrependNonNull(messagePrefix)
                 .JoinLines();
        }
    }

    [PublicAPI]
    public class EnumNotInSetException<T> : InvalidEnumArgumentException {
        private         string _baseMessage;
        public override string Message       => Lists.Of(MessagePrefix, "", _baseMessage).NonNull().JoinLines();
        public virtual  string MessagePrefix { get; }

        /// <inheritdoc cref="InvalidEnumArgumentException"/>
        /// <summary>
        /// Constructs a new <see cref="EnumNotInSetException{T}"/>, listing information about the invalid <paramref name="expectedValues"/> and the <paramref name="superset"/>.
        /// </summary>
        /// <param name="expectedValues">A collection of <see cref="T"/> values, where <b>at least one</b> (but not necessarily all) is <b>not</b> in <paramref name="superset"/>.
        /// <br/>
        /// Only unique, invalid items from <paramref name="expectedValues"/> will be included in the logging message.
        /// </param>
        /// <param name="superset">The set of valid <see cref="T"/> values.</param>
        /// <param name="messagePrefix">A user-provided message, which will be <b>prepended</b> to the built-in message.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this, if any.</param>
        public EnumNotInSetException(
            IEnumerable<T>  superset,
            IEnumerable<T>? expectedValues,
            string          messagePrefix  = null,
            Exception       innerException = null
        ) : base(messagePrefix, innerException) {
            _baseMessage  = BuildMessage(superset, expectedValues);
            MessagePrefix = messagePrefix;
        }

        public EnumNotInSetException(
            IEnumerable<T> superset,
            T              invalidValue,
            string         messagePrefix  = null,
            Exception      innerException = null
        ) : this(superset, Enumerable.Repeat(invalidValue, 1), messagePrefix, innerException) { }

        private string BuildMessage(IEnumerable<T> superset, IEnumerable<T>? valuesThatShouldBeThere) {
            var prettySettings = PrettificationSettings.Default with { TypeLabelStyle = TypeNameStyle.Full };

            var badValues = valuesThatShouldBeThere?.Except(superset);

            return new Dictionary<object, object?>() {
                [superset.GetType().PrettifyType(prettySettings)] = this,
                [nameof(valuesThatShouldBeThere)]                 = valuesThatShouldBeThere,
                ["Disallowed values"]                             = badValues
            }.Prettify(prettySettings);
        }
    }
}