using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Allows an object to specifically designate a method for use by <see cref="Prettification"/>.
    /// </summary>
    public interface IPrettifiable {
        /// <summary>
        /// Generates a fancy <see cref="string"/> representation of this object, taking <see cref="PrettificationSettings"/> into account.
        /// </summary>
        /// <param name="settings">a set of <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        [System.Diagnostics.Contracts.Pure]
        public string Prettify(PrettificationSettings? settings = default);
    }
}