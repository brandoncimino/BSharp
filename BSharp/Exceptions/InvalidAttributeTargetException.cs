using System;
using System.Reflection;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions {
    public class InvalidAttributeTargetException<T> : BrandonException {
        private readonly MemberInfo BadTarget;
        public override  string     BaseMessage => $"The custom attribute {typeof(T).Name} was set to an invalid target, {StringUtils.FormatMember(BadTarget)}!";

        public InvalidAttributeTargetException() { }

        public InvalidAttributeTargetException(string message, MemberInfo badTarget, Exception innerException = null) : base(message, innerException) {
            BadTarget = badTarget;
        }
    }
}