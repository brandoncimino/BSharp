using System;
using System.Collections.Generic;

namespace BrandonUtils.Standalone.Enums {
    /// <summary>
    /// A specialized <see cref="ISet{T}"/> that contains <see cref="Enum"/> values.
    /// </summary>
    /// <typeparam name="T">the <see cref="Enum"/> type of the set's members</typeparam>
    public interface IEnumSet<T> : ISet<T> where T : struct, Enum {
        public IEnumSet<T> Inverse();
    }
}