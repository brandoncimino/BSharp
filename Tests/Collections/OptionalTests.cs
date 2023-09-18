using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.Testing;

using Newtonsoft.Json;

using NUnit.Framework;

namespace BSharp.Tests.Collections;

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public class OptionalTests {
    [Test]
    public void EmptyEqualsEmpty() {
        var a = new Optional<int>();
        var b = new Optional<int>();

        Asserter.Against(a)
                .And(Is.EqualTo(b), "a IsEqualTo b")
                .And(Is.EqualTo(new Optional<int>()))
                .And(Is.EqualTo(default(Optional<int>)))
                .And(it => it == b,      Is.True, "a == b")
                .And(it => it.Equals(b), Is.True, "a.Equals(b)")
                .Invoke();
    }

    [Test]
    public void EmptyEqualsDefault(
        [Values(typeof(int), typeof(string), typeof(object), typeof(DateTime), typeof(List<int>))]
        Type type
    ) {
        var optionalType = typeof(Optional<>);
        var genericType  = optionalType.MakeGenericType(type);
        var obj          = Activator.CreateInstance(genericType);

        Console.WriteLine(obj);
    }

    [Test]
    public void DefaultEqualsEmpty() {
        Optional<int> a = default;

        Console.WriteLine(a);

        AssertAll.Of(
            a,
            Has.Property(nameof(a.HasValue)).EqualTo(false),
            Is.EqualTo(new Optional<int>())
        );
    }

    [Test]
    public void OptionalEqualsUnboxed_String() {
        const string str = "yolo";
        var          a   = new Optional<string>(str);

        AssertAll.Of(
            () => Assert.That(a.Equals(str), "a.Equals(str)"),
            () => Assert.That(a,             Is.EqualTo(str)),
            () => Assert.That(a   == str,    "a == str"),
            () => Assert.That(str == a,      "str == a"),
            () => Assert.That(str,           Is.EqualTo(a)),
            () => Assert.That(a.Equals(str), "a.Equals(str)")
        );
    }

    [Test]
    public void OptionalEqualsUnboxed_IntLong() {
        const int  i = 5;
        const long l = 5;

        var iOpt = Optional.Of(i);
        var lOpt = Optional.Of(l);

        AssertAll.Of(
            () => Assert.That(iOpt == i, "iOpt == i"),
            // () => Assert.That(iOpt == l, "iOpt == l"),
            () => Assert.That(lOpt == i,    "lOpt == i"),
            () => Assert.That(lOpt == l,    "lOpt == l"),
            () => Assert.That(i    == iOpt, "i == iOpt"),
            () => Assert.That(i    == lOpt, "i == lOpt"),
            // () => Assert.That(l == iOpt, "l == iOpt"),
            () => Assert.That(l == lOpt, "l == lOpt")
        );
    }

    [Test]
    public void ReturnBoxed() {
        Assert.That(Boxional(5), Is.TypeOf<Optional<int>>());
    }

    [Test]
    public void ReturnUnboxed() {
        Assert.That(Unboxional(Optional.Of(5)), Is.TypeOf<int>());
    }

    [Test]
    public void PassUnboxional() {
        Assert.That(Unboxional<int>(5), Is.TypeOf<int>());
    }

    private static Optional<T> Boxional<T>(T value) {
        return value;
    }

    private static T? Unboxional<T>(Optional<T> optional) {
        return optional.GetValueOrDefault((T?)default);
    }

    #region Optional of null

    [Test]
    public void OptionalOfNull_HasValue() {
        var ofNull = new Optional<string?>(null);

        Asserter.Against(ofNull)
                .And(Has.Property(nameof(ofNull.HasValue)).True)
                .And(Has.Property(nameof(ofNull.Value)).Null)
                .Invoke();
    }

    [Test]
    public void Optional_EqualsInnerValue() {
        const int value = 5;
        var       opt   = Optional.Of(value);

        Asserter.Against(opt)
                .And(it => it    == value)
                .And(it => value == it)
                .And(it => it.Equals(value))
                .Invoke();
    }

    [Test]
    public void Optional_EqualsEquivalentOptional() {
        var val1 = new string("abc");
        var val2 = new string(val1);
        Assert.That(val1, Is.Not.SameAs(val2));

        var opt1 = Optional.Of(val1);
        var opt2 = Optional.Of(val2);
        Asserter.Against(opt1)
                .And(Is.Not.SameAs(opt2))
                .And(Is.EqualTo(opt2))
                .And(it => it == opt2)
                .Invoke();
    }

    [Test]
    public void Optional_OfNull_DoesNotEqual_Empty() {
        var ofNull = Optional.Of(default(string));
        var empty  = Optional.Empty<string>();

        Asserter.Against(ofNull)
                .And(Is.Not.EqualTo(empty))
                .And(it => it == empty,      Is.False)
                .And(it => it.Equals(empty), Is.False)
                .And(it => empty == it,      Is.False)
                .And(it => empty.Equals(it), Is.False)
                .Invoke();
    }

    [Test]
    public void Optional_OfNull_EqualsNull() {
        var           ofNull    = Optional.Of(default(string));
        const string? nullValue = default;

        Asserter.WithHeading()
                .And(ofNull    == nullValue)
                .And(nullValue == ofNull)
                .And(() => ofNull.Equals(nullValue), Is.True)
                .Invoke();
    }

    [Test]
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public void OptionalOfNull_Equality() {
        Ignore.This("This test is...a mess.");

        Optional<string?>  ofNull           = new Optional<string?>(null);
        Optional<string?>  ofNull2          = new Optional<string?>(null);
        string?            nullValue        = null;
        Optional<string>?  nullOptional     = (Optional<string>?)null;
        IOptional<string>? nullInterface    = (IOptional<string>?)null;
        Optional<string>?  defaultOptional  = default;
        IOptional<string>? defaultInterface = default;
        Optional<string>   empty            = new Optional<string>();

        Console.WriteLine($"ofNull: [{ofNull}]");
        Console.WriteLine($"nullOptional: [{nullOptional}]");
        Console.WriteLine($"defaultOptional: [{defaultOptional}]");

        AssertAll.Of(
            // vs. nullValue
            () => Assert.That(ofNull                == nullValue,             "ofNull == nullValue"),
            () => Assert.That(ofNull                != nullValue,             Is.False, "ofNull != nullValue"),
            () => Assert.That((ofNull == nullValue) == (nullValue == ofNull), "(ofNull == nullValue) == (nullValue == ofNull)"),
            () => Assert.That((ofNull != nullValue) == (nullValue != ofNull), "(ofNull != nullValue) == (nullValue != ofNull)"),
            // vs. nullOptional
            () => Assert.That(ofNull                   == nullOptional,             "ofNull == nullOptional"),
            () => Assert.That(ofNull                   != nullOptional,             Is.False, "ofNull != nullOptional"),
            () => Assert.That((ofNull == nullOptional) == (nullOptional == ofNull), "(ofNull == nullOptional) == (nullOptional == ofNull)"),
            () => Assert.That((ofNull != nullOptional) == (nullOptional != ofNull), "(ofNull != nullOptional) == (nullOptional != ofNull)"),
            // vs. nullInterface
            // 📝 No longer supported as of 11/23/2021
            // () => Assert.That(ofNull == nullInterface, Is.False, "ofNull == nullInterface"),
            // () => Assert.That(ofNull != nullInterface, "ofNull != nullInterface"),
            // operators with a left-hand IOptional aren't supported as it would cause lots of ambiguity with the other operators
            // () => Assert.That((ofNull == nullInterface) == (nullInterface == ofNull), "(ofNull == nullInterface) == (nullInterface == ofNull)"),
            // vs. ofNull2
            () => Assert.That(ofNull == ofNull2, "ofNull == ofNull2"),
            () => Assert.That(ofNull != ofNull2, Is.False, "ofNull != ofNull2"),
            // vs. defaultOptional
            () => Assert.That(ofNull == defaultOptional,                                  Is.False, "ofNull == defaultOptional"),
            () => Assert.That(ofNull != defaultOptional,                                  "ofNull != defaultOptional"),
            () => Assert.That(ofNull,                                                     Is.Not.EqualTo(defaultOptional), "ofNull, Is.Not.EqualTo(defaultOptional)"),
            () => Assert.That((ofNull == defaultOptional) == (defaultOptional == ofNull), "(ofNull == defaultOptional) == (defaultOptional == ofNull)"),
            // vs. defaultInterface
            // 📝 No longer supported as of 11/23/2021
            // () => Assert.That(ofNull == defaultInterface, Is.False, "ofNull == defaultInterface"),
            // () => Assert.That(ofNull != defaultInterface, "ofNull != defaultInterface"),
            () => Assert.That(ofNull, Is.Not.EqualTo(defaultInterface), "ofNull, Is.Not.EqualTo(defaultInterface)"),
            // vs. empty
            () => Assert.That(ofNull == empty,                        Is.False, "ofNull == empty"),
            () => Assert.That(ofNull != empty,                        "ofNull != empty"),
            () => Assert.That(ofNull,                                 Is.Not.EqualTo(empty), "ofNull, Is.Not.EqualTo(empty)"),
            () => Assert.That((ofNull == empty) == (empty == ofNull), "(ofNull == empty) == (empty == ofNull)")
        );
    }

    [Test]
    public void Serialize_NestedOptionalOfArray() {
        var inner = Optional.Of(
            new[] {
                1, 2, 3
            }
        );
        var outer = Optional.Of(inner);
        // Console.WriteLine(inner == outer);
        var json = JsonConvert.SerializeObject(outer);
        Console.WriteLine(json);
    }

    #endregion

    #region ToOptional

    [Test]
    public void ToOptional_MultipleItems() {
        var ls = new[] {
            1, 2, 3
        };

        Assert.Throws<InvalidOperationException>(() => ls.ToOptional());
    }

    [Test]
    public void ToOptional_Empty() {
        var ls = Array.Empty<int>();
        Asserter.Against(ls.ToOptional)
                .And(it => it.IsEmpty)
                .Invoke();
    }

    [Test]
    [TestCase(double.PositiveInfinity)]
    [TestCase("#yolo")]
    [TestCase(
        new[] {
            1, 2, 3
        }
    )]
    [TestCase(default(object))]
    public void Enumerable_ToOptional_Single(object value) {
        // var value = double.NegativeInfinity;
        var ls = new object[] {
            value
        };

        Asserter.Against(ls.ToOptional())
                .And(it => it.Count() == 1)
                .And(it => it.HasValue)
                .And(it => it.Value == value)
                .And(it => it.IsNotEmpty())
                .Invoke();
    }

    #endregion

    #region ToString

    public static (Optional<object?>, string)[] GetOptionalToStringExpectations() {
        return new[] {
            (new Optional<object?>(5), "5"),
            (new Optional<object?>(), Optional.EmptyPlaceholder),
            (new Optional<object?>(null), ""),
            (new Optional<object?>(new Optional<object>(new Optional<object>("yolo"))), "yolo")
        };
    }

    [Test]
    public void OptionalToString([ValueSource(nameof(GetOptionalToStringExpectations))] (Optional<object>, string) expectation) {
        var (optional, expectedString) = expectation;
        Assert.That(optional.ToString(), Is.EqualTo(expectedString));
    }

    #endregion

    #region Flatten

    [Test]
    public void Flatten_2() {
        var inner = Optional.Of(5);
        var outer = Optional.Of(inner);
        var flat  = outer.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(true))
                .And(it => it.Value,    Is.EqualTo(inner.Value))
                .Invoke();
    }

    [Test]
    public void Flatten_2_Empty() {
        var inner = Optional.Empty<int>();
        var outer = Optional.Of(inner);
        var flat  = outer.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(false))
                .Satisfies(it => _ = it.Value, Throws.InvalidOperationException)
                .Invoke();
    }

    [Test]
    public void Flatten_3() {
        var inner  = Optional.Of(5);
        var middle = Optional.Of(inner);
        var outer  = Optional.Of(middle);
        var flat   = outer.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(true))
                .And(it => it.Value,    Is.EqualTo(5))
                .Invoke();
    }

    [Test]
    public void Flatten_3_Empty() {
        var inner  = Optional.Empty<int>();
        var middle = Optional.Of(inner);
        var outer  = Optional.Of(middle);
        var flat   = outer.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(false))
                .Satisfies(it => _ = it.Value, Throws.InvalidOperationException)
                .Invoke();
    }

    [Test]
    public void Flatten_4() {
        var t1   = Optional.Of(5);
        var t2   = Optional.Of(t1);
        var t3   = Optional.Of(t2);
        var t4   = Optional.Of(t3);
        var flat = t4.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(true))
                .And(it => it.Value,    Is.EqualTo(5))
                .Invoke();
    }

    [Test]
    public void Flatten_4_Empty() {
        var t1   = Optional.Empty<int>();
        var t2   = Optional.Of(t1);
        var t3   = Optional.Of(t2);
        var t4   = Optional.Of(t3);
        var flat = t4.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.EqualTo(false))
                .Satisfies(it => _ = it.Value, Throws.InvalidOperationException)
                .Invoke();
    }

    [Test]
    public void Flatten_Mixed() {
        var inner = Optional.Empty<Optional<int>>();
        var outer = Optional.Of(inner);
        var flat  = outer.Flatten();

        Asserter.Against(flat)
                .And(Is.TypeOf<Optional<int>>())
                .And(it => it.HasValue, Is.False)
                .Satisfies(it => _ = it.Value, Throws.InvalidOperationException)
                .And(() => outer.HasValue, Is.True)
                .Invoke();
    }

    #endregion
}