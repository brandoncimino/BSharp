using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// A type-safe wrapper around an <see cref="System.Exception"/>.<see cref="System.Exception.Data"/> entry.
/// </summary>
/// <remarks>
/// An instance of <see cref="ExceptionDataProperty{T}"/> can be used as an alternative to <see cref="ExceptionDataPropertyExtensions.GetData{T}"/> / <see cref="ExceptionDataPropertyExtensions.SetData{T}"/>.
/// </remarks>
/// <param name="Exception">the owning <see cref="System.Exception"/></param>
/// <param name="Key">the name of the <see cref="System.Exception.Data"/> entry that stores this value</param>
/// <typeparam name="T">the type of the value being stored</typeparam>
public record ExceptionDataProperty<T>(Exception Exception, string Key) {
    /// <summary>
    /// The underlying <see cref="System.Exception.Data"/> entry's <see cref="DictionaryEntry.Value"/>, i.e. <see cref="Exception"/>.<see cref="System.Exception.Data"/>[<see cref="Key"/>].
    /// </summary>
    /// <remarks>
    /// Any changes to the <see cref="Value"/> property will write-through the <see cref="Exception"/>.<see cref="System.Exception.Data"/>[<see cref="Key"/>].
    /// </remarks>
    /// <seealso cref="ExceptionDataPropertyExtensions.GetData{T}"/>
    /// <seealso cref="ExceptionDataPropertyExtensions.SetData{T}"/>
    public T? Value {
        get => Get();
        set => Set(value);
    }

    /// <summary>
    /// Retrieves the underlying <see cref="System.Exception.Data"/> entry's <see cref="DictionaryEntry.Value"/> as an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <returns><inheritdoc cref="ExceptionDataPropertyExtensions.GetData{T}"/></returns>
    /// <seealso cref="ExceptionDataPropertyExtensions.GetData{T}"/>
    /// <exception cref="InvalidCastException">if <see cref="Key"/>'s <see cref="DictionaryEntry.Value"/> is present but cannot be cast to <typeparamref name="T"/></exception>
    public T? Get() => Exception.GetData<T>(Key);

    /// <summary>
    /// Sets the underlying <see cref="Exception"/>.<see cref="System.Exception.Data"/> entry's <see cref="DictionaryEntry.Value"/> to <paramref name="value"/>. 
    /// </summary>
    /// <remarks>
    /// Equivalent to <see cref="set_Value"/>.
    /// </remarks>
    /// <param name="value">the new <see cref="DictionaryEntry.Value"/></param>
    public void Set(T? value) => Exception.SetData(value, Key);
}

public static class ExceptionDataPropertyExtensions {
    /// <summary>
    /// Retrieves the <see cref="Exception"/>.<see cref="Exception.Data"/> entry with the <see cref="DictionaryEntry.Key"/> <paramref name="key"/> as an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="key"/> parameter will be automatically set via <see cref="CallerMemberNameAttribute"/>,
    /// allowing you to use <see cref="GetData{T}"/> and <see cref="SetData{T}"/> together as a pseudo-auto-property:
    /// <code><![CDATA[
    /// public class CustomException : Exception {
    ///     public string? Recourse { 
    ///         get => this.GetData<string>();  // [CallerMemberName] will set the `key` parameters to `Recourse`
    ///         set => this.SetData(value);     // 
    ///     } 
    /// }
    /// ]]></code>
    /// Alternatively, you can use <see cref="ExceptionDataProperty{T}"/>, which groups <see cref="GetData{T}"/> and <see cref="SetData{T}"/> together into an <i>actual</i> property, <see cref="ExceptionDataProperty{T}.Value"/>.
    /// <code><![CDATA[
    /// public class CustomException : Exception {
    ///     public CustomException(
    /// }
    /// ]]></code>
    /// </remarks>
    /// <param name="exception">this <see cref="Exception"/></param>
    /// <param name="key">the <see cref="Exception.Data"/> entry's <see cref="DictionaryEntry.Key"/>.
    /// <br/>Set automatically via <see cref="CallerMemberNameAttribute"/>.</param>
    /// <typeparam name="T">the type of the underlying value</typeparam>
    /// <returns>
    /// the <see cref="DictionaryEntry.Value"/> for <paramref name="key"/> <i>(as <typeparamref name="T"/>)</i>, if present and non-<c>null</c>;
    /// <br/>otherwise, the <c>default</c>(<typeparamref name="T"/>) value
    /// </returns>
    /// <exception cref="ArgumentNullException">if <paramref name="key"/> is <c>null</c></exception>
    /// <exception cref="InvalidCastException">if <see cref="Exception.Data"/> contains an entry for <paramref name="key"/> that can't be cast to <typeparamref name="T"/></exception>
    public static T? GetData<T>(this Exception exception, [CallerMemberName] string key = default!) {
        var value = exception.Data[key];

        // 📎 `IDictionary[object]` returns `null` if the `key` isn't found; so no need for an extra `Contains()` check
        return value switch {
            null => default,
            T t  => t,
            _    => throw new InvalidCastException($"The {exception.GetType().Name}.{nameof(exception.Data)}[{key}] entry is a {value.GetType().Name}, which can't be cast to the required type of {typeof(T).Name}!")
        };
    }

    /// <summary>
    /// Sets the <see cref="Exception"/>.<see cref="Exception.Data"/> entry the with <see cref="DictionaryEntry.Key"/> <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="GetData{T}"/></remarks>
    /// <param name="exception">this <see cref="Exception"/></param>
    /// <param name="value">the new <see cref="DictionaryEntry.Value"/></param>
    /// <param name="key">the <see cref="DictionaryEntry.Key"/> being modified</param>
    /// <typeparam name="T">the <see cref="DictionaryEntry.Value"/> type</typeparam>
    /// <exception cref="ArgumentNullException">if <paramref name="key"/> is <c>null</c></exception>
    public static void SetData<T>(this Exception exception, T value, [CallerMemberName] string key = default!) => exception.Data[key] = value;
}