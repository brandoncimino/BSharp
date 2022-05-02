using System;
using System.Reflection;

using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions {
    public static class ExceptionUtils {
        private static readonly MethodInfo ModifyMessageMethod = typeof(ExceptionUtils).GetMethod(
            nameof(ModifyMessage_Internal),
            BindingFlags.Default |
            BindingFlags.Static  |
            BindingFlags.NonPublic
        ) ?? throw new ArgumentNullException();

        private static T ModifyMessage_Internal<T>(T exception, string newMessage) where T : Exception {
            try {
                return exception.InnerException == null ? ReflectionUtils.Construct<T>($"{newMessage}\n{exception.Message}") : ReflectionUtils.Construct<T>($"{newMessage}\n{exception.Message}", exception.InnerException);
            }
            catch (Exception e) {
                throw new BrandonException($"Couldn't modify the exception message!\n\tOriginal message: {exception.Message}\n\tNew message: {newMessage}", e);
            }
        }

        /// <summary>
        /// Returns a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> changed to <paramref name="newMessage"/>.
        /// </summary>
        /// <remarks>
        /// The new exception <i>will</i> maintain the actual type of the original <paramref name="exception"/>.
        /// </remarks>
        /// <param name="exception">the original <see cref="Exception"/></param>
        /// <param name="newMessage">the new <see cref="Exception.Message"/></param>
        /// <returns>a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> changed to <paramref name="newMessage"/></returns>
        public static Exception ModifyMessage(this Exception exception, string newMessage) {
            var genericModifyMessage = ModifyMessageMethod.MakeGenericMethod(exception.GetType());
            return genericModifyMessage.Invoke(null, new object[] { exception, newMessage }) as Exception;
        }

        /**
         * <inheritdoc cref="ModifyMessage"/>
         */
        public static T ModifyMessage<T>(this T exception, string newMessage) where T : Exception {
            return ModifyMessage_Internal(exception, newMessage);
        }

        /// <summary>
        /// Returns a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> prepended with <paramref name="additionalMessage"/>.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="ModifyMessage"/>
        /// </remarks>
        /// <param name="exception"><inheritdoc cref="ModifyMessage"/></param>
        /// <param name="additionalMessage">the string to prepend the original's <see cref="Exception.Message"/></param>
        /// <returns></returns>
        public static Exception PrependMessage(this Exception exception, string additionalMessage) {
            return ModifyMessage(exception, $"{additionalMessage}\n{exception.Message}");
        }

        /**
         * <inheritdoc cref="PrependMessage"/>
         */
        public static T PrependMessage<T>(this T exception, string additionalMessage) where T : Exception {
            return ModifyMessage_Internal(exception, $"{additionalMessage}\n{exception.Message}");
        }

        /// <summary>
        /// Generates a new <typeparamref name="T"/> exception with the given <see cref="Exception.Message"/> and optional <see cref="Exception.InnerException"/>. 
        /// </summary>
        /// <param name="message">the desired <see cref="Exception.Message"/></param>
        /// <param name="innerException">the desired <see cref="Exception.InnerException"/></param>
        /// <typeparam name="T">the type of <see cref="Exception"/></typeparam>
        /// <returns>a new <typeparamref name="T"/> instance</returns>
        /// <exception cref="BrandonException">If we couldn't construct a <see cref="T"/> (likely because it lacked an appropriate <see cref="ConstructorInfo"/>)</exception>
        /// <remarks>I would rather construct a parameterless <typeparamref name="T"/> and then modify its properties, rather than relying on specific constructor signatures to be present,
        /// but unfortunately we can't, because <see cref="Exception"/> and <see cref="Exception.InnerException"/> don't have setters.</remarks>
        /// TODO: utilize this in <see cref="ModifyMessage"/>, etc.ce
        public static T ConstructException<T>(string message, Exception? innerException = default) where T : Exception {
            try {
                return innerException == null
                           ? ReflectionUtils.Construct<T>(message)
                           : ReflectionUtils.Construct<T>(message, innerException);
            }
            catch (Exception e) {
                throw new BrandonException($"Unable to build an exception of type {typeof(T)} using {{message: '{message}', innerException: {innerException}", e);
            }
        }

        /// <summary>
        /// Applies the given <see cref="StringFilter"/>s to the <see cref="Exception.StackTrace"/> of <paramref name="exception"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="filter"></param>
        /// <param name="additionalFilters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string?[] FilteredStackTrace<T>(this T exception, StringFilter filter, params StringFilter[] additionalFilters) where T : Exception {
            return StringUtils.CollapseLines(exception.StackTrace.SplitLines(), filter, additionalFilters);
        }
    }
}