using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// A wrapper around <see cref="HashSet{T}"/> that only allows read operations.
    ///</summary>
    [PublicAPI]
    public class ReadOnlySet<T> : ISet<T>, IReadOnlyCollection<T> {
        private readonly ISet<T> RealSet;

        public ReadOnlySet() : this(new HashSet<T>()) { }

        public ReadOnlySet(ISet<T> original) {
            RealSet = original ?? throw new ArgumentNullException(nameof(original));
        }

        private static NotSupportedException ReadOnlyException() {
            return new NotSupportedException("Collection is read-only.");
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return RealSet.GetEnumerator();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)RealSet).GetEnumerator();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ICollection<T>.Add(T item) {
            throw ReadOnlyException();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ISet<T>.ExceptWith(IEnumerable<T> other) {
            throw ReadOnlyException();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ISet<T>.IntersectWith(IEnumerable<T> other) {
            throw ReadOnlyException();
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool IsProperSubsetOf([InstantHandle] IEnumerable<T> other) {
            return RealSet.IsProperSubsetOf(other);
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool IsProperSupersetOf([InstantHandle] IEnumerable<T> other) {
            return RealSet.IsProperSupersetOf(other);
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool IsSubsetOf([InstantHandle] IEnumerable<T> other) {
            return RealSet.IsSubsetOf(other);
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool IsSupersetOf([InstantHandle] IEnumerable<T> other) {
            return RealSet.IsSupersetOf(other);
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool Overlaps(IEnumerable<T> other) {
            return RealSet.Overlaps(other);
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool SetEquals(IEnumerable<T> other) {
            return RealSet.SetEquals(other);
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ISet<T>.SymmetricExceptWith([NoEnumeration] IEnumerable<T> other) {
            throw ReadOnlyException();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ISet<T>.UnionWith([NoEnumeration] IEnumerable<T> other) {
            throw ReadOnlyException();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        bool ISet<T>.Add(T item) {
            throw ReadOnlyException();
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        void ICollection<T>.Clear() {
            throw ReadOnlyException();
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        bool ICollection<T>.Contains(T item) {
            return RealSet.Contains(item);
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public void CopyTo(T[] array, int arrayIndex) {
            RealSet.CopyTo(array, arrayIndex);
        }

        [ContractAnnotation("=> stop")]
        [CollectionAccess(CollectionAccessType.None)]
        bool ICollection<T>.Remove(T item) {
            throw ReadOnlyException();
        }

        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public int Count => RealSet.Count;

        [Pure]
        [CollectionAccess(CollectionAccessType.None)]
        bool ICollection<T>.IsReadOnly => true;
    }
}