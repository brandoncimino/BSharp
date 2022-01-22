using System;

using FowlFever.BSharp.Strings;

namespace FowlFever.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        Func<string>? Heading { get; }

        /// <remarks>
        /// The <c>protected</c> modifier, in an interface, does <b>NOT</b> allow <b>implementing classes</b> to access
        /// that member - it only allows <b>child interfaces</b> to access it!
        /// </remarks>
        int Indent { get; }

        PrettificationSettings? PrettificationSettings { get; }
    }
}