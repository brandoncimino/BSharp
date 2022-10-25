using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FowlFever.BSharp;

public readonly partial record struct ValueArray<T> where T : struct, IEquatable<T> {
    #region Insert

    public ValueArray<T> Insert(int index, T item) => AsImmutableArray.Insert(index, item);

    public ValueArray<T> Insert(int index, ValueArray<T> items) {
        return items.IsEmpty ? this :
               IsEmpty       ? items :
                               AsImmutableArray.InsertRange(index, items.AsImmutableArray);
    }

    public ValueArray<T> Insert(int index, ImmutableArray<T> items) {
        return items.IsDefaultOrEmpty ? this : AsImmutableArray.InsertRange(index, items);
    }

    public ValueArray<T> Insert(int index, IEnumerable<T> items) {
        return items switch {
            ValueArray<T> valueArray => Insert(index, valueArray),
            _                        => Insert(index, items.ToImmutableArray())
        };
    }

    #endregion

    #region Append

    public ValueArray<T> Append(T                 item)  => Insert(Length, item);
    public ValueArray<T> Append(ImmutableArray<T> items) => Insert(Length, items);
    public ValueArray<T> Append(ValueArray<T>     items) => Insert(Length, items);

    public static ValueArray<T> operator +(ValueArray<T> a, ValueArray<T> b) => a.Append(b);

    #endregion

    #region Prepend

    public ValueArray<T> Prepend(T                 item)  => Insert(0, item);
    public ValueArray<T> Prepend(ImmutableArray<T> items) => Insert(0, items);
    public ValueArray<T> Prepend(ValueArray<T>     items) => Insert(0, items);

    #endregion
}