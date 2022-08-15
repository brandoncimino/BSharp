using System;
using System.Linq;
using System.Threading;

using FowlFever.BSharp.Attributes;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Sugar;

public static class Lazily {
    public static LazyHas<T> Get<T>(Func<T> supplier) => new(supplier);

    public static LazyHas<T> Get<T>(T value) => new(value);

    public static LazyHas<TLazy> Get<TFIn, TLazy, TFOut>(Func<TFIn, TFOut> supplier, TFIn input)
        where TFOut : TLazy => new(() => supplier(input));

    /// <summary>
    /// A method to return <see cref="Lazy{T}.Value"/> to help play nice with <see cref="Func{TResult}"/>s, such as inside of <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,int,TResult})"/>
    /// </summary>
    /// <param name="lazy">this <see cref="Lazy{T}"/></param>
    /// <typeparam name="T">the type of the underlying <see cref="Lazy{T}.Value"/></typeparam>
    /// <returns>the <see cref="Lazy{T}.Value"/></returns>
    public static T Get<T>(this Lazy<T> lazy) => lazy.Value;

    public static LazyHas<T> AsHas<T>(this Lazy<T> lazy) => lazy.IsValueCreated ? Get(lazy.Value) : Get(lazy.Get);

    public static T? OrDefault<T>(this Lazy<T>? lazy) => lazy == null ? default : lazy.Value;

    /// <summary>
    /// "Chains" operations together lazily by applying a <see cref="Func{T,TResult}"/> to the <see cref="Lazy{T}.Value"/> of <paramref name="lazy"/>.
    /// </summary>
    /// <param name="lazy">the original <see cref="Lazy{T}"/></param>
    /// <param name="transformation">applied to the <see cref="Lazy{T}.Value"/></param>
    /// <typeparam name="T">the original <see cref="Lazy{T}"/> type</typeparam>
    /// <typeparam name="T2">the transformed <see cref="Lazy{T}"/> type</typeparam>
    /// <returns>a new <see cref="Lazy{T}"/></returns>
    public static Lazy<T2> AndThen<T, T2>(this Lazy<T> lazy, Func<T, T2> transformation) {
        return new Lazy<T2>(() => transformation(lazy.Value));
    }

    /// <summary>
    /// Returns the <see cref="Lazy{T}.Value"/> of the <paramref name="fallback"/> if this <see cref="Lazy{T}"/> returns <c>null</c>.
    /// </summary>
    /// <param name="lazy">this <see cref="Lazy{T}"/></param>
    /// <param name="fallback">the result if this <see cref="Lazy{T}"/> returns <c>null</c></param>
    /// <typeparam name="T">the <see cref="Lazy{T}.Value"/> type</typeparam>
    /// <returns>a new <see cref="Lazy{T}"/></returns>
    public static Lazy<T> Or<T>(this Lazy<T?> lazy, LazyShim<T> fallback) {
        var lz = fallback.Lazy;
        return new Lazy<T>(() => lazy.Value ?? lz.Value);
    }
}

/// <inheritdoc cref="Lazy{T}"/>
/// <remarks>
/// Applies the <see cref="IHas{T}"/> interface to the <see cref="Lazy{T}"/> object.
/// </remarks>
[Experimental]
public class LazyHas<T> : Lazy<T>, IHas<T> {
    public LazyHas() { }

    public LazyHas(bool isThreadSafe) : base(isThreadSafe) { }

    public LazyHas(Func<T> valueFactory) : base(valueFactory) { }

    public LazyHas(Func<T> valueFactory, bool isThreadSafe) : base(valueFactory, isThreadSafe) { }

    public LazyHas(Func<T> valueFactory, LazyThreadSafetyMode mode) : base(valueFactory, mode) { }

    public LazyHas(LazyThreadSafetyMode mode) : base(mode) { }

    public LazyHas(T value) : base(value) { }
}