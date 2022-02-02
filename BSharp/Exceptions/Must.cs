using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using BenchmarkDotNet.Jobs;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.BSharp.Strings.Tabler;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis.Operations;

using Pure = System.Diagnostics.Contracts.PureAttribute;
namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// Methods for validating arguments and generating <see cref="Exception"/>s.
/// </summary>
/// <remarks>
/// Each of these methods should return the original value if their validation succeeds.
/// <br/>
/// <see cref="Exception"/> objects should be created by <see cref="RejectArgument"/> methods.
/// </remarks>
[PublicAPI]
public static class Must {
    #region Arbitration

    public static T Satisfy<T>(T? actualValue, Func<T,bool> predicate, string parameterName, string methodName, string? reason = default) {
        return Satisfy(new ArgInfo<T>(actualValue!, parameterName, methodName), predicate, reason);
    }

    public static T Satisfy<T>(ArgInfo<T> argInfo, Func<T, bool> predicate, string? reason = default) {
        Exception? exc = default;

        try {
            if (predicate(argInfo.ActualValue)) {
                return argInfo.ActualValue;
            }
        }
        catch (Exception e) {
            exc = e;
        }

        reason ??= $"Predicate {InnerPretty.PrettifyDelegate(predicate, PrettificationSettings.Default)} failed!";

        throw new ArgumentException(argInfo.GetMessage(reason), exc);
    }

    #endregion

    #region Truthfullness

    public static T BeTrue<T>(T? actualValue, string parameterName, string methodName) {
        throw new NotImplementedException();
    }

    public static T BeFalse<T>(T? actualValue, string parameterName, string methodName) {
        throw new NotImplementedException();
    }

    #endregion

    #region Nullity

    public static T NotBeNull<T>(T? actualValue, string parameterName, string methodName) {
        return NotBeNull(new ArgInfo<T?>(actualValue, parameterName, methodName));
    }

    public static T NotBeNull<T>(ArgInfo<T?> argInfo) {
        return argInfo.ActualValue ?? throw argInfo.WasNull();
    }

    public static T? BeNull<T>(T? actualValue, string parameterName, string methodName) {
        return BeNull(new ArgInfo<T?>(actualValue, parameterName, methodName));
    }

    public static T? BeNull<T>(ArgInfo<T?> argInfo) {
        if (argInfo.ActualValue == null) {
            return argInfo.ActualValue;
        }

        throw argInfo.WasNull();
    }

    #endregion

    #region Numbers

    public static T BeANumericValue<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNumber() == true) {
            return actualValue;
        }

        throw RejectArgument.WasNotANumericValue(actualValue, parameterName, methodName);
    }

    [NonNegativeValue]
    public static T BePositive<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotPositive(actualValue, parameterName, methodName);
    }

    [NonNegativeValue]
    public static T BeStrictlyPositive<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsStrictlyPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotStrictlyPositive(actualValue, parameterName, methodName);
    }

    public static T BeNegative<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsNegative(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotNegative(actualValue, parameterName, methodName);
    }

    #endregion

    #region Strings

    /// <summary>
    ///
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="parameterName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string NotBeBlank(string? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNotBlank() == true) {
            return actualValue;
        }

        throw RejectArgument.WasBlank(actualValue, parameterName, methodName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="parameterName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string NotBeEmpty(string? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNotEmpty() == true) {
            return actualValue;
        }

        throw RejectArgument.WasEmpty(actualValue, parameterName, methodName);
    }

    #region NotContain

    public static string NotContain(
        string? actualValue,
        string  unwantedString,
        string  parameterName,
        string  methodName
    ) => NotContain(new ArgInfo<string?>(actualValue, parameterName, methodName), unwantedString);

    public static string NotContain(ArgInfo<string?> argInfo, string unwantedString) {
        if (argInfo.ActualValue?.Contains(unwantedString) == false) {
            return argInfo.ActualValue;
        }

        throw argInfo.GetException($"Contained the substring \"{unwantedString}\"!");
    }

    #endregion

    #region NotMatch

    public static string NotMatch(
        string? actualValue,
        Regex   pattern,
        string  parameterName,
        string  methodName
    ) => NotMatch(new ArgInfo<string?>(actualValue, parameterName, methodName), pattern);

    public static string NotMatch(ArgInfo<string?> argInfo, Regex pattern) {
        if (argInfo.ActualValue?.Matches(pattern) == false) {
            return argInfo.ActualValue;
        }

        throw argInfo.GetException($"Matched the Regex pattern /{pattern}/!");
    }

    #endregion

    #region Contain

    public static string Contain(
        string? actualValue,
        string  substring,
        string  parameterName,
        string  methodName
    ) => Contain(new ArgInfo<string?>(actualValue, parameterName, methodName), substring);

    public static string Contain(ArgInfo<string?> argInfo, string substring) {
        if (argInfo.ActualValue?.Contains(substring) == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.GetException($"Didn't contain the substring \"{substring}\"");
    }

    #endregion

    #region Match

    public static string Match(
        string? actualValue,
        Regex   pattern,
        string  parameterName,
        string  methodName
    ) => Match(new ArgInfo<string?>(actualValue, parameterName, methodName), pattern);

    public static string Match(ArgInfo<string?> argInfo, Regex pattern) {
        if (argInfo.ActualValue?.Matches(pattern) == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.GetException($"Didn't match the Regex pattern /{pattern}/!");
    }

    #endregion

    #endregion

    #region Files

    #region Exist

    public static T Exist<T>(T fileSystemInfo, string parameterName, string methodName)
        where T : FileSystemInfo? {
        return Exist(new ArgInfo<T>(fileSystemInfo, parameterName, methodName));
    }

    public static T Exist<T>(ArgInfo<T> argInfo) where T : FileSystemInfo? {
        if (argInfo.ActualValue?.Exists == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.DidNotExist();
    }

    #endregion

    #region NotBeEmpty

    public static FileInfo NotBeEmpty(ArgInfo<FileInfo?> argInfo) {
        if (argInfo.ActualValue?.IsNotEmpty() == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.HadLengthZero();
    }

    public static FileInfo NotBeEmpty(FileInfo? actualValue, string parameterName, string methodName) => NotBeEmpty(new ArgInfo<FileInfo?>(actualValue, parameterName, methodName));

    public static DirectoryInfo NotBeEmpty(ArgInfo<DirectoryInfo?> argInfo) {
        if (argInfo.ActualValue?.IsNotEmpty() == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.HadNoContents();
    }

    public static DirectoryInfo NotBeEmpty(DirectoryInfo? actualValue, string parameterName, string methodName) => NotBeEmpty(new ArgInfo<DirectoryInfo?>(actualValue, parameterName, methodName));

    #endregion

    #region BeEmptyOrMissing

    public static FileInfo BeEmpty(ArgInfo<FileInfo> argInfo) {
        if (argInfo.ActualValue.IsEmptyOrMissing()) {
            return argInfo.ActualValue;
        }

        throw argInfo.WasNotEmpty();
    }

    public static FileInfo BeEmpty(FileInfo actualValue, string parameterName, string methodName) => BeEmpty(new ArgInfo<FileInfo>(actualValue, parameterName, methodName));

    public static DirectoryInfo BeEmpty(ArgInfo<DirectoryInfo> argInfo) {
        if (argInfo.ActualValue.IsEmptyOrMissing()) {
            return argInfo.ActualValue;
        }

        throw argInfo.WasNotEmpty();
    }

    public static DirectoryInfo BeEmpty(DirectoryInfo actualValue, string parameterName, string methodName) => BeEmpty(new ArgInfo<DirectoryInfo>(actualValue, parameterName, methodName));

        #endregion

    #region NotExist

    public static T NotExist<T>(ArgInfo<T> argInfo) where T : FileSystemInfo? {
        if (argInfo.ActualValue?.Exists != true) {
            return argInfo.ActualValue;
        }

        throw argInfo.AlreadyExisted();
    }

    public static T NotExist<T>(T fileSystemInfo, string parameterName, string methodName) where T: FileSystemInfo? => NotExist(new ArgInfo<T>(fileSystemInfo, parameterName, methodName));

    #endregion

    #endregion
}

public record ArgInfo<T>(T ActualValue, string ParameterName, string MethodName) {
    private const string Icon = "🚮";

    public string GetPreamble() {
        var    typeName    = typeof(T).Name;
        var paramString = typeName.Equals(ParameterName, StringComparison.OrdinalIgnoreCase)
                              ? $"[{ParameterName}]"
                              : $"[{typeName}]{ParameterName}";
        return $"{Icon} {MethodName}() 🙅 {paramString}";
    }

    public string GetMessage(string reason) {
        reason = reason.IfBlank("<reason not specified 🤷>");
        return $"{GetPreamble()}: {reason}";
    }

    public ArgumentException GetException(string reason) {
        return new ArgumentException(GetMessage(reason), ParameterName);
    }
}

internal static class RejectArgument {
    private const string Icon = "🚮";

    private static string GetPreamble<T>(T? actualValue, string parameterName, string methodName) {
        return $"{Icon} {methodName} rejected parameter [{typeof(T).Name}] {parameterName}";
    }

    private static string GetMessage<T>(
        T?     actualValue,
        string parameterName,
        string methodName,
        string reason
    ) {
        reason = reason.IfBlank("<reason not specified 🤷>");
        return $"{GetPreamble(actualValue, parameterName, methodName)}: {reason}";
    }

    #region Nullity

    public static ArgumentNullException WasNull<T>(this ArgInfo<T> argInfo) {
        return new ArgumentNullException(argInfo.ParameterName, argInfo.GetPreamble());
    }

    public static ArgumentException WasNotNull<T>(this ArgInfo<T> argInfo) {
        return new ArgumentException(argInfo.GetMessage(nameof(WasNotNull)));
    }

    #endregion

    #region Numbers

    [Pure]
    public static ArgumentOutOfRangeException WasNotPositive<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be positive (x ≥ 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotStrictlyPositive<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be strictly positive (x > 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotNegative<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be negative (x < 0)!"
        );
    }

    [Pure]
    public static ArgumentException WasNotANumericValue<T>(T? actualValue, string parameterName, string methodName) {
        var preamble = GetPreamble(actualValue, parameterName, methodName);
        var message  = $"{preamble}: [{typeof(T).Prettify()}]{actualValue} was not a numeric value!";
        return new ArgumentException(message, parameterName);
    }

    #endregion

    #region Switches

    [Pure]
    public static ArgumentException UnhandledSwitchBranch<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentException(
            GetMessage(
                actualValue,
                parameterName,
                methodName,
                "Value was unhandled by any switch branch!"
            )
        );
    }

    [Pure]
    public static ArgumentException UnhandledSwitchType<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentException(
            GetMessage(
                actualValue,
                parameterName,
                methodName,
                "Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!"
            )
        );
    }

    [Pure]
    public static InvalidEnumArgumentException UnhandledSwitchEnum<T>(
        T? actualValue,
        string parameterName,
        string methodName
    ) where T : Enum {
        return new InvalidEnumArgumentException(
            GetMessage(
                actualValue,
                parameterName,
                methodName,
                $"The {typeof(T).Name} value [{actualValue.OrNullPlaceholder()}] was not handled by any branches of the switch statement!"
            )
        );
    }

    #endregion

    #region Strings

    [Pure]
    public static ArgumentException WasBlank(string? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Must not be blank (null, empty, or whitespace)!");
    }

    [Pure]
    public static ArgumentException WasEmpty(string? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Must not be null or an empty string!");
    }

    #endregion

    #region Files

    public static FileNotFoundException DidNotExist<T>(this ArgInfo<T> argInfo) where T : FileSystemInfo? {
        return new FileNotFoundException(argInfo.GetMessage(nameof(DidNotExist)));
    }

    public static IOException AlreadyExisted<T>(this ArgInfo<T> argInfo)
        where T : FileSystemInfo? {
        return new IOException(argInfo.GetMessage(nameof(AlreadyExisted)));
    }

    #region WasEmpty

    public static IOException HadLengthZero(this ArgInfo<FileInfo?> argInfo) {
        return new IOException(argInfo.GetMessage($"Had a {nameof(FileInfo)}.{nameof(FileInfo.Length)}"));
    }

    public static IOException HadNoContents(this ArgInfo<DirectoryInfo?> argInfo) {
        return new IOException(argInfo.GetMessage($"{argInfo.ActualValue?.GetType().Name ?? nameof(DirectoryInfo)} didn't contain any files or subdirectories!"));
    }

    #endregion

    #region WasNotEmpty

    public static IOException WasNotEmpty(this ArgInfo<FileInfo> argInfo) {
        return new IOException(argInfo.GetMessage($"Existed and had a {nameof(FileInfo.Length)} > 0!"));
    }

    public static IOException WasNotEmpty(this ArgInfo<DirectoryInfo> argInfo) {
        return new IOException(argInfo.GetMessage($"Existed and contained files or sub-directories!"));
    }

    #endregion
    #endregion
}