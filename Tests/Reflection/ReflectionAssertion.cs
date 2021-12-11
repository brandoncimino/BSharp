using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Reflection {
    internal static class Validate {
        public static void HasAutoProperty(this object testClass, string propertyName) {
            IsAutoProperty(testClass.GetType().GetProperty(propertyName, ReflectionUtils.VariablesBindingFlags));
        }

        public static void IsAutoProperty(this PropertyInfo propertyInfo) {
            AssertAll.Of(
                () => Assert.That(propertyInfo.BackingField(),                               Is.Not.Null),
                () => Assert.That(propertyInfo.BackingField()?.IsAutoPropertyBackingField(), Is.True),
                () => BacksProperty(propertyInfo.BackingField(), propertyInfo)
            );
        }

        private static void IsBackedBy(this PropertyInfo? propertyInfo, FieldInfo? expectedBackingField) {
            AssertAll.Of(
                $"{propertyInfo.Prettify()} should have the backing field {expectedBackingField.Prettify()}",
                () => Assert.That(propertyInfo?.BackingField(), Is.Not.Null.And.EqualTo(expectedBackingField)),
                () => BacksProperty(expectedBackingField, propertyInfo)
            );
        }

        public static void AllHaveBackingField(this IEnumerable<PropertyInfo?>? propertyInfos, FieldInfo? expectedBackingField) {
            Asserter
                .WithHeading($"All of the {nameof(propertyInfos)} should be backed by {expectedBackingField.Prettify()}")
                .And(propertyInfos?.Select<PropertyInfo, Action>(it => () => it.IsBackedBy(expectedBackingField)))
                .Invoke();
        }

        private static void BacksProperty(this FieldInfo? backingField, params PropertyInfo?[]? expectedBackedProperties) {
            Assert.That(backingField?.BackedProperties(), Is.Not.Null.And.SupersetOf(expectedBackedProperties));
        }
    }
}