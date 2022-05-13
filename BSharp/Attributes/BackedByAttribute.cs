using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BackedByAttribute : BrandonAttribute {
        public string BackerName { get; }

        public BackedByAttribute(string backerName) {
            BackerName = backerName;
        }
    }
}