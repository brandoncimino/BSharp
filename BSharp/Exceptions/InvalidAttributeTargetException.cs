using System;
using System.Reflection;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Exceptions {
    public class InvalidAttributeTargetException : BrandonException {
        private readonly   Type?       AttributeType;
        private readonly   MemberInfo? BadTarget;
        private            string      AttributeName => AttributeType?.Name ?? "<attribute type not provided 🤷‍>";
        protected override string      BaseMessage   => $"The custom attribute {AttributeName} was set to an invalid target, {BadTarget?.ToString() ?? "<target not provided 🤷‍>"}!";

        public InvalidAttributeTargetException(string? message = default, Exception? innerException = default) : base(message, innerException) { }

        public InvalidAttributeTargetException(
            Type       attributeType,
            MemberInfo badTarget,
            string?    message,
            Exception? innerException
        ) : base(message, innerException) {
            AttributeType = attributeType;
            BadTarget     = badTarget;
        }
    }
}

public static class InvalidAttributeTargetExceptionExtensions {
    public static InvalidAttributeTargetException RejectInvalidTarget(
        this Attribute attribute,
        MemberInfo     badTarget,
        string?        message        = default,
        Exception?     innerException = default
    ) {
        return new InvalidAttributeTargetException(attribute.GetType(), badTarget, message, innerException);
    }
}