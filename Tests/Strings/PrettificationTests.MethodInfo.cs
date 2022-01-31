using System;
using System.Reflection;

using FowlFever.BSharp.Strings;

using NUnit.Framework;

namespace BSharp.Tests.Strings; 

public partial class PrettificationTests {
    public record MethodInfoExpectation(MethodInfo Method, string ExpectedString);

    public void VoidMethod() {
        Console.WriteLine("A method _so dark_, even the darkness calls it dark");
    }

    public static MethodInfoExpectation[] MethodInfoExpectations = new[] {
        new MethodInfoExpectation(new Func<string?>(() => "I am lambda-style Func<string>").Method, ""),
        new MethodInfoExpectation(typeof(PrettificationTests).GetMethod(nameof(VoidMethod))!,       "")
    };

    [Test]
    public static void PrettifyMethodInfo([ValueSource(nameof(MethodInfoExpectations))] MethodInfoExpectation expectation) {
        Console.WriteLine(expectation.Method.ReturnType);
        Console.WriteLine(expectation.Method.ReturnType.Prettify());
        Console.WriteLine(expectation.Method.ReturnParameter);
        Console.WriteLine(expectation.Method.ReturnParameter.Prettify());
    }
}