using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// A specialized <see cref="ISet{T}"/> that contains <see cref="Enum"/> values.
    ///
    /// TODO: Remove <c>struct</c> constraint
    /// TODO: Extend <see cref="IImmutableSet{T}"/>
    /// </summary>
    /// <typeparam name="T">the <see cref="Enum"/> type of the set's members</typeparam>
    public interface IEnumSet<T> : ISet<T>
        where T : struct, Enum {
        public IEnumSet<T> Inverse();
    }
}