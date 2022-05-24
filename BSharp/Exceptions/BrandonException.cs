using System;
using System.Runtime.Serialization;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions {
    [PublicAPI]
    public class BrandonException : SystemException {
        /// <summary>
        /// The class-defined "default" message for this <see cref="Exception"/> type.
        /// </summary>
        /// <remarks>
        /// This value shouldn't depend on the "message" parameter passed to constructors.
        /// </remarks>
        /// <seealso cref="CustomMessage"/>
        protected virtual string BaseMessage { get; } = "This was probably Brandon's fault. For support, call 203-481-1845.";
        /// <summary>
        /// The caller-defined portion of the <see cref="Message"/>, which should be set constructor.
        /// </summary>
        protected string? CustomMessage { get; init; }

        /// <inheritdoc cref="Exception.Message"/>
        /// <remarks>
        /// <see cref="BrandonException"/> implementations cannot <c>override</c> this directly. They should instead use <see cref="BaseMessage"/> and <see cref="CustomMessage"/>.
        /// </remarks>
        public sealed override string Message {
            get {
                return new[] {
                    CustomMessage,
                    "",
                    BaseMessage,
                }.JoinLines();
            }
        }

        public BrandonException(string? message = default, Exception? innerException = null) : base(message, innerException) {
            CustomMessage = message;
        }

        protected BrandonException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}