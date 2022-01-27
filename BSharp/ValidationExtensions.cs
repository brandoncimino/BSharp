using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp {
    [Obsolete("Use Must instead")]
    public static class ValidationExtensions {
        [ContractAnnotation("null => stop")]
        public static T MustNotBeNull<T>(this T? obj) {
            return obj ?? throw new ArgumentNullException(nameof(obj), $"Parameter of type {typeof(T).Prettify()} must not be null!");
        }

        public static void ValidateMultiple<T>(T toValidate, IEnumerable<Action<T>> checks) {
            var failures = checks.Select(it => it!.Try(toValidate)).Where(it => it.Failed).ToArray();
            if (failures.Any()) {
                var failStr = failures.JoinLines();
                var msg     = $"The {typeof(T).Prettify()} [{toValidate}] was not valid: ";

                if (failures.Length > 1) {
                    msg += "\n";
                }

                msg += failStr;

                throw new ArgumentException(msg);
            }
        }

        public static void ValidateMultiple<T>(T toValidate, params Action<T>[] checks) {
            ValidateMultiple(toValidate, checks.AsEnumerable());
        }
    }
}