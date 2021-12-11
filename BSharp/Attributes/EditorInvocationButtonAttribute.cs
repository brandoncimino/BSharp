using System;
using System.Reflection;

using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    /// <summary>
    /// Creates a button in the Unity editor that will execute the annotated <see cref="AttributeTargets.Method"/>.
    /// </summary>
    /// <remarks>
    /// <li>The actual code that creates the button and invokes the method is located in the <c>BrandonUtils.Editor</c> namespace, in (as of 10/6/2020) <c>MonoBehaviourEditor</c> (which cannot be referenced here, because this is a <see cref="Runtime"/> class).</li>
    /// <li>Works with both public and private methods.</li>
    /// <li>Works with both static and instance methods.</li>
    /// <li>Works with both declared and inherited methods.</li>
    /// <li>Works in both Editor and Play mode.</li>
    /// <li>While it is theoretically possible to pass values to the <c>MonoBehaviourEditor</c> invocation, it is currently unsupported.</li>
    /// <li><see cref="ValidateMethodInfo"/> will throw an exception if the given <c>"MonoBehaviourEditor"</c> is invalid - which in this case would be anything that contains parameters.</li>
    /// </remarks>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorInvocationButtonAttribute : BrandonAttribute {
        public override void ValidateMethodInfo(MethodInfo methodInfo) {
            if (methodInfo.GetParameters().Length != 0) {
                throw new InvalidAttributeTargetException<EditorInvocationButtonAttribute>(
                    $"Methods annotated with {nameof(EditorInvocationButtonAttribute)} must have exactly 0 parameters, but {methodInfo.Name} has {methodInfo.GetParameters().Length}!",
                    methodInfo
                );
            }
        }
    }
}