using System;
using System.Reflection;

using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    /// <summary>
    /// Creates a button in the Unity editor that will execute the annotated <see cref="AttributeTargets.Method"/>.
    /// </summary>
    /// <remarks>
    /// <li>The actual code that creates the button and invokes the method is located in the <c>BrandonUtils.Editor</c> namespace, in (as of 10/6/2020) <c>MonoBehaviourEditor</c> (which cannot be referenced here, because this is a non-Unity <see cref="BSharp"/> class).</li>
    /// <li>Works with both <see cref="BindingFlags.Public"/> and <see cref="BindingFlags.NonPublic"/> methods.</li>
    /// <li>Works with both <see cref="BindingFlags.Static"/> and <see cref="BindingFlags.Instance"/> methods.</li>
    /// <li>Works with both <see cref="BindingFlags.DeclaredOnly">Declared</see> and inherited methods.</li>
    /// <li>Works in both Editor and Play mode.</li>
    /// <li>While it is theoretically possible to pass values to the <c>MonoBehaviourEditor</c> invocation, it is currently unsupported.</li>
    /// <li><see cref="ValidateTarget_Hook"/> will throw an exception if the given <c>"MonoBehaviourEditor"</c> is invalid - which in this case would be anything that contains parameters.</li>
    /// </remarks>
    [MeansImplicitUse]
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorInvocationButtonAttribute : BrandonAttribute {
        public override void ValidateTarget_Hook(MemberInfo target) {
            var method = target.MustBe<MethodInfo>();
            var arity  = method.GetParameters().Length;
            if (arity == 0) {
                return;
            }

            throw this.RejectInvalidTarget(target, $"Methods annotated with {GetType().Name} must have exactly 0 parameters, but {target.Name} has {arity}!");
        }
    }
}