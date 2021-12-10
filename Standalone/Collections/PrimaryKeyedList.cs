using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Collections {
    /// <summary>
    /// A version of <see cref="KeyedList{TKey,TValue}"/> that specifically contains <see cref="IPrimaryKeyed{T}"/> entries,
    /// using their <see cref="IPrimaryKeyed{T}.PrimaryKey"/> as the <see cref="KeyedList{TKey,TValue}.KeyExtractor"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [PublicAPI]
    [Serializable]
    public class PrimaryKeyedList<TKey, TValue> : KeyedList<TKey, TValue> where TValue : IPrimaryKeyed<TKey> {
        public PrimaryKeyedList(IEnumerable<TValue> collection) : base(it => it.PrimaryKey, collection) { }
        public PrimaryKeyedList() : base(it => it.PrimaryKey) { }
    }
}