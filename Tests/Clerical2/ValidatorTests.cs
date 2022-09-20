using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.Clerical.Fluffy;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BSharp.Tests.Clerical2;

public class ValidatorTests {
    public record ValidatorMethodExpectation(Type ValidatedType, string MethodName, ValidatorStyle? Style) {
        public MethodInfo Method => ValidatedType.GetMethod(MethodName, MemberTypes.Method.GetBindingFlags()) ?? throw new NullReferenceException($"Method {MethodName} wasn't found!");

        public void DescribeMethod() {
            Brandon.Print(Method.IsChainable());
            Brandon.Print(Method.IsCheckpoint());
            Brandon.Print(Method.IsVoid());
            Brandon.Print(Method.ReturnType);
        }
    };

    public record ValidatedTypeExpectation(Type ValidatedType, params ValidatorMethodExpectation[] ValidatorMethodExpectations) { }

    public record PositiveNumber(int N) {
        [Validator] public static bool           Static_Predicate(PositiveNumber n)  => n.N >= 0;
        [Validator] public        bool           Instance_Predicate()                => N   >= 0;
        [Validator] public static PositiveNumber Static_Checkpoint(PositiveNumber n) => Must.Be(n,    it => it.N.IsPositive());
        [Validator] public        PositiveNumber Instance_Checkpoint()               => Must.Be(this, it => it.N.IsPositive());
        [Validator] public static void           Static_Assertion(PositiveNumber n)  => Must.BePositive(n.N);
        [Validator] public        void           Instance_Assertion()                => Must.BePositive(N);

        public static readonly ValidatorMethodExpectation[] PositiveNumberMethods = {
            new(typeof(PositiveNumber), nameof(Static_Predicate), ValidatorStyle.Predicate),
            new(typeof(PositiveNumber), nameof(Instance_Predicate), ValidatorStyle.Predicate),
            new(typeof(PositiveNumber), nameof(Static_Checkpoint), ValidatorStyle.Checkpoint),
            new(typeof(PositiveNumber), nameof(Instance_Checkpoint), ValidatorStyle.Checkpoint),
            new(typeof(PositiveNumber), nameof(Static_Assertion), ValidatorStyle.Assertion),
            new(typeof(PositiveNumber), nameof(Instance_Assertion), ValidatorStyle.Assertion),
        };
    }

    public record EvenPositiveNumber(int N) : PositiveNumber(N) {
        [Validator] private bool IsEven() => N.IsEven();

        [Validator]
        private void ThrowIfOdd() {
            if (N.IsNotEven()) {
                throw new ArgumentException("was odd");
            }
        }

        [Validator] private static void EvenPls(EvenPositiveNumber     value) => Must.Be(value.N.IsEven());
        [Validator] private static void CheckParentType(PositiveNumber value) => Must.Be(value.N.IsEven());

        public static readonly ValidatorMethodExpectation[] EvenNumberMethods = {
            new(typeof(EvenPositiveNumber), nameof(IsEven), ValidatorStyle.Predicate),
            new(typeof(EvenPositiveNumber), nameof(ThrowIfOdd), ValidatorStyle.Assertion),
            new(typeof(EvenPositiveNumber), nameof(EvenPls), ValidatorStyle.Assertion),
            new(typeof(EvenPositiveNumber), nameof(CheckParentType), ValidatorStyle.Assertion),
        };
    }

    public static readonly Dictionary<Type, ValidatorMethodExpectation[]> DeclaredValidatorMethods = new() {
        [typeof(PositiveNumber)]     = PositiveNumber.PositiveNumberMethods,
        [typeof(EvenPositiveNumber)] = EvenPositiveNumber.EvenNumberMethods,
    };

    public static readonly Dictionary<Type, ValidatorMethodExpectation[]> ExpectedValidatorMethods = new() {
        [typeof(PositiveNumber)]     = PositiveNumber.PositiveNumberMethods,
        [typeof(EvenPositiveNumber)] = DeclaredValidatorMethods[typeof(PositiveNumber)].Concat(DeclaredValidatorMethods[typeof(EvenPositiveNumber)]).ToArray(),
    };

    public static readonly ValidatorMethodExpectation[] AllValidatorMethods = DeclaredValidatorMethods.SelectMany(it => it.Value).ToArray();

    [Test]
    public void ValidatorStyleTest([ValueSource(nameof(AllValidatorMethods))] ValidatorMethodExpectation expectation) {
        Assert.That(Validator.InferValidatorStyle(expectation.Method), Is.EqualTo(expectation.Style));
    }

    [Test]
    [TestCase(typeof(PositiveNumber))]
    [TestCase(typeof(EvenPositiveNumber))]
    public void GetValidatorMethods(Type validatedType) {
        var validatorMethods = Validator.GetValidatorMethods(validatedType).ToArray();
        Assert.That(validatorMethods, Is.Unique);
        Assert.That(validatorMethods, Has.Length.EqualTo(ExpectedValidatorMethods[validatedType].Length));
    }

    [Test]
    public void Validate_PositiveNumber([Values(-1, 0, -2, -99, int.MinValue, int.MaxValue, 1, 2)] int value) {
        var                positiveNumber = new PositiveNumber(value);
        IResolveConstraint constraint     = value < 0 ? Throws.Exception : Throws.Nothing;
        Assert.That(() => Validator.Validate(positiveNumber), constraint);
    }

    [Test]
    public void Child_Pass() {
        var evenNumb = new EvenPositiveNumber(4);
        Asserter.Against(Validator.TryValidate(evenNumb))
                .And(Has.Count.EqualTo(PositiveNumber.PositiveNumberMethods.Length + EvenPositiveNumber.EvenNumberMethods.Length))
                .And(Has.None.With.Property(nameof(IFailable.Failed)).True)
                .Invoke();
    }

    [Test]
    [TestCase(typeof(PositiveNumber), nameof(PositiveNumber.Instance_Assertion), typeof(PositiveNumber))]
    public void Get_Validated_Type(Type owningType, string methodName, Type expectedValidatedType) {
        var bd = ImmutableArray.CreateBuilder<int>();
        var lb = ImmutableList.CreateBuilder<string>();
    }

    [Test]
    public void Child_Fail_Parent_Pass() {
        var oddNumb           = new EvenPositiveNumber(9);
        var validationResults = Validator.TryValidate(oddNumb).ToArray();
        Asserter.Against(validationResults)
                .And(it => it.Where(f => f.Failed).ToArray(), Has.Length.EqualTo(EvenPositiveNumber.EvenNumberMethods.Length))
                .And(it => it.Where(f => f.Passed).ToArray(), Has.Length.EqualTo(PositiveNumber.PositiveNumberMethods.Length))
                .Invoke();
    }

    [Test]
    public void Child_Pass_Parent_Fail() {
        var negativeEven      = new EvenPositiveNumber(-8);
        var validationResults = Validator.TryValidate(negativeEven).ToArray();
        Asserter.Against(validationResults)
                .And(it => it.Where(f => f.Failed).ToArray(), Has.Length.EqualTo(PositiveNumber.PositiveNumberMethods.Length))
                .And(it => it.Where(f => f.Passed).ToArray(), Has.Length.EqualTo(EvenPositiveNumber.EvenNumberMethods.Length))
                .Invoke();
    }
}