using System;

namespace FowlFever.BSharp.Sugar;

public static class Lazily {
    public static Lazy<T> Get<T>(Func<T> supplier) => new(supplier);
}