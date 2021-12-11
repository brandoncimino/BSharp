using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// Allows an object to specifically designate a method for use by <see cref="Prettification"/>.
    /// </summary>
    public interface IPrettifiable {
        [Pure]
        public string Prettify(PrettificationSettings? settings = default);
    }
}