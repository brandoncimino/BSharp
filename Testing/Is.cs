using System;

using FowlFever.BSharp.Enums;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// This is an "extension" of NUnit's <see cref="NUnit.Framework.Is"/> entry point for <see cref="Constraint"/>s.
    /// </summary>
    /// <remarks>
    /// I'm not 100% convinced about this yet, but it <i>is</i> what the official <a href="https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html#custom-constraint-usage-syntax">NUnit documentation</a> says to do.
    /// </remarks>
    public abstract class Is : NUnit.Framework.Is {
        public static ApproximationConstraint Approximately(object expectedValue, object threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return new ApproximationConstraint(expectedValue, threshold, clusivity);
        }

        public static ApproximationConstraint CloseTo(object expectedValue, object threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return Approximately(expectedValue, threshold, clusivity);
        }

        public static ApproximationConstraint Approximately(object expectedValue) {
            return new ApproximationConstraint(expectedValue);
        }

        public static ApproximationConstraint CloseTo(object expectedValue) {
            return Approximately(expectedValue);
        }

        public static ApproximationConstraint Approximately(DateTime expectedValue, TimeSpan threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return new ApproximationConstraint(expectedValue, threshold);
        }

        public static ApproximationConstraint CloseTo(DateTime expectedValue, TimeSpan threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return Approximately(expectedValue, threshold, clusivity);
        }
    }
}