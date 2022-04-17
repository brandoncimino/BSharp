using System;

using FowlFever.BSharp.Enums;
using FowlFever.Clerical.Validated;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests.Clerical2;

public class ValidatedStringTests {
    private record ValStr(string Value) : ValidatedString(Value);

    [Test]
    public static void ValidatedString_Equality() {
        var  str = "YOLO";
        var  val = new ValStr(str);
        ValidateEquality(val, str, Should.Pass);
    }

    private static void ValidateEquality(ValStr val, string str, Should should) {
        IResolveConstraint GetEqualityConstraint(object other) => should switch {
            Should.Pass => Is.EqualTo(other),
            Should.Fail => Is.Not.EqualTo(other),
            _           => throw BEnum.UnhandledSwitch(should, nameof(should), nameof(GetEqualityConstraint))
        };

        Assert.Multiple(
            () => {
                // Assert.That(Equals(val, str), should.Constrain(),   "Equals(val, str)");
                // Assert.That(Equals(str, val), should.Constrain(),   "Equals(str, val)");
                Assert.That(val == str,      should.Constrain(),           "val == str");
                Assert.That(val != str,      should.Inverse().Constrain(), "val != str");
                Assert.That(str == val,      should.Constrain(),           "str == val");
                Assert.That(str != val,      should.Inverse().Constrain(), "str != val");
                Assert.That(val.Equals(str), should.Constrain(),           "val.Equals(str)");
                Assert.That(str.Equals(val), should.Constrain(),           "str.Equals(val)");
                Assert.That(val,             GetEqualityConstraint(str));
                Assert.That(str,             GetEqualityConstraint(val));
            }
        );
    }
}