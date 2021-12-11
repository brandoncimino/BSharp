using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// A simple implementation of <see cref="KeyedCollection{TKey,TItem}"/>.
    /// </summary>
    /// <remarks>
    /// <p>
    /// <b>NOTE:
    /// <br/>
    /// You <i>cannot</i> properly deserialize a <see cref="KeyedList{TKey,TValue}"/>!
    /// If you need to support serialization, use a <see cref="PrimaryKeyedList{TKey,TValue}"/> instead!
    /// </b>
    /// </p>
    /// <p/>
    /// Though a <see cref="KeyedList{TKey,TValue}"/> resembles an <see cref="Dictionary{TKey,TValue}"/> in many ways, it does <b>not</b>
    /// implement <see cref="IDictionary{TKey,TValue}"/>.
    /// <br/>
    /// This is intentional, as doing so would introduce ambiguity when using <see cref="Enumerable"/> extension methods.
    /// <br/>
    /// For example, calling <see cref="Enumerable.ToList{TSource}"/> on a <see cref="KeyedCollection{TKey,TValue}"/> returns a <see cref="List{T}"/>
    /// of <typeparamref name="TValue"/>, but calling it on an <see cref="IDictionary{TKey,TValue}"/> returns a <see cref="List{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </remarks>
    /// <typeparam name="TKey">the type of the <see cref="Keys"/></typeparam>
    /// <typeparam name="TValue">the type of the <see cref="Values"/></typeparam>
    [PublicAPI]
    public class KeyedList<TKey, TValue> : KeyedCollection<TKey, TValue> {
        protected readonly Func<TValue, TKey> KeyExtractor;

        protected override TKey GetKeyForItem(TValue item) {
            return KeyExtractor.Invoke(item);
        }

        public void Add(TKey key, TValue value) {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key) {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value) {
            throw new NotImplementedException();
        }

        public new TValue this[TKey key] {
            get => base[key];
            set => Put(value);
        }
        public ICollection<TKey>   Keys   { get; }
        public ICollection<TValue> Values { get; }

        public KeyedList(Func<TValue, TKey> keyExtractor, IEnumerable<TValue> collection) : this(keyExtractor) {
            foreach (TValue item in collection) {
                this.Add(item);
            }
        }

        public KeyedList(Func<TValue, TKey> keyExtractor) : base() {
            KeyExtractor = keyExtractor;
        }

        public KeyedList<TKey, TValue> Copy() {
            return new KeyedList<TKey, TValue>(KeyExtractor);
        }

        public void ForEach(Action<TValue> action) {
            this.ToList().ForEach(action);
        }

        public TValue Grab(TKey key) {
            var found = this[key];
            Remove(key);
            return found;
        }

        /// <summary>
        /// If the <see cref="GetKeyForItem">primary key</see> of <paramref name="item"/> already exists, <b>replace</b> it.
        /// <br/>
        /// If not, <see cref="Collection{T}.Add"/> it.
        /// </summary>
        /// <param name="item">the item to <i>maybe</i> <see cref="Collection{T}.Add"/> to this <see cref="KeyedList{TKey,TValue}"/></param>
        /// <returns>the old item with the primary key matching <paramref name="item"/> if it existed; otherwise, <see langword="null"/>.</returns>
        public Optional<TValue> Put(TValue item) {
            Optional<TValue> oldItem = default;
            if (Contains(GetKeyForItem(item))) {
                oldItem = this[GetKeyForItem(item)];
            }

            Remove(GetKeyForItem(item));
            Add(item);

            return oldItem;
        }

        /// <summary>
        /// Method called with the object is de-serialized, e.g. via <see cref="JsonConvert.DeserializeObject{T}(string)"/>.
        /// </summary>
        /// <param name="streamingContext"></param>
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext streamingContext) {
            Clear();
        }

        /// <summary>
        /// A parameterless overload for <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/> that uses <see cref="GetKeyForItem"/> as the selector.
        /// </summary>
        /// <remarks>
        /// <see cref="KeyedList{TKey,TValue}"/> <b>intentionally</b> does not implement the <see cref="IDictionary{TKey,TValue}"/> interface,
        /// because doing so would create ambiguity when calling <see cref="Enumerable"/> extension methods.
        /// <p/>
        /// For example, when using <see cref="Enumerable.ToList{TSource}"/>:
        /// <ul>
        /// <li><see cref="KeyedList{TKey,TValue}"/> returns a <see cref="List{T}"/> of <typeparamref name="TValue"/></li>
        /// <li><see cref="IDictionary{TKey,TValue}"/> returns a <see cref="List{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/></li>
        /// </ul>
        /// </remarks>
        /// <returns>A <i>new</i> <see cref="Dictionary{TKey,TValue}"/> where:
        /// <li>The <see cref="Dictionary{TKey,TValue}.Values"/> are the <see cref="KeyedList{TKey,TValue}"/>'s <see cref="Values"/></li>
        /// <li>The <see cref="Dictionary{TKey,TValue}.Keys"/> are the <see cref="KeyedList{TKey,TValue}"/>'s <see cref="Keys"/></li>
        /// </returns>
        /// <seealso cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>
        public Dictionary<TKey, TValue> ToDictionary() {
            return this.ToDictionary(GetKeyForItem);
        }
    }
}