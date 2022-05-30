using System;

namespace FowlFever.BSharp.Sugar;

public static class Lazily {
    public static Lazy<T> Get<T>(Func<T> supplier) => new(supplier);

    public static Lazy<T> Get<T>(T value) => new(() => value);

    public static Lazy<TLazy> Get<TFIn, TLazy, TFOut>(Func<TFIn, TFOut> supplier, TFIn input)
        where TFOut : TLazy
        => new(() => supplier(input));
}