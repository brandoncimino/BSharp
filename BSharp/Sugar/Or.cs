namespace FowlFever.BSharp.Sugar;

public static class Or {
    public static T OrNew<T>(this T? self)
        where T : new() {
        return self ?? new T();
    }
}