using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class BackerForAttribute : BrandonAttribute {
        public string BackedPropertyName { get; }

        public BackerForAttribute(string backedPropertyName) {
            BackedPropertyName = backedPropertyName;
        }
    }
}