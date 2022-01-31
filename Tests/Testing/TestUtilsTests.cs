using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp;
using FowlFever.BSharp.Enums;
using FowlFever.Testing;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = FowlFever.Testing.Is;

namespace BSharp.Tests.Testing; 

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public class TestUtilsTests {
    private readonly struct ApproximationExpectation {
        private readonly double _actual;
        public readonly  object Actual;
        private readonly double _threshold;
        public readonly  object Threshold;
        private readonly Type   Expected_Type;
        private readonly Type   Threshold_Type;

        public ApproximationExpectation(Type actual_type, Type expectedType, Type thresholdType, double actual = 20, double threshold = 5) {
            _actual        = actual;
            _threshold     = threshold;
            Expected_Type  = expectedType;
            Threshold_Type = thresholdType;
            Actual         = Convert.ChangeType(actual,    actual_type);
            Threshold      = Convert.ChangeType(threshold, thresholdType);
        }

        public object Expected_ToPass(Clusivity clusivity) {
            var value = _actual + (clusivity == Clusivity.Inclusive ? _threshold : _threshold / 2);
            return Convert.ChangeType(value, Expected_Type);
        }

        public object Expected_ToFail(Clusivity clusivity) {
            var value = _actual + (clusivity == Clusivity.Inclusive ? _threshold * 1.5 : _threshold);
            return Convert.ChangeType(value, Expected_Type);
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(
                new Dictionary<object, object>() {
                    { nameof(Actual), $"[{Actual.GetType()}]{Actual}" },
                    { nameof(Threshold), $"[{Threshold_Type}]{Threshold}" },
                    { nameof(Expected_ToPass), $"[{Expected_Type}]{Expected_ToPass(Clusivity.Inclusive)}" },
                    { nameof(Expected_ToFail), $"[{Expected_Type}]{Expected_ToFail(Clusivity.Inclusive)}" },
                }
            );
        }
    }

    [Test]
    public void ApproximationConstraint(
        [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
        Type actualType,
        [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
        Type expectedType,
        [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
        Type thresholdType,
        [Values] Clusivity clusivity
    ) {
        var exp = new ApproximationExpectation(actualType, expectedType, thresholdType);

        Console.WriteLine(exp);

        AssertAll.Of(
            () => Assert.That(exp.Actual, Is.Approximately(exp.Expected_ToPass(clusivity), exp.Threshold, clusivity),     $"{nameof(exp.Expected_ToPass)}, {clusivity}"),
            () => Assert.That(exp.Actual, Is.Not.Approximately(exp.Expected_ToFail(clusivity), exp.Threshold, clusivity), $"{nameof(exp.Expected_ToFail)}, {clusivity}")
        );
    }
}