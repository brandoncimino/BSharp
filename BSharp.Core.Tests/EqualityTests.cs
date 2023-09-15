using System.Numerics;

namespace FowlFever.BSharp.Core.Tests;

public class EqualityTests {
    [Test]
    public void Equality_ByComparison() {
        var          stringComparer = StringComparer.OrdinalIgnoreCase;
        const string x              = "a";
        const string y              = "A";
        x.AssertNotEquals(y);
        stringComparer.Compare(x, y).AssertEquals(0);

        Assert.Multiple(
            () => {
                var equality = Equality.ByComparison(stringComparer);
                equality.Equals(x, y).AssertEquals(true);
                equality.GetHashCode(x).AssertEquals(x.GetHashCode());
                equality.GetHashCode(y).AssertEquals(y.GetHashCode());
            }
        );
    }

    [Test]
    public void Equality_ByReference() {
        const string og   = "og";
        string       diff = new string(og);
        Assert.That(diff, Is.Not.SameAs(og));

        Assert.Multiple(
            () => {
                var refComparer = Equality.ByReference<string>();
                refComparer.AssertSame(Equality.ByReference<string>());
                refComparer.Equals(og,   diff).AssertEquals(false);
                refComparer.Equals(diff, og).AssertEquals(false);
                refComparer.Equals(og,   og).AssertEquals(true);
            }
        );
    }

    [Test]
    public void Equality_ByEqualsMethod() {
        Assert.That(Equality.ByEqualsMethod<string>(), Is.SameAs(EqualityComparer<string>.Default));
    }

    [Test]
    public void Equality_OnResultOf() {
        const string a = "yolo";
        const string b = "swag";
        const string z = "!";

        Func<string, int> transformation = static s => s.Length;
        a.AssertNotEquals(b);
        transformation(a)
            .AssertEquals(transformation(b))
            .AssertNotEquals(transformation(z));

        Assert.Multiple(
            () => {
                var equality = Equality.OnResultOf(transformation);
                equality.Equals(a, b).AssertEquals(true);
                equality.Equals(b, a).AssertEquals(true);
                equality.Equals(a, z).AssertEquals(false);
                equality.Equals(z, a).AssertEquals(false);
            }
        );
    }

    private sealed class OnlyOneStyle : IEqualityOperators<OnlyOneStyle, OnlyOneStyle, bool> {
        public enum Style {
            EqualsMethod, EqualsOperator, NotEqualsOperator,
        }

        private readonly Style  MyStyle;
        private readonly string Value;

        public OnlyOneStyle(string value, Style style) {
            MyStyle = style;
            Value   = value;
        }

        public override bool Equals(object? obj) {
            if (MyStyle != Style.EqualsMethod) {
                Assert.Fail($"{this} only supports equality via {MyStyle}!");
            }

            var other = obj as OnlyOneStyle ?? throw new ArgumentException(nameof(obj));

            return Equals(Value, other.Value);
        }

        public static bool operator ==(OnlyOneStyle? x, OnlyOneStyle? y) {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);

            if (x.MyStyle != Style.EqualsOperator) {
                Assert.Fail($"{x} only supports equality via {x.MyStyle}!");
            }

            return Equals(x.Value, y.Value);
        }

        public static bool operator !=(OnlyOneStyle? x, OnlyOneStyle? y) {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);

            if (x.MyStyle != Style.NotEqualsOperator) {
                Assert.Fail($"{x} only supports equality via {x.MyStyle}!");
            }

            return !Equals(x.Value, y.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }

    [Test]
    public void Equality_ByEqualsOperator() {
        var a1 = new OnlyOneStyle("a", OnlyOneStyle.Style.EqualsOperator);
        var a2 = new OnlyOneStyle("a", OnlyOneStyle.Style.EqualsOperator);
        var b1 = new OnlyOneStyle("b", OnlyOneStyle.Style.EqualsOperator);

        var equality = Equality.ByEqualsOperator<OnlyOneStyle>();
        Assert.Multiple(
            () => {
                equality.AssertSame(Equality.ByEqualsOperator<OnlyOneStyle>());
                equality.Equals(a1, a2).AssertEquals(true);
                equality.Equals(a2, a1).AssertEquals(true);
                equality.Equals(a1, b1).AssertEquals(false);
                equality.Equals(b1, a1).AssertEquals(false);
            }
        );
    }

    [Test]
    public void Equality_ByNotEqualsOperator() {
        var a1 = new OnlyOneStyle("a", OnlyOneStyle.Style.NotEqualsOperator);
        var a2 = new OnlyOneStyle("a", OnlyOneStyle.Style.NotEqualsOperator);
        var b1 = new OnlyOneStyle("b", OnlyOneStyle.Style.NotEqualsOperator);

        var equality = Equality.ByNotEqualsOperator<OnlyOneStyle>();
        Assert.Multiple(
            () => {
                equality.AssertSame(Equality.ByNotEqualsOperator<OnlyOneStyle>());
                equality.Equals(a1, a2).AssertEquals(true);
                equality.Equals(a2, a1).AssertEquals(true);
                equality.Equals(a1, b1).AssertEquals(false);
                equality.Equals(b1, a1).AssertEquals(false);
            }
        );
    }
}