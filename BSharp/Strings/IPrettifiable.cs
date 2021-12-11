using System.Diagnostics.Contracts;
using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Allows an object to specifically designate a method for use by <see cref="Prettification"/>.
    /// </summary>
    public interface IPrettifiable {
        [System.Diagnostics.Contracts.Pure]
        public string Prettify(PrettificationSettings? settings = default);
    }
}