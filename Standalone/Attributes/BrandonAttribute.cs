using System;
using System.Reflection;

using BrandonUtils.Standalone.Exceptions;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Attributes {
    /// <summary>
    /// The parent class for <see cref="BrandonUtils"/> <see cref="Attribute"/>s like <see cref="EditorInvocationButtonAttribute"/>.
    /// </summary>
    /// <remarks>
    /// <b>NOTE:</b> Even if an attribute only <b>affects</b> editor functionality (as is the case with <see cref="EditorInvocationButtonAttribute"/>), if it will <b>target</b> runtime code (which is most likely the case), then the <see cref="Attribute"/> class itself should be declared <b>inside <see cref="Packages.BrandonUtils.Runtime"/></b>.
    /// <br/>
    /// This mimics the setup of Unity's built-in attributes like <see cref="UnityEngine.HeaderAttribute"/> and <see cref="UnityEngine.RangeAttribute"/>, which are declared inside of <see cref="UnityEngine"/> rather than <see cref="UnityEditor"/>.
    /// </remarks>
    public abstract class BrandonAttribute : Attribute {
        /// <summary>
        /// Returns whether or not this <see cref="BrandonAttribute"/> is attached to a valid <see cref="MethodInfo"/>.
        /// </summary>
        /// <remarks>
        /// This an allow for "complex" validations, such as the number or types of parameters in <paramref name="methodInfo"/>.
        /// <br/>
        /// For example, <see cref="EditorInvocationButtonAttribute"/> uses <see cref="EditorInvocationButtonAttribute.ValidateMethodInfo"/> to ensure that <paramref name="methodInfo"/> has exactly 0 parameters.
        /// <p/>
        /// The primary use case for <see cref="ValidateMethodInfo"/> is in an <a href="https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html">Editor.OnInspectorGUI</a> call, which should be triggered whenever the developer focuses on the Unity application.
        /// </remarks>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAttributeTargetException{T}">If <paramref name="methodInfo"/> fails validation.</exception>
        [UsedImplicitly]
        public virtual void ValidateMethodInfo(MethodInfo methodInfo) {
            // to be implemented by inheritors, if necessary
        }
    }
}