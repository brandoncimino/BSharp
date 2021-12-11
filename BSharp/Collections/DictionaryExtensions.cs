using System;
using System.Collections.Generic;

namespace FowlFever.BSharp.Collections {
    public static class DictionaryExtensions {
        public static TValue? GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dic,
            TKey                           key,
            TValue?                        fallback = default
        ) {
            if (dic == null) {
                throw new ArgumentNullException(nameof(dic));
            }

            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            return dic.ContainsKey(key) ? dic[key] : fallback;
        }

        public static TValue? GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dic,
            TKey                           key,
            Func<TValue>                   fallbackSupplier
        ) {
            if (dic == null) {
                throw new ArgumentNullException(nameof(dic));
            }

            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (fallbackSupplier == null) {
                throw new ArgumentNullException(nameof(fallbackSupplier));
            }

            return dic.ContainsKey(key) ? dic[key] : fallbackSupplier.Invoke();
        }
    }
}