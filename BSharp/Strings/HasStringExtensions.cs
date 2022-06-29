using System;

namespace FowlFever.BSharp.Strings;

public static class OneLineExtensions {
    public static OneLine IfEmpty(this OneLine  line, OneLine       fallback) => line.IsEmpty  == false ? line : fallback;
    public static OneLine IfBlank(this OneLine  line, OneLine       fallback) => line.IsBlank  == false ? line : fallback;
    public static OneLine IfEmpty(this OneLine? line, OneLine       fallback) => line?.IsEmpty == false ? line.Value : fallback;
    public static OneLine IfBlank(this OneLine? line, OneLine       fallback) => line?.IsBlank == false ? line.Value : fallback;
    public static OneLine IfEmpty(this OneLine  line, Func<OneLine> fallback) => line.IsEmpty  == false ? line : fallback.Invoke();
    public static OneLine IfBlank(this OneLine  line, Func<OneLine> fallback) => line.IsBlank  == false ? line : fallback.Invoke();
}