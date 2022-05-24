using System;
using System.Reflection;

using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes {
    /// <summary>
    /// The parent class for <see cref="BSharp"/> <see cref="Attribute"/>s like <see cref="EditorInvocationButtonAttribute"/>.
    /// </summary>
    /// <remarks>
    /// <b>NOTE:</b> Even if an attribute only <b>affects</b> editor functionality (as is the case with <see cref="EditorInvocationButtonAttribute"/>), if it will <b>target</b> runtime code (which is most likely the case), then the <see cref="Attribute"/> class itself should be declared <b>inside <see cref="Packages.BrandonUtils.Runtime"/></b>.
    /// <br/>
    /// This mimics the setup of Unity's built-in attributes like <see cref="UnityEngine.HeaderAttribute"/> and <see cref="UnityEngine.RangeAttribute"/>, which are declared inside of <see cref="UnityEngine"/> rather than <see cref="UnityEditor"/>.
    /// </remarks>
    public abstract class BrandonAttribute : Attribute {
        /// <summary>
        /// Throws an <see cref="InvalidAttributeTargetException{T}"/> if this <see cref="BrandonAttribute"/> is attached to an invalid <see cref="MemberInfo"/>.
        /// </summary>
        /// <remarks>
        /// This can allow for "complex" validations, such as the number or types of parameters in <see cref="MethodBase.GetParameters"/>.
        /// <br/>
        /// For example, <see cref="EditorInvocationButtonAttribute"/> uses <see cref="EditorInvocationButtonAttribute.ValidateTarget"/> to ensure that <paramref name="target"/> has exactly 0 parameters.
        /// <p/>
        /// The primary use case for <see cref="ValidateTarget"/> is in an <a href="https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html">Editor.OnInspectorGUI</a> call, which should be triggered whenever the developer focuses on the Unity application.
        ///
        /// <p/>
        /// <b>📎 Protip:</b> <see cref="Type"/> is a <see cref="MemberInfo"/>, too!
        /// </remarks>
        /// <param name="target">the <see cref="MemberInfo"/> that this <see cref="BrandonAttribute"/> is applied to</param>
        /// <exception cref="InvalidAttributeTargetException{T}">If <paramref name="target"/> fails validation.</exception>
        [UsedImplicitly]
        public virtual void ValidateTarget(MemberInfo target) {
            // to be implemented by inheritors, if necessary
        }
    }
}