using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Reflection {
    public static class ReflectionException {
        private static readonly PrettificationSettings PrettificationSettings = TypeNameStyle.Full;

        internal static ArgumentException WrongMemberTypeException(
            MemberInfo  badMember,
            MemberTypes expectedTypes,
            [CallerArgumentExpression("badMember")]
            string? paramName = default,
            Exception? innerException = default
        ) {
            return new ArgumentException($"{badMember.Prettify(PrettificationSettings)} isn't a {expectedTypes.Prettify(PrettificationSettings)}!", paramName, innerException);
        }

        internal static MissingMemberException VariableNotFoundException(Type type, string variableName, Exception? innerException = null) {
            return new MissingMemberException($"The {nameof(type)} {type} did not have a field or property named {variableName}!", innerException);
        }

        internal static NullReferenceException NoOwningTypeException(MemberInfo memberInfo) {
            return new NullReferenceException($"Somehow, {memberInfo.Prettify()} doesn't have a {nameof(MemberInfo.ReflectedType)} OR {nameof(MemberInfo.DeclaringType)}...!");
        }

        internal static MissingFieldException NoAutoPropertyBackingFieldException(Type owningType, string propertyName) {
            return new MissingFieldException($"The type {owningType.Prettify(PrettificationSettings)} doesn't contain an auto-property backing field that matches the property [{propertyName}]!");
        }

        internal static ArgumentNullException NoDeclaringTypeException(MemberInfo orphanMember, [CallerArgumentExpression("orphanMember")] string? paramName = default) {
            return new ArgumentNullException(paramName, $"[{orphanMember.GetType()}] {orphanMember} doesn't have a {nameof(orphanMember.DeclaringType)}!");
        }

        internal static ArgumentNullException NoReflectedTypeException(MemberInfo orphanMember, [CallerArgumentExpression("orphanMember")] string? paramName = default) {
            return new ArgumentNullException(paramName, $"[{orphanMember.GetType()}] {orphanMember} doesn't have a {nameof(orphanMember.ReflectedType)}!");
        }
    }
}