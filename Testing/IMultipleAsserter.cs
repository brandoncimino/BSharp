using System;

using FowlFever.BSharp.Strings;

namespace BrandonUtils.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        Func<string>? Heading { get; }

        int Indent { get; set; }

        PrettificationSettings PrettificationSettings { get; }
    }
}