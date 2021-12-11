using System;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// Essentially creates an <see cref="IPrimaryKeyed{T}"/> out of <see cref="Value"/>, where the <see cref="IPrimaryKeyed{T}.PrimaryKey"/> is determined by <see cref="Func{T,TResult}.Invoke"/>-ing <see cref="KeySelector"/>.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Keyed<TValue, TKey> : IPrimaryKeyed<TKey> {
        public TValue             Value;
        public Func<TValue, TKey> KeySelector;
        public TKey               PrimaryKey => KeySelector.Invoke(Value);

        public Keyed(TValue value, Func<TValue, TKey> keySelector) {
            this.Value       = value;
            this.KeySelector = keySelector;
        }

        public static Keyed<TValue, TKey> Of(TValue value, Func<TValue, TKey> keySelector) {
            return new Keyed<TValue, TKey>(value, keySelector);
        }
    }
}