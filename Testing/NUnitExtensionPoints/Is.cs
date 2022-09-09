using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Enums;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing.NUnitExtensionPoints;

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

    /// <summary>
    /// Tests that a <paramref name="collection"/> contains the actual value.
    /// </summary>
    /// <param name="collection">the possible values</param>
    /// <returns>a new <see cref="AnyOfConstraint"/></returns>
    public static AnyOfConstraint In(IEnumerable collection) {
        return AnyOf(collection.Cast<object>().ToArray());
    }

    /// <inheritdoc cref="Does.Match(Regex)"/>
    public static RegexConstraint Match(Regex pattern) => Does.Match(pattern);

    /// <inheritdoc cref="Does.Match(string)"/>
    public static RegexConstraint Match(string pattern) => Does.Match(pattern);
}