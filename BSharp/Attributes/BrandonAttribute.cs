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
        /// <remarks>
        /// This is the overridable portion of <see cref="ValidateTarget"/>, which will catch and re-throw any <see cref="Exception"/>s thrown by <see cref="ValidateTarget_Hook"/>.
        /// </remarks>
        /// <inheritdoc cref="ValidateTarget"/>
        [UsedImplicitly]
        public virtual void ValidateTarget_Hook(MemberInfo target) {
            // to be implemented by inheritors, if necessary
        }

        /// <summary>
        /// Throws an <see cref="InvalidAttributeTargetException"/> if this <see cref="BrandonAttribute"/> is attached to an invalid <see cref="MemberInfo"/>.
        /// <p/>
        /// This enables "complex" validations, such as the number or types of <see cref="MethodBase.GetParameters"/>.
        /// </summary>
        /// <remarks>
        /// This method will re-throw any <see cref="Exception"/> raised by <see cref="ValidateTarget_Hook"/> as an <see cref="InvalidAttributeTargetException"/>.
        /// <p/>
        /// 
        /// The primary use case for <see cref="ValidateTarget"/> is in an <a href="https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html">Editor.OnInspectorGUI</a> call, which should be triggered whenever the developer focuses on the Unity application.
        ///
        /// <p/>
        /// <b>📎 Protip:</b> <see cref="Type"/> is a <see cref="MemberInfo"/>, too!
        /// </remarks>
        /// <param name="target">the <see cref="MemberInfo"/> that this <see cref="BrandonAttribute"/> is attached to</param>
        /// <exception cref="InvalidAttributeTargetException">If <paramref name="target"/> fails validation.</exception>
        /// <seealso cref="ValidateTarget_Hook"/>
        [UsedImplicitly]
        public void ValidateTarget(MemberInfo target) {
            try {
                ValidateTarget_Hook(target);
            }
            catch (Exception e) {
                this.RejectInvalidTarget(target, innerException: e);
            }
        }
    }
}