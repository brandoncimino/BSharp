using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Randomization {
    public class FromList<T> : Randomized<T> {
        public FromList(ICollection<T> source) : base(source.Random) { }

        public static implicit operator T(FromList<T> self) {
            return self.Value;
        }
    }
}