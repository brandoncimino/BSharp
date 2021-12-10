using System;
using System.Reflection;

using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Standalone.Exceptions {
    public static class ExceptionUtils {
        private static readonly MethodInfo ModifyMessageMethod = typeof(ExceptionUtils).GetMethod(
            nameof(ModifyMessage_Internal),
            BindingFlags.Default |
            BindingFlags.Static  |
            BindingFlags.NonPublic
        );

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
        /// Applies the given <see cref="StringFilter"/>s to the <see cref="Exception.StackTrace"/> of <paramref name="exception"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="filter"></param>
        /// <param name="additionalFilters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] FilteredStackTrace<T>(this T exception, StringFilter filter, params StringFilter[] additionalFilters) where T : Exception {
            return StringUtils.CollapseLines(exception.StackTrace.SplitLines(), filter, additionalFilters);
        }
    }
}