namespace FowlFever.BSharp;

public static class WrappedExtensions {
    public static Wrapped<T> Wrap<T>(this T obj) => obj;
}