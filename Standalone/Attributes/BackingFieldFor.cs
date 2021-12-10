using System;
using System.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    [PublicAPI]
    public class BackingFieldForAttribute : BrandonAttribute {
        public string BackedPropertyName { get; }

        public PropertyInfo? BackedProperty { get; }

        public BackingFieldForAttribute(string backedPropertyName) {
            BackedPropertyName = backedPropertyName;
        }
    }
}