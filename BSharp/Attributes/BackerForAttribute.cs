using System;
using System.Reflection;

using FowlFever.BSharp.Reflection;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class BackerForAttribute : BrandonAttribute {
        public string BackedPropertyName       { get; }
        public Type?  BackedPropertyOwningType { get; }

        public BackerForAttribute(string backedPropertyName) => BackedPropertyName = backedPropertyName;

        public BackerForAttribute(Type backedPropertyOwningType, string backedPropertyName) {
            BackedPropertyName       = backedPropertyName;
            BackedPropertyOwningType = backedPropertyOwningType;
        }

        public PropertyInfo GetBackedProperty(VariableInfo annotatedVariable) {
            var owningType = BackedPropertyOwningType ?? annotatedVariable.MustGetReflectedType();
            return owningType.GetRuntimeProperty(BackedPropertyName);
        }
    }
}