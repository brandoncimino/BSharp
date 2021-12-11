using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Json;
using FowlFever.Testing;

using JetBrains.Annotations;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = FowlFever.Testing.Is;

namespace BrandonUtils.Tests.Standalone.Collections {
    public class FallbackTests {
        [SetUp]
        public void SetJsonSettings() {
            JsonConvert.DefaultSettings = MySettings;
        }

        private JsonSerializerSettings MySettings() {
            return new JsonSerializerSettings() {
                TraceWriter = new ConsoleTraceWriter(this) {
                    LevelFilter = TraceLevel.Verbose
                }
            };
        }

        [Test]
        public void EmptyFallbackSerializesEmpty() {
            var fallback = new Fallback<int>();
            var json     = JsonConvert.SerializeObject(fallback);
            Console.WriteLine(json);
            Assert.That(json, Is.EqualTo(@"{""FallbackValue"":0,""ExplicitValue"":[]}"));
        }

        [Test]
        public void FallbackExplicitlySetToDefaultSerializesProperly() {
            var fallback = new Fallback<int>().Set(default);
            var json     = JsonConvert.SerializeObject(fallback);
            Console.WriteLine(json);
            Assert.That(json, Is.EqualTo($@"{{""FallbackValue"":{default(int)},""ExplicitValue"":[{default(int)}]}}"));
        }

        [Test]
        public void Serialize_SimpleFallback() {
            var fallback = new Fallback<string>("FB").Set("EXP");
            Asserter.Against(() => JsonConvert.SerializeObject(fallback))
                    .And(Contains.Substring(fallback.ExplicitValue.Value))
                    .And(Contains.Substring(fallback.FallbackValue))
                    .Invoke();
        }

        [Test]
        public void Serialize_NestedFallback() {
            var parent = new Fallback<Fallback<string>>(new Fallback<string>("👸"));
            var child  = new Fallback<string>("🥚").Set("🐣");
            parent.Set(child);

            var serializedJson = JsonConvert.SerializeObject(parent);
            Console.WriteLine(serializedJson);
            Asserter.Against(serializedJson)
                    .And(Is.EqualTo($@"{{""FallbackValue"":{{""FallbackValue"":""👸"",""ExplicitValue"":[]}},""ExplicitValue"":[{{""FallbackValue"":""🥚"",""ExplicitValue"":[""🐣""]}}]}}"))
                    .Invoke();

            var deserialized = JsonConvert.DeserializeObject<Fallback<Fallback<string>>>(serializedJson);
            Asserter.Against(deserialized)
                    .AndComparingFallbacks(deserialized,       parent)
                    .AndComparingFallbacks(deserialized.Value, child)
                    .Invoke();
        }

        [Test]
        public void ReSerializeFallback() {
            var fallback = new Fallback<int>(5);

            var json     = JsonConvert.SerializeObject(fallback);
            var fromJson = JsonConvert.DeserializeObject<Fallback<int>>(json);
            Console.WriteLine(fromJson);
        }

        [Test]
        public void DeserializeFallback() {
            const string fb  = "FB";
            const string exp = "EXP";
            var json = $@"{{
  ""FallbackValue"": ""{
      fb
  }"",
  ""ExplicitValue"": [
    ""{
        exp
    }""
  ]
}}";

            var expected     = new Fallback<string>(fb).Set(exp);
            var deserialized = JsonConvert.DeserializeObject<Fallback<string>>(json);
            Asserter.Against(deserialized)
                    .And(Has.Property(nameof(deserialized.Value)).EqualTo(exp))
                    .And(Has.Property(nameof(deserialized.ExplicitValue)).EqualTo(exp))
                    .And(Has.Property(nameof(deserialized.FallbackValue)).EqualTo(fb))
                    .AndComparingFallbacks(deserialized, expected)
                    .Invoke();
        }
    }

    public class FallbackComparer : IEqualityComparer {
        private static readonly PropertyInfo ValueProperty    = typeof(Fallback<>).GetProperty(nameof(Fallback<object>.Value));
        private static readonly PropertyInfo FallbackProperty = typeof(Fallback<>).GetProperty(nameof(Fallback<object>.FallbackValue));
        private static readonly PropertyInfo ExplicitProperty = typeof(Fallback<>).GetProperty(nameof(Fallback<object>.ExplicitValue));

        [ContractAnnotation("x:null, y:null => true")]
        [ContractAnnotation("x:null, y:notnull => false")]
        [ContractAnnotation("x:notnull, y:null => false")]
        public new bool Equals(object? x, object? y) {
            var xt = x?.GetType();
            var yt = y?.GetType();

            if (xt == null || yt == null || IsFallbackType(xt) == false || IsFallbackType(yt) == false) {
                return object.Equals(xt, yt);
            }

            var valEq = Equals(ValueProperty?.GetValue(x),    ValueProperty?.GetValue(y));
            var fbEq  = Equals(FallbackProperty?.GetValue(x), FallbackProperty?.GetValue(y));
            var exEq  = Equals(ExplicitProperty?.GetValue(x), ExplicitProperty?.GetValue(y));

            return valEq && fbEq && exEq;
        }

        private static bool IsFallbackType(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Fallback<>);
        }

        public int GetHashCode(object obj) {
            throw new NotImplementedException();
        }
    }
}