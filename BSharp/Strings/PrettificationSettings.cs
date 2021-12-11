using System;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings {
    public class PrettificationSettings : IJsonCloneable {
        public static PrettificationSettings DefaultSettings { get; set; } = new PrettificationSettings();

        /// <summary>
        /// When the <see cref="PreferredLineStyle"/> is <see cref="LineStyle.Dynamic"/>, <see cref="LineLengthLimit"/> is used to decide between <see cref="LineStyle.Multi"/> and <see cref="LineStyle.Single"/>.
        /// </summary>

        public Fallback<int> LineLengthLimit { get; } = new Fallback<int>(70);

        public Fallback<string> TableHeaderSeparator { get; } = new Fallback<string>("-");
        public Fallback<string> TableColumnSeparator { get; } = new Fallback<string>(" ");
        public Fallback<string> NullPlaceholder      { get; } = new Fallback<string>("⛔");

        /// <summary>
        /// The preferred <see cref="LineStyle"/>.
        /// </summary>

        public Fallback<LineStyle> PreferredLineStyle { get; } = new Fallback<LineStyle>(LineStyle.Dynamic);

        public Fallback<TypeNameStyle> TypeLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.Full);

        public Fallback<TypeNameStyle> EnumLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.None);

        public Fallback<HeaderStyle> HeaderStyle { get; } = new Fallback<HeaderStyle>(Strings.HeaderStyle.None);


        public static PrettificationSettings GetDefault(Action<PrettificationSettings>? modifications = default) {
            return DefaultSettings.JsonClone(modifications);
        }


        public static PrettificationSettings GetEmpty(Action<PrettificationSettings>? modifications = default) {
            var settings = new PrettificationSettings();
            modifications?.Invoke(settings);
            return settings;
        }


        public static implicit operator PrettificationSettings(LineStyle lineStyle) {
            return GetDefault(it => it.PreferredLineStyle.Set(lineStyle));
        }


        public static implicit operator PrettificationSettings(TypeNameStyle typeLabelStyle) {
            return GetDefault(it => it.TypeLabelStyle.Set(typeLabelStyle));
        }


        public static implicit operator PrettificationSettings(HeaderStyle headerStyle) {
            return GetDefault(it => it.HeaderStyle.Set(headerStyle));
        }

        [JsonIgnore]
        public ITraceWriter? TraceWriter { get; set; } = null;


        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}