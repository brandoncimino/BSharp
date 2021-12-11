using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Strings.Json {
    /// <summary>
    /// Indicates that it is safe to <b>deep clone</b> this object using <see cref="JsonConvert"/> serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// This interface indicates that cloning via Json serialization is <b>generally safe</b>, but it does <b>not</b> provide any specific implementation of that serialization.
    /// <p/>
    /// In most cases, the extension method <see cref="JsonCloneableExtensions.JsonClone{T}"/> should be sufficient.
    /// <p/>
    /// TODO: If ever I get access to "default implementations", move <see cref="JsonCloneableExtensions.JsonClone{T}"/> into the <see cref="IJsonCloneable"/> interface.
    /// </remarks>
    public interface IJsonCloneable { }

    public static class JsonCloneableExtensions {
        /// <summary>
        /// Creates a <b>deep clone</b> of this <typeparamref name="T"/> instance by <see cref="JsonConvert.SerializeObject(object?)">serializing</see> and then <see cref="JsonConvert.DeserializeObject{T}(string)">deserializing</see> it.
        /// </summary>
        /// <remarks>
        /// This method uses the default <see cref="JsonConverter{T}"/>s and optional <see cref="JsonSerializerSettings"/>.
        /// </remarks>
        /// <param name="original">this <typeparamref name="T"/> instance</param>
        /// <param name="settings">optional <see cref="JsonSerializerSettings"/></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>a <b>new</b> <typeparamref name="T"/> instance that is a <b>deep clone</b> of <paramref name="original"/></returns>
        [ContractAnnotation("original:null => null")]
        [ContractAnnotation("original:notnull => notnull")]
        public static T? JsonClone<T>(this T? original, JsonSerializerSettings? settings = default) where T : IJsonCloneable {
            var json = JsonConvert.SerializeObject(original, settings!);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Creates a <see cref="JsonClone{T}(T,Newtonsoft.Json.JsonSerializerSettings)"/> of this <typeparamref name="T"/>, while also performing some arbitrary <paramref name="modifications"/> to the new clone.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="modifications"></param>
        /// <param name="settings"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [ContractAnnotation("original:null => null")]
        [ContractAnnotation("original:notnull => notnull")]
        public static T? JsonClone<T>(this T? original, Action<T>? modifications, JsonSerializerSettings? settings = default) where T : IJsonCloneable {
            var clone = original.JsonClone(settings);
            modifications?.Invoke(clone);
            return clone;
        }
    }
}