using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing;

public interface IRecursiveEqualityComparer : IEqualityComparer {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="xExpression">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="yExpression">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <returns></returns>
    /// <remarks>
    /// Does putting <see cref="CallerArgumentExpressionAttribute"/> on the interface actually do anything...?
    /// </remarks>
    public bool EqualsRecursively(
        object? x,
        object? y,
        [CallerArgumentExpression("x")]
        string? xExpression = default,
        [CallerArgumentExpression("y")]
        string? yExpression = default
    );
}

/// <summary>
/// TODO: Add tests, after I refactor the project (again)!
/// </summary>
internal class RecursiveMemberwiseEqualityComparator : EqualityComparer<object?>, IRecursiveEqualityComparer {
    private const int Default_Recursion_Limit = 20;
    private       int RecursionLimit;

    public RecursiveMemberwiseEqualityComparator(int recursionLimit = Default_Recursion_Limit) {
        RecursionLimit = recursionLimit;
    }

    public override bool Equals(object? x, object? y) => EqualsRecursively(x, y);

    public bool EqualsRecursively(
        object? x,
        object? y,
        string? xExpression = default,
        string? yExpression = default
    ) => EqualsRecursively(x, y, 0);

    private static bool EqualsRecursively(
        object? x,
        object? y,
        int     recursionCount
    ) {
        Console.WriteLine($"Comparing: [{recursionCount}]\n\t[{x?.GetType().Name}] {x}\n\t[{y?.GetType().Name}] {y}");

        recursionCount++;

        if (recursionCount > Default_Recursion_Limit) {
            throw new BrandonException($"BRANDON OVERFLOW EXCEPTION - recursion exceeded {nameof(Default_Recursion_Limit)} {Default_Recursion_Limit}");
        }

        // Attempt to compare using the default NUnitComparer first
        var tolerance = Tolerance.Default;
        if (new NUnitEqualityComparer().AreEqual(x, y, ref tolerance)) {
            return true;
        }

        var variables = x.GetType().GetVariables();
        return variables.All(
            it => EqualsRecursively(
                x.GetVariableValue<object>(it.Name),
                y.GetVariableValue<object>(it.Name),
                // VariableInfoExtensions.GetVariableValue(x, it.Name),
                // VariableInfoExtensions.GetVariableValue(y, it.Name),
                recursionCount
            )
        );
    }

    public override int GetHashCode(object? obj) {
        throw new NotImplementedException();
    }
}