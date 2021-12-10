using System;

using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        Func<string>? Heading { get; }

        int Indent { get; set; }

        PrettificationSettings PrettificationSettings { get; }
    }
}