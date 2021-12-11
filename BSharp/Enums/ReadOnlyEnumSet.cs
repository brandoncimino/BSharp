using System;
using System.Collections.Generic;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Enums {
    public class ReadOnlyEnumSet<T> : ReadOnlySet<T>, IEnumSet<T> where T : struct, Enum {
        public ReadOnlyEnumSet(ISet<T>     realSet) : base(realSet) { }
        public ReadOnlyEnumSet(IEnumSet<T> realSet) : base(realSet) { }

        public IEnumSet<T> Inverse() {
            var set1 = new HashSet<int>();
            var set2 = new HashSet<DayOfWeek>();

            set2.IsEnumSet();
            set2.Inverted();

            throw new NotImplementedException();
        }
    }
}