using System;
using System.Collections;
using System.Linq;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// TODO: Add tests, after I refactor the project (again)!
    /// </summary>
    public class RecursiveMemberwiseEqualityComparator : IEqualityComparer {
        private const int Default_Recursion_Limit = 20;
        private       int RecursionLimit;

        public RecursiveMemberwiseEqualityComparator(int recursionLimit = Default_Recursion_Limit) {
            RecursionLimit = recursionLimit;
        }

        public bool Equals(object x, object y) {
            return EqualsRecursively(x, y);
        }

        private static bool EqualsRecursively(object x, object y, int recursionCount = 0) {
            Console.WriteLine($"Comparing: [{recursionCount}]\n\t[{x.GetType().Name}] {x}\n\t[{y.GetType().Name}] {y}");

            recursionCount++;

            if (recursionCount > Default_Recursion_Limit) {
                throw new BrandonException($"BRANDON OVERFLOW EXCEPTION - recursion exceeded {nameof(Default_Recursion_Limit)} {Default_Recursion_Limit}");
            }

            // Attempt to compare using the default NUnitComparer first
            var tolerance = Tolerance.Default;
            if (NUnitEqualityComparer.Default.AreEqual(x, y, ref tolerance)) {
                return true;
            }

            var variables = x.GetType().GetVariables();
            return variables.All(
                it => EqualsRecursively(
                    ReflectionUtils.GetVariableValue(x, it.Name),
                    ReflectionUtils.GetVariableValue(y, it.Name),
                    recursionCount
                )
            );
        }

        public int GetHashCode(object obj) {
            return base.GetHashCode();
        }
    }
}