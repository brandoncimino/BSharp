using System;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;

using NUnit.Framework;

namespace BSharp.Tests.Reflection;

public class ReflectionEnumTests {
    class HasMembersOfAllTypes {
        public HasMembersOfAllTypes() { }

        public int B_Field = default;
        public int B_Prop     => B_Field;
        public int B_Method() => B_Field;
    }

    [Test]
    [TestCase(typeof(ConstructorInfo), 1)]
    [TestCase(typeof(MemberInfo),      4)]
    [TestCase(typeof(FieldInfo),       1)]
    [TestCase(typeof(PropertyInfo),    1)]
    [TestCase(typeof(MethodInfo),      1)]
    public void GetMemberInfoMemberTypes(Type memberInfoType, int expectedCount) {
        var memberTypeFlags = ReflectionUtils.GetMemberInfoMemberTypes(memberInfoType).MustNotBeNull();
        var foundMembers = typeof(HasMembersOfAllTypes).FindMembers(memberTypeFlags, (BindingFlags)int.MaxValue, (info, criteria) => true, default)
                                                       .Where(it => it.Name.StartsWith("B_") || it.MemberType == MemberTypes.Constructor)
                                                       .ToArray();

        Console.WriteLine($"Found {foundMembers.Length} members:");
        foreach (var m in foundMembers) {
            Console.WriteLine(m);
        }

        Assert.That(foundMembers, Has.Exactly(expectedCount).Items);
    }

    [Test]
    [TestCase(MemberTypes.Method,      1)]
    [TestCase(MemberTypes.Property,    1)]
    [TestCase(MemberTypes.Field,       1)]
    [TestCase(MemberTypes.Constructor, 1)]
    [TestCase(MemberTypes.All,         4)]
    public void GetMemberInfoBindingFlags(MemberTypes memberTypes, int expectedCount) {
        var bindingFlags = memberTypes.GetBindingFlags();
        Console.WriteLine($"{memberTypes.Prettify()} => {bindingFlags.Prettify()}");
        var found = typeof(HasMembersOfAllTypes).FindMembers(memberTypes, bindingFlags, (info, criteria) => true, default)
                                                .Where(it => it.Name.StartsWith("B_") || it.MemberType == MemberTypes.Constructor);
        Assert.That(found, Has.Exactly(expectedCount).Items);
    }
}