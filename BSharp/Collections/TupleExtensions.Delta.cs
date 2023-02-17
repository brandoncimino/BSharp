using System;

using FowlFever.BSharp.Attributes;

namespace FowlFever.BSharp.Collections;

public static partial class TupleExtensions {
    #region Delta

    /// <summary>
    /// Calculates the change from <see cref="(T, T2).Item1">before</see> to <see cref="(T, T2).Item2">after</see>.
    /// </summary>
    /// <remarks>
    /// In target frameworks where <a href="https://learn.microsoft.com/en-us/dotnet/standard/generics/math">generic math</a> is supported (.NET 6+), this method
    /// is constrained to the <see cref="T:System.Numerics.ISubtractionOperators`3"/> interface, and uses the <see cref="M:System.Numerics.ISubtractionOperators`3.op_Subtraction(`0,`1)"/>.
    /// <p/>
    /// In previous versions, it is instead constrained to <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters#unmanaged-constraint">unmanaged types</a>.
    /// </remarks>
    /// <param name="tuple"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="ValueTuple{T1,T2}.Item2">after</see> ➖ <see cref="ValueTuple{T1,T2}.Item1">before</see></returns>
    [Experimental("Having this conditionally compile the generic type parameter seems...wonky")]
    public static T Delta<T>(this (T before, T after) tuple)
        where T :
#if NET6_0_OR_GREATER
        System.Numerics.ISubtractionOperators<T, T, T> {
        return tuple.after - tuple.before;
    }
#else
        unmanaged {
        return FowlFever.BSharp.Memory.PrimitiveMath.Subtract(tuple.after, tuple.before);
    }
#endif

    /// <summary>
    /// Gets the <see cref="IComparable{T}.CompareTo"/> sign from <see cref="ValueTuple{T,T2}.Item1"/> to <see cref="ValueTuple{T,T2}.Item2"/>.
    /// </summary>
    /// <param name="tuple">two <see cref="IComparable{T}"/> values</param>
    /// <typeparam name="T">an <see cref="IComparable{T}"/> type</typeparam>
    /// <returns>the <see cref="IComparable{T}.CompareTo"/> sign from <see cref="ValueTuple{T,T2}.Item1"/> to <see cref="ValueTuple{T,T2}.Item2"/></returns>
    public static int Direction<T>(this (T before, T after) tuple) where T : IComparable<T> {
        return tuple.before.CompareTo(tuple.after);
    }

    #endregion
}