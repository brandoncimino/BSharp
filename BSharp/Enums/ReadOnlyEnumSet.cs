using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Enums {
    [Obsolete(
        $"This class was ill-conceived and never finished. It should be replaced with an implementation of {nameof(EnumSet<T>)} that is based on <see" +
        $"{nameof(ImmutableHashSet<T>)}"
    )]
    public class ReadOnlyEnumSet<T> : ReadOnlySet<T>, IEnumSet<T>
        where T : struct, Enum {
        public ReadOnlyEnumSet(ISet<T>     realSet) : base(realSet) { }
        public ReadOnlyEnumSet(IEnumSet<T> realSet) : base(realSet) { }

        public IEnumSet<T> Inverse() {
            throw new NotImplementedException();
        }
    }
}