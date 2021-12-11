using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

using FowlFever.BSharp.Strings;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Optional {
    /// <summary>
    /// A custom <see cref="JsonConverter"/> for <see cref="Optional{T}"/>.
    /// </summary>
    public class OptionalJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            serializer.TraceWriter?.Trace(TraceLevel.Info, $"[{GetType().Prettify()}] serializing {value}", default);
            var enumerable = value as IEnumerable;
            /*
             * We create a new JsonSerializer here so that it doesn't get upset about self-referencing loops when we try to serialize
             * this optional as an IEnumerable.
             *
             * Somehow, even though the new serializer doesn't contain the settings from the old one,
             * the writer contains the original's settings as well, and those seem to be used...
             *
             * The call to .Cast<object>().ToList() is necessary because that's what _actually_ prevents this from being an infinite loop,
             * because it returns a NEW list (for some reason, just .Cast<object>() wasn't enough)
             */
            JsonSerializer.Create().Serialize(writer, enumerable?.Cast<object>().ToList());
        }


        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var elementType = objectType.GetGenericArguments()[0];
            var arType      = elementType.MakeArrayType();
            var asIList     = serializer.Deserialize(reader, arType) as IList ?? throw new NullReferenceException($"Got a null value when deserializing {objectType.Prettify()} to the intermediate type {arType.Prettify()} as {nameof(IList)}");
            return asIList.Count == 0 ? Activator.CreateInstance(objectType) : Activator.CreateInstance(objectType, asIList[0]);
        }

        public override bool CanConvert(Type objectType) {
            return objectType.IsGenericType
                   && typeof(Optional<>).IsAssignableFrom(objectType.GetGenericTypeDefinition());
        }
    }
}