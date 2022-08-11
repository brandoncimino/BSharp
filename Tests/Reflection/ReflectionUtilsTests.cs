using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.Model;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.Testing;

using NUnit.Framework;

// ReSharper disable StaticMemberInGenericType

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace BSharp.Tests.Reflection;

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public partial class ReflectionUtilsTests {
    private const int Prop_Static_Get_Only_Default_Value = 5;

    #region Variables

    public record VariableExpectation {
        public string      Name       { get; init; } = null!;
        public MemberTypes MemberType { get; init; }

        private bool _gettable;
        public bool Gettable {
            get => MemberType == MemberTypes.Field || _gettable;
            set => _gettable = value;
        }

        private bool _settable;
        public bool Settable {
            get => MemberType == MemberTypes.Field || _settable;
            set => _settable = value;
        }

        public string? BackingFieldName => MemberType != MemberTypes.Property ? null : $"<{Name}>k__BackingField";

        public override string ToString() {
            return Name;
        }
    }

    private class Privacy<T> {
        public    T Field_Public;
        private   T Field_Private;
        protected T Field_Protected;

        public    T Prop_Public               { get;         set; }
        private   T Prop_Private              { get;         set; }
        protected T Prop_Protected            { get;         set; }
        public    T Prop_Mixed_Private_Getter { private get; set; }
        public    T Prop_Mixed_Private_Setter { get;         private set; }
        public    T Prop_Get_Only             { get; }

        public static    T?  Field_Static_Public;
        private static   T?  Field_Static_Private;
        protected static T?  Field_Static_Protected;
        public static    T?  Prop_Static_Public               { get;         set; }
        private static   T?  Prop_Static_Private              { get;         set; }
        protected static T?  Prop_Static_Protected            { get;         set; }
        public static    T?  Prop_Static_Mixed_Private_Getter { private get; set; }
        public static    T?  Prop_Static_Mixed_Private_Setter { get;         private set; }
        public static    int Prop_Static_Get_Only             { get; } = Prop_Static_Get_Only_Default_Value;

        public Privacy(T value) {
            Field_Public    = value;
            Field_Private   = value;
            Field_Protected = value;

            Prop_Public               = value;
            Prop_Private              = value;
            Prop_Protected            = value;
            Prop_Mixed_Private_Getter = value;
            Prop_Mixed_Private_Setter = value;
            Prop_Get_Only             = value;

            Field_Static_Public    = value;
            Field_Static_Private   = value;
            Field_Static_Protected = value;

            Prop_Static_Public               = value;
            Prop_Static_Private              = value;
            Prop_Static_Protected            = value;
            Prop_Static_Mixed_Private_Getter = value;
            Prop_Static_Mixed_Private_Setter = value;
        }

        public static List<VariableExpectation> VariableInfos() => new List<VariableExpectation>() {
            #region Instance Variables

            #region Instance Fields

            new() { Name = nameof(Field_Public), MemberType    = MemberTypes.Field },
            new() { Name = nameof(Field_Private), MemberType   = MemberTypes.Field },
            new() { Name = nameof(Field_Protected), MemberType = MemberTypes.Field },

            #endregion

            #region Instance Properties

            new() { Name = nameof(Prop_Public), MemberType               = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Private), MemberType              = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Protected), MemberType            = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Mixed_Private_Getter), MemberType = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Mixed_Private_Setter), MemberType = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Get_Only), MemberType             = MemberTypes.Property, Gettable = true, Settable = false },

            #endregion

            #endregion

            #region Static Variables

            #region Static Fields

            new() { Name = nameof(Field_Static_Public), MemberType    = MemberTypes.Field },
            new() { Name = nameof(Field_Static_Private), MemberType   = MemberTypes.Field },
            new() { Name = nameof(Field_Static_Protected), MemberType = MemberTypes.Field },

            #endregion

            #region Static Properties

            new() { Name = nameof(Prop_Static_Public), MemberType               = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Static_Private), MemberType              = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Static_Protected), MemberType            = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Static_Mixed_Private_Getter), MemberType = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Static_Mixed_Private_Setter), MemberType = MemberTypes.Property, Gettable = true, Settable = true },
            new() { Name = nameof(Prop_Static_Get_Only), MemberType             = MemberTypes.Property, Gettable = true, Settable = false },

            #endregion

            #endregion
        };
    }

    /// <summary>
    /// Returns the settable entries from <see cref="AllVariables"/>
    /// </summary>
    public static List<VariableExpectation> SettableVariables => Privacy<int>.VariableInfos().Where(it => it.Settable).ToList();

    /// <summary>
    /// Allows access to <see cref="Privacy{T}.VariableInfos"/> cleanly in <see cref="ValueSourceAttribute"/>s
    /// </summary>
    public static List<VariableExpectation> AllVariables => Privacy<int>.VariableInfos();
    public static List<string> AllVariableNames => AllVariables.Select(it => it.Name).ToList();

    /// <summary>
    /// Returns the <see cref="MemberTypes.Property"/> entries from <see cref="AllVariables"/>
    /// </summary>
    public static List<VariableExpectation> AllProperties => Privacy<int>.VariableInfos().Where(it => it.MemberType == MemberTypes.Property).ToList();

    [Test]
    public void GetVariable(
        [ValueSource(nameof(AllVariables))] VariableExpectation expectedGettableVariable
    ) {
        var setInt = expectedGettableVariable.Name == nameof(Privacy<int>.Prop_Static_Get_Only) ? Prop_Static_Get_Only_Default_Value : new Random().Next();

        var privacy = new Privacy<int>(setInt);

        var val = privacy.GetVariableValue<int>(expectedGettableVariable.Name);
        Assert.That(val, Is.EqualTo(setInt));
    }

    [Test]
    public void SetVariable(
        [ValueSource(nameof(SettableVariables))]
        VariableExpectation expectedSettableVariable
    ) {
        Assume.That(expectedSettableVariable, Has.Property(nameof(expectedSettableVariable.Settable)).True);

        var initialInt = new Random().Next();
        var updatedInt = initialInt + 1;

        var privacy = new Privacy<int>(initialInt);

        VariableInfoExtensions.SetVariableValue(privacy, expectedSettableVariable.Name, updatedInt);
        Assert.That(VariableInfoExtensions.GetVariableValue(privacy, expectedSettableVariable.Name), Is.EqualTo(updatedInt));
        Assert.That(VariableInfoExtensions.GetVariableValue(privacy, expectedSettableVariable.Name), Is.Not.EqualTo(initialInt));
    }

    /// <summary>
    /// Somewhat redundant with <see cref="GetVariablesHasOnlyExpectedVariables"/>, but Unity's version of NUnit doesn't support soft assertions >:(
    /// </summary>
    [Test]
    public void GetVariablesHasAllExpectedVariables() {
        var actualVariableNames = typeof(Privacy<int>).GetVariables().Select(it => it.Name).ToList();
        Assert.That(actualVariableNames, Is.SupersetOf(AllVariableNames));
    }

    /// <summary>
    /// Somewhat redundant with <see cref="GetVariablesHasAllExpectedVariables"/>, but Unity's version of NUnit doesn't support soft assertions >:(
    /// </summary>
    [Test]
    public void GetVariablesHasOnlyExpectedVariables() {
        var actualVariableNames = typeof(Privacy<int>).GetVariables().Select(it => it.Name).ToList();
        Assert.That(actualVariableNames, Is.SubsetOf(AllVariableNames));
    }

    [Test]
    public void GetVariables() {
        Assert.That(
            typeof(Privacy<int>).GetVariables().Select(it => it.Name),
            Is.EquivalentTo(AllVariables.Select(it => it.Name))
        );
    }

    #region Backing Fields

    [Test]
    public void GetBackedPropertyName(
        [ValueSource(nameof(AllVariables))] VariableExpectation propertyWithBackingField
    ) {
        if (propertyWithBackingField.MemberType != MemberTypes.Property) {
            throw new IgnoreException($"{propertyWithBackingField} is not a {MemberTypes.Property}!");
        }

        var backingFieldInfo = typeof(Privacy<int>).GetField(propertyWithBackingField.BackingFieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        Assert.That(backingFieldInfo, Is.Not.Null, $"Unable to retrieve a field named {propertyWithBackingField.BackingFieldName}");

        Console.WriteLine($"propInfo: {backingFieldInfo}");
        Console.WriteLine(backingFieldInfo.Name);
        Assert.That(AutoProperty.GetBackedPropertyName(backingFieldInfo.Name), Is.EqualTo(propertyWithBackingField.Name));
    }

    [Test]
    public void BackingField(
        [ValueSource(nameof(AllProperties))] VariableExpectation propertyWithBackingField
    ) {
        Assume.That(propertyWithBackingField.MemberType, Is.EqualTo(MemberTypes.Property));
        var propertyInfo = typeof(Privacy<int>).GetProperty(propertyWithBackingField.Name, BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

        Assert.That(propertyInfo, Is.Not.Null, $"Couldn't retrieve a property info for {propertyWithBackingField}");

        var backingField = propertyInfo?.GetBacker();

        Assert.That(backingField, Is.Not.Null, $"Couldn't retrieve a backing field for {propertyWithBackingField}");

        Assert.That(backingField, Has.Property(nameof(MemberInfo.Name)).EqualTo(propertyWithBackingField.BackingFieldName), $"Found a backing field for {propertyWithBackingField}, but it wasn't named {propertyWithBackingField.BackingFieldName}");
    }

    /// <summary>
    /// TODO: Revisit this test, taking advantage of the <see cref="System.Runtime.CompilerServices.CompilerGeneratedAttribute"/>
    /// </summary>
    /// <param name="propertyWithBackingField"></param>
    [Test]
    public void IsBackingField(
        [ValueSource(nameof(AllProperties))] VariableExpectation propertyWithBackingField
    ) {
        Assume.That(propertyWithBackingField.MemberType, Is.EqualTo(MemberTypes.Property));
        var backingFieldInfo = typeof(Privacy<int>).GetField(propertyWithBackingField.BackingFieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        Assert.That(backingFieldInfo,                             Is.Not.Null);
        Assert.That(backingFieldInfo!.IsAutoPropertyBackingField, Is.True);
    }

    #endregion

    #endregion

    #region Constructing

    #region Classes with constructors

    private abstract class Constructibles {
        public bool Successful { get; private set; }

        public class NoParamConstructor : Constructibles {
            public NoParamConstructor() {
                Successful = true;
            }
        }

        public class OptionalParamConstructor : Constructibles {
            public bool OptionalWasProvided { get; }

            public OptionalParamConstructor(bool optionalWasProvided = false) {
                Successful          = true;
                OptionalWasProvided = optionalWasProvided;
            }
        }
    }

    #endregion

    [Test]
    public void NoParamConstructor() {
        Assert.That(ReflectionUtils.Construct<Constructibles.NoParamConstructor>(), Has.Property(nameof(Constructibles.Successful)).True);
    }

    [Test]
    public void OptionalParamConstructor_NotProvided() {
        Assert.That(
            () => ReflectionUtils.Construct<Constructibles.OptionalParamConstructor>(),
            Throws.InstanceOf<MissingMemberException>()
        );
    }

    [Test]
    public void OptionalParamConstructor_Provided_JustGeneric() {
        Assert.That(
            ReflectionUtils.Construct<Constructibles.OptionalParamConstructor>(true),
            Has.Property(nameof(Constructibles.Successful))
               .True
               .And.Property(nameof(Constructibles.OptionalParamConstructor.OptionalWasProvided))
               .True
        );
    }

    #endregion

    #region Type Ancestry

    [TestCase(typeof(PropertyInfo), typeof(MemberInfo), typeof(object))]
    [TestCase(typeof(MethodInfo),   typeof(MethodBase), typeof(MemberInfo), typeof(object))]
    [TestCase(typeof(int),          typeof(ValueType),  typeof(object))]
    [TestCase(typeof(object))]
    public void GetAncestors(Type heir, params Type[] ancestors) {
        DiffResult DiffArr<T>(IEnumerable<T> a, IEnumerable<T> b) {
            const string token  = "%$/";
            var          aStr   = a.JoinString(token);
            var          bStr   = b.JoinString(token);
            var          diffar = Differ.Instance.CreateCustomDiffs(aStr, bStr, false, false, str => str.Split(token));

            var sbs = SideBySideDiffBuilder.Diff(new Differ(), aStr, bStr, false, false, new CustomFunctionChunker(str => str.Split(token)));
            Console.WriteLine($"sbs: {sbs}");
            Console.WriteLine("old");
            Console.WriteLine(sbs.OldText.Lines.Select(it => $"{it.Type,20} > {it.Text,5} // {it.SubPieces}").JoinLinesNumbered());
            Console.WriteLine("neu");
            Console.WriteLine(sbs.NewText.Lines.Select(it => $"{it.Type,20} > {it.Text,5} // {it.SubPieces}").JoinLinesNumbered());

            return diffar;
        }

        // var a      = new []{1,2,3,7};
        // var b      = new List<int>(){1,2,3,9,9,9,9,9};
        var a      = new[] { 1, 23, 4 };
        var b      = new[] { 1, 2, 3, 4 };
        var diffar = DiffArr(a, b);
        Brandon.Print(diffar);
        Brandon.Print(diffar.DiffBlocks);
        Console.WriteLine(diffar.DiffBlocks.JoinLinesNumbered());
        Brandon.Print(diffar.PiecesOld);
        Brandon.Print(diffar.PiecesNew);


        var giff = Giff.Of(a, b);
        Console.WriteLine(giff);

        Assert.That(a, Is.EquivalentTo(b.AsEnumerable()));
        return;
        Assert.That(heir.GetAncestors(ReflectionUtils.AncestryOption.IncludeSelf).ToArray(), Is.EqualTo(ancestors).AsCollection);
    }

    [TestCase(
        new[] {
            typeof(List<object>), typeof(IList<object>), typeof(IEnumerable<object>)
        },
        typeof(IEnumerable<object>)
    )]
    [TestCase(
        new[] {
            typeof(int), typeof(string)
        },
        typeof(IComparable)
    )]
    [TestCase(
        new[] {
            typeof(List<object>), typeof(Collection<object>)
        },
        typeof(IList<object>)
    )]
    public void MostCommonType(Type[] types, Type expectedType) {
        var ass = Asserter.WithHeading($"{nameof(MostCommonType)} for: {types.Prettify()}");

        new[] {
            (types, "Forward"), (types.Randomize(), "Random"), (types.Reverse(), "Reversed")
        }.ForEach(
            it => {
                var (typeList, message) = it;
                ass.And(() => Assert.That(ReflectionUtils.CommonType(typeList), Is.EqualTo(expectedType), message));
            }
        );

        ass.Invoke();
    }

    public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> AllInterfaceExpectations() {
        return InterfaceExpectations.ExpectedTypes;
    }

    [Test]
    public void AllInterfacesTest([ValueSource(nameof(AllInterfaceExpectations))] KeyValuePair<Type, IEnumerable<Type>> expectation) {
        Assert.That(ReflectionUtils.GetAllInterfaces(expectation.Key).ToList(), Is.EquivalentTo(expectation.Value.Where(it => it.IsInterface).ToList()));
    }

    [Test]
    [TestCase(
        typeof(Duckmobile),
        typeof(TrainCar),
        new[] {
            typeof(ILand), typeof(ICar), typeof(IVehicle)
        }
    )]
    public void CommonInterface(Type a, Type b, Type[] expectedOptions) {
        var actual = ReflectionUtils.CommonInterfaces(a, b);

        Assert.That(actual, Is.EquivalentTo(expectedOptions));
    }

    #endregion

    #region Assignability (inheritance)

    public class SuperList<T> : List<T> { }

    public class IntList : List<int> { }

    public class SuperIntList : SuperList<int> { }

    [Test]
    [TestCase(typeof(int[]),                     Should.Pass)]
    [TestCase(typeof(string),                    Should.Pass)]
    [TestCase(typeof(KeyedList<int, DayOfWeek>), Should.Pass)]
    [TestCase(typeof(int),                       Should.Fail)]
    [TestCase(typeof(int[][][]),                 Should.Pass)]
    [TestCase(typeof(IEnumerable<object>),       Should.Pass)]
    [TestCase(typeof(IEnumerable<>),             Should.Pass)]
    [TestCase(typeof(List<List<int[]>>),         Should.Pass)]
    [TestCase(typeof(IList<List<object>>),       Should.Pass)]
    [TestCase(typeof(IDictionary<,>),            Should.Pass)]
    [TestCase(typeof(SuperList<>),               Should.Pass)]
    [TestCase(typeof(SuperList<int>),            Should.Pass)]
    [TestCase(typeof(IntList),                   Should.Pass)]
    [TestCase(typeof(SuperIntList),              Should.Pass)]
    [TestCase(typeof(SuperList<object>),         Should.Pass)]
    [TestCase(typeof(IEnumerable),               Should.Fail)]
    [TestCase(typeof(IList),                     Should.Fail)]
    public static void IsEnumerable(Type type, Should should) {
        Asserter.Against(type)
                .WithHeading($"{type.Prettify()} is IEnumerable<>")
                .And(ReflectionUtils.IsEnumerable,               should.Constrain())
                .And(it => it.Implements(typeof(IEnumerable<>)), should.Constrain())
                .Invoke();
    }

    private static void yolo() {
        Console.WriteLine("#🐣");
    }

    private static (Delegate dgate, bool compGen, string nickname)[] Gates = {
        (new Action(yolo), false, "new Action(yolo)"),
        (new Func<string>(() => "swag?"), true, "new Func<string>(() => \"swag?\")"),
        (new Func<Delegate, PrettificationSettings, string>(InnerPretty.PrettifyDelegate), false, "new Func<Delegate, PrettificationSettings, string>(InnerPretty.PrettifyDelegate)"),
        (new Func<Delegate, PrettificationSettings, string>((del, set) => InnerPretty.PrettifyDelegate(del, set)), true, "new Func<Delegate, PrettificationSettings, string>((del, set) => InnerPretty.PrettifyDelegate(del, set))"),
        (new Func<bool, string>(it => it.Icon()), true, "it => it.Icon()"),
        (new Func<bool, string>(PrimitiveUtils.Icon), false, "PrimitiveUtils.Icon"),
    };

    private void yolocal() {
        Console.WriteLine("♿💔");
    }

    private static readonly Action YoAct = yolo;

    private static readonly (Action action, bool compGen, string nickname)[] Actions = {
        (yolo, false, "yolo"),
        (() => yolo(), true, "() => yolo()"),
        (() => _ = 5, true, "() => _ = 5"),
        (() => { }, true, "() => { }"),
        (new ReflectionUtilsTests().yolocal, false, "new ReflectionUtilsTests().yolocal"),
        (() => YoAct.Invoke(), true, "() => YoAct.Invoke()"),
        (YoAct.Invoke, false, "YoAct.Invoke"),
        (YoAct, false, "YoAct"),
        (new Action(YoAct), false, "new Action(YoAct)")
    };

    #endregion

    #region Compiler-Generated Stuff

    [Test]
    public void DelegateIsCompilerGenerated() {
        var ass    = Asserter.WithHeading("Actions");
        var aGates = Actions.Select(it => ((Delegate)it.action, it.compGen, it.nickname));
        Gates.Concat(aGates)
             .ForEach((dg, cg, nn) => ass.And(() => dg.IsCompilerGenerated(), Is.EqualTo(cg), nn));
        ass.Invoke();
    }

    #endregion

    #region Implements

    public interface ISuperList<T> : IList<T> { }

    [Test]
    [TestCase(typeof(List<>),                            typeof(IEnumerable<>))]
    [TestCase(typeof(IList<>),                           typeof(ICollection<>))]
    [TestCase(typeof(SuperList<>),                       typeof(IEnumerable))]
    [TestCase(typeof(ISuperList<int>),                   typeof(IList<int>))]
    [TestCase(typeof(ISuperList<int>),                   typeof(IEnumerable))]
    [TestCase(typeof(IReadOnlyCollection<int>),          typeof(IReadOnlyCollection<>))]
    [TestCase(typeof(ReadOnlyCollection<int>),           typeof(IReadOnlyCollection<>))]
    [TestCase(typeof(List<string>),                      typeof(IReadOnlyCollection<string>))]
    [TestCase(typeof(IList<IList<object>>),              typeof(IEnumerable<IEnumerable<object>>))]
    [TestCase(typeof(IList<ISuperList<SuperList<int>>>), typeof(IEnumerable<IEnumerable<IEnumerable>>))]
    public void ImplementsOther(Type self, Type other) {
        Assume.That(other.IsInterface);
        Asserter.Against(self)
                .And(it => it.Implements(other), Is.True, $"{self.Prettify()} implements {other.Prettify()}")
                .Invoke();
    }

    [Test]
    [TestCase(typeof(List<>),                   typeof(IList<int>))]
    [TestCase(typeof(IReadOnlyCollection<int>), typeof(IReadOnlyCollection<string>))]
    [TestCase(typeof(IList<int>),               typeof(IEnumerable<string>))]
    [TestCase(typeof(IEnumerable),              typeof(IEnumerable<>))]
    [TestCase(typeof(IEnumerable<int>),         typeof(IList<int>))]
    public void Implements_NOT(Type self, Type other) {
        Assume.That(other.IsInterface, () => $"{nameof(other)} {other.Prettify()} must be an interface!");
        Asserter.Against(self)
                .And(it => it.Implements(other), Is.False, $"{self.Prettify()} does NOT implement {other.Prettify()}")
                .Invoke();
    }

    [Test]
    [TestCase(typeof(IEnumerable))]
    [TestCase(typeof(IEnumerable<>))]
    [TestCase(typeof(IEnumerable<int>))]
    [TestCase(typeof(IList))]
    [TestCase(typeof(IList<>))]
    [TestCase(typeof(IDictionary<,>))]
    [TestCase(typeof(IList<IDictionary<IList, int>>))]
    [TestCase(typeof(IList<IList<object>>))]
    [TestCase(typeof(IEnumerable<IEnumerable<object>>))]
    [TestCase(typeof(IList<ISuperList<SuperList<int>>>))]
    public void ImplementsSelf(Type self) {
        Console.WriteLine($"Type: {self.Prettify()}");
        Assume.That(self.IsInterface);
        Asserter.Against(self)
                .And(it => it.Implements(it), Is.True, $"{self.Prettify()} implements itself")
                .Invoke();
    }

    #endregion

    #region ToString

    class NoToString { }

    class HasToString {
        public override string ToString() {
            return $"I am an explicitly overridden ToString method. I was declared in {nameof(HasToString)}, but exist in {GetType().Prettify()}";
        }
    }

    class ChildOfString : HasToString { }

    class ToDoppelganger {
        public string ToString(int i) {
            return $"🦹 I tricked you! I am a {GetType().Prettify()}!";
        }
    }

    class Doppelface : IDoppelface {
        string IDoppelface.ToString() {
            return $"😈 I've fooled you once again! I am {GetType().Prettify()}!";
        }
    }

    class Newface {
        public new string ToString() {
            return $"🤖";
        }
    }

    interface IDoppelface {
        string ToString();
    }

    abstract class AbstractSelf {
        public abstract override string ToString();
    }

    abstract class IncorporealSelf : AbstractSelf {
        public override string ToString() => "👻";
    }

    class SpiritualSelf : IncorporealSelf { }

    class CorporealSelf : IncorporealSelf {
        public override string ToString() => "💪";
    }

    class ReifiedSelf : AbstractSelf {
        public override string ToString() {
            return "🧠 -> 🙋‍";
        }
    }

    [Test]
    [TestCase(typeof(object),          Should.BeNull)]
    [TestCase(typeof(NoToString),      Should.BeNull)]
    [TestCase(typeof(HasToString),     Should.BeNotNull)]
    [TestCase(typeof(ChildOfString),   Should.BeNotNull)]
    [TestCase(typeof(List<int>),       Should.BeNull)]
    [TestCase(typeof(IList<int>),      Should.BeNull)]
    [TestCase(typeof(ToDoppelganger),  Should.BeNull)]
    [TestCase(typeof(Doppelface),      Should.BeNull)]
    [TestCase(typeof(IDoppelface),     Should.BeNull)]
    [TestCase(typeof(Newface),         Should.BeNotNull)]
    [TestCase(typeof(AbstractSelf),    Should.BeNull)]
    [TestCase(typeof(ReifiedSelf),     Should.BeNotNull)]
    [TestCase(typeof(IncorporealSelf), Should.BeNotNull)]
    [TestCase(typeof(CorporealSelf),   Should.BeNotNull)]
    [TestCase(typeof(IncorporealSelf), Should.BeNotNull)]
    public void PersonalToStringMethod(Type type, Should should) {
        Assert.That(type.GetToStringOverride, should.Constrain());
    }

    #endregion

    #region Construction

    private class StartPrivately {
        public object Value           { get; }
        public Type   ConstructorType { get; }

        private StartPrivately(int i) {
            Value           = i;
            ConstructorType = typeof(int);
        }

        public StartPrivately(string s) {
            Value           = s;
            ConstructorType = typeof(string);
        }

        public StartPrivately(object o) {
            Value           = o;
            ConstructorType = typeof(object);
        }
    }

    [Test]
    [TestCase(5,        typeof(int))]
    [TestCase("yolo",   typeof(string))]
    [TestCase((short)5, typeof(int))]
    [TestCase(6L,       typeof(object))]
    public void UsePrivateConstructor(object value, Type constructorType) {
        Asserter.Against(() => typeof(StartPrivately).Construct(value))
                .And(Is.Not.Null)
                .And(Has.Property(nameof(StartPrivately.Value)).EqualTo(value))
                .And(Has.Property(nameof(StartPrivately.ConstructorType)).EqualTo(constructorType))
                .Invoke();
    }

    #endregion
}