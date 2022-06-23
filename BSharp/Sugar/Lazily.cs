using System;
using System.Linq;

namespace FowlFever.BSharp.Sugar;

public static class Lazily {
    public static Lazy<T> Get<T>(Func<T> supplier) => new(supplier);

    public static Lazy<T> Get<T>(T value) => new(() => value);

    public static Lazy<TLazy> Get<TFIn, TLazy, TFOut>(Func<TFIn, TFOut> supplier, TFIn input)
        where TFOut : TLazy
        => new(() => supplier(input));

    /// <summary>
    /// A method to return <see cref="Lazy{T}.Value"/> to help play nice with <see cref="Func{TResult}"/>s, such as inside of <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,int,TResult})"/>
    /// </summary>
    /// <param name="lazy">this <see cref="Lazy{T}"/></param>
    /// <typeparam name="T">the type of the underlying <see cref="Lazy{T}.Value"/></typeparam>
    /// <returns>the <see cref="Lazy{T}.Value"/></returns>
    public static T Get<T>(this Lazy<T> lazy) => lazy.Value;
}