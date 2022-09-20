using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public static class Exceptional {
    #region ConstructException

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
    [PublicAPI]
    public static T ConstructException<T>(string message, Exception? innerException = default)
        where T : Exception {
        return _ConstructException_Internal<T>(message, innerException);
    }

    private static T _ConstructException_Internal<T>(string message, Exception? innerException)
        where T : Exception {
        try {
            return innerException == null
                       ? ReflectionUtils.Construct<T>(message)
                       : ReflectionUtils.Construct<T>(message, innerException);
        }
        catch (Exception e) {
            throw new BrandonException($"Unable to build an exception of type {typeof(T)} using {{message: '{message}', innerException: {innerException?.ToString() ?? "⛔"}}}", e);
        }
    }

    private static readonly MethodInfo ConstructExceptionMethod = typeof(ReflectionUtils).GetMethod(
                                                                      nameof(_ConstructException_Internal),
                                                                      BindingFlags.Default |
                                                                      BindingFlags.Static  |
                                                                      BindingFlags.NonPublic
                                                                  ) ??
                                                                  throw new ArgumentNullException();

    /// <summary>
    /// Constructs a new <see cref="Exception"/> of the desired type.
    /// </summary>
    /// <param name="exceptionType">the desired <see cref="Exception"/> type</param>
    /// <param name="message">the desired <see cref="Exception.Message"/></param>
    /// <param name="innerException">an optional <see cref="Exception.InnerException"/></param>
    /// <returns>a new <see cref="Exception"/></returns>
    [PublicAPI]
    public static Exception ConstructException(Type exceptionType, string message, Exception? innerException = default) {
        var genericMethod = ConstructExceptionMethod.MakeGenericMethod(exceptionType);
        return (Exception)genericMethod.Invoke(null, new object?[] { message, innerException });
    }

    #endregion

    #region ModifyMessage

    /// <summary>
    /// Returns a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> changed to <paramref name="newMessage"/>.
    /// </summary>
    /// <remarks>
    /// The new exception <i>will</i> maintain the actual type of the original <paramref name="exception"/>.
    /// </remarks>
    /// <param name="exception">the original <see cref="Exception"/></param>
    /// <param name="newMessage">the new <see cref="Exception.Message"/></param>
    /// <returns>a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> changed to <paramref name="newMessage"/></returns>
    public static Exception ModifyMessage(this Exception exception, string newMessage) => ConstructException(exception.GetType(), newMessage, exception.InnerException);

    /// <inheritdoc cref="ModifyMessage(System.Exception,string)"/>
    public static Exception ModifyMessage(this Exception exception, Func<string, string> modification) => ConstructException(exception.GetType(), modification(exception.Message), exception.InnerException);

    /// <inheritdoc cref="ModifyMessage(System.Exception,string)"/>
    public static T ModifyMessage<T>(this T exception, string newMessage)
        where T : Exception => ConstructException<T>(newMessage, exception.InnerException);

    /// <inheritdoc cref="ModifyMessage(System.Exception,string)"/>
    public static T ModifyMessage<T>(this T exception, Func<string, string> modification)
        where T : Exception => ConstructException<T>(modification(exception.Message), exception.InnerException);

    #endregion

    #region PrependMessage

    /// <summary>
    /// Returns a new copy of <paramref name="exception"/> with the <see cref="Exception.Message"/> prepended with <paramref name="additionalMessage"/>.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="ModifyMessage(System.Exception,string)"/>
    /// </remarks>
    /// <param name="exception"><inheritdoc cref="ModifyMessage(System.Exception,string)"/></param>
    /// <param name="additionalMessage">the string to prepend the original's <see cref="Exception.Message"/></param>
    /// <param name="joiner">a <see cref="string"/> interposed betwixt the original and <paramref name="additionalMessage"/>s</param>
    /// <returns>a new <see cref="Exception"/></returns>
    public static Exception PrependMessage(this Exception exception, string additionalMessage, string joiner = "\n") => ModifyMessage(exception, $"{additionalMessage}{joiner}{exception.Message}");

    ///<inheritdoc cref="PrependMessage"/>
    public static T PrependMessage<T>(this T exception, string additionalMessage, string joiner = "\n")
        where T : Exception => exception.ModifyMessage($"{additionalMessage}{joiner}{exception.Message}");

    #endregion

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

    /// <summary>
    /// Combines all of the non-null <see cref="Exception"/>s into an <see cref="AggregateException"/>. 
    /// </summary>
    /// <param name="exceptions">a bunch of <see cref="Exception"/>s</param>
    /// <returns>a new <see cref="AggregateException"/>, if there were any non-<c>null</c> <see cref="Exception"/>s; otherwise, <c>null</c></returns>
    public static AggregateException? Aggregate(IEnumerable<Exception?>? exceptions) {
        var exc = exceptions.NonNull().ToArray();
        return exc.IsEmpty() ? null : new AggregateException();
    }

    /// <inheritdoc cref="Aggregate(System.Collections.Generic.IEnumerable{System.Exception?}?)"/>
    public static AggregateException? Aggregate(params Exception?[] exceptions) => Aggregate(exceptions.AsEnumerable());

    public static void SetData<T>(this Exception exception, T value, [CallerMemberName] string? _key = default) {
        if (_key == null) {
            throw new ArgumentNullException(nameof(_key));
        }

        // exception.Data[_key]
    }
}