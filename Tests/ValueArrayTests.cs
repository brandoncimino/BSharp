using System;
using System.Collections.Immutable;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using FowlFever.BSharp;

using NUnit.Framework;

namespace BSharp.Tests;

public class ValueArrayTests {
    [TestCase(99)]
    [TestCase(1, 2, 3)]
    public void DifferentArrays_WithSameValues_AreEqual(params int[] values) {
        var ar1 = ValueArray.Of(values);
        var ar2 = ValueArray.Of(values);

        ar1.AsSpan().Overlaps(ar2.AsSpan()).Should().BeFalse();
        ar1.Equals(ar2).Should().BeTrue();
        (ar1 == ar2).Should().BeTrue();
    }

    [TestCase(1, 2, 3)]
    public void DifferentOrder_IsNotEqual(params int[] values) {
        Assume.That(values.Length >= 2);

        var ar1 = ValueArray.Of(values);
        var ar2 = ValueArray.Of(values.Reverse());

        ar1.Equals(ar2).Should().BeFalse();
    }

    [Test]
    public void CollectionOfOne_EqualsSingleItem() {
        var ar = ValueArray.Of(9);
        ar.Equals(9).Should().BeTrue();
        (ar == 9).Should().BeTrue();
    }

    [Test]
    public void Default_IsEmpty() {
        var def = default(ValueArray<int>);

        using var scope = new AssertionScope();

        def.IsEmpty.Should().BeTrue();
        def.IsNotEmpty.Should().BeFalse();
        def.Equals(default(ValueArray<int>)).Should().BeTrue();
        def.Equals(default(ImmutableArray<int>)).Should().BeTrue();
        def.Equals(ImmutableArray<int>.Empty).Should().BeTrue();
        def.AsImmutableArray.IsDefault.Should().BeFalse();
        def.AsImmutableArray.IsEmpty.Should().BeTrue();
    }

    [Test]
    public void NullableValueArray_Equals_Normal() {
        ValueArray<int>  ar  = ValueArray.Of(1, 2, 3);
        ValueArray<int>? nar = ValueArray.Of(1, 2, 3);

        using var scope = new AssertionScope();

        ar.Equals(nar).Should().BeTrue();
        nar.Equals(ar).Should().BeTrue();
        (ar          == nar).Should().BeTrue();
        (nar         == ar).Should().BeTrue();
        (ar.AsSpan() == nar.AsSpan()).Should().BeFalse();
        ar.AsSpan().SequenceEqual(nar.AsSpan()).Should().BeTrue();
    }

    [Test]
    public void Empty_Equals_OneNullable() {
        var  ar   = ValueArray<int>.Empty;
        int? nint = null;

        (ar   == nint).Should().BeTrue();
        (nint == ar).Should().BeTrue();
        ar.Equals(nint).Should().BeTrue();
    }

    [Test]
    public void NullableArray_Equals_NullableItem() {
        ValueArray<int>? nar  = null;
        int?             nint = null;

        (nar  == nint).Should().BeTrue();
        (nint == nar).Should().BeTrue();
    }

    [TestCase(new int[] { 1, 2, 3 }, new int[] { 9, 9, 9 }, new int[] { 1, 2, 3, 9, 9, 9 })]
    public void Array_Plus_Array(int[] a, int[] b, int[] expected) {
        var aVar   = ValueArray.Of(a);
        var bVar   = ValueArray.Of(b);
        var actual = aVar + bVar;
        actual.SequenceEqual(expected).Should().BeTrue();
    }

    [TestCase(new int[] { 1, 2, 3 }, 9, new int[] { 1, 2, 3, 9 })]
    public void Array_Plus_Item(int[] array, int item, int[] expected) {
        var aVar   = ValueArray.Of(array);
        var actual = aVar + item;
        actual.SequenceEqual(expected).Should().BeTrue();
    }

    [Test]
    [TestCase(new int[] { 0, 1, 2, 3 }, 0, 2)]
    public void Slice_Returns_Range(int[] input, int start, int end) {
        var ar                  = ValueArray.Of(input);
        var range               = start..end;
        var expected_array      = input[range];
        var expected_valueArray = ValueArray.Of(expected_array);
        var actual              = ar[range];
        actual.SequenceEqual(expected_array).Should().BeTrue();
        actual.Equals(expected_valueArray).Should().BeTrue();
    }
}