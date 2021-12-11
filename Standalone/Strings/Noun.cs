using System;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// A class for handling noun conjugations consistently.
    /// </summary>
    [Obsolete("Please use FowlFever.Conjugal.Plurable instead.")]
    [Serializable]
    public class Noun {
        public string Singular;
        public string Plural;

        public Noun(string singular, string plural = null) {
            this.Singular = singular;
            this.Plural   = plural ?? Singular + "s";
        }

        public string Get(int quantity = 1) {
            return quantity == 1 ? Singular : Plural;
        }
    }
}