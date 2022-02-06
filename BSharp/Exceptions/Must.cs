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
using FowlFever.Conjugal.Affixing;

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
    public static ArgInfo<T> GetArgInfo<T>(T argInfo, string parameterName, string methodName) => new ArgInfo<T>(argInfo, parameterName, methodName);

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

        throw new ArgumentException(argInfo.WithReason(reason).GetLongMessage(), exc);
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

    #region NotContain (string, strings)

    public static string NotContain(
        string? actualValue,
        string  unwantedString,
        string  parameterName,
        string  methodName
    ) => NotContain(new ArgInfo<string?>(actualValue, parameterName, methodName), unwantedString);

    public static string NotContain(ArgInfo<string?> argInfo, string unwantedString) {
        if (argInfo.ActualValue?.DoesNotContain(unwantedString) == true) {
            return argInfo.ActualValue;
        }

        throw argInfo.GetException($"Contained the substring \"{unwantedString}\"!");
    }

    public static string NotContain(ArgInfo<string?> argInfo, IEnumerable<string> unwantedStrings) {
        NotBeNull(argInfo);

        var badStrings = unwantedStrings.Where(it => argInfo.ActualValue?.Contains(it) == true)
                                        .JoinString(", ");

        if (badStrings.IsNotEmpty()) {
            throw argInfo.GetException(
                $"Contained disallowed substrings: [{badStrings}]"
            );
        }

        return argInfo.ActualValue!;
    }

    #region NotContain (string, chars)

    public static string NotContain(
        string?           actualValue,
        IEnumerable<char> unwantedChars,
        string            parameterName,
        string            methodName
    ) => NotContain(new ArgInfo<string?>(actualValue, parameterName, methodName), unwantedChars);

    public static string NotContain(ArgInfo<string?> argInfo, IEnumerable<char> unwantedChars) {
        NotBeNull(argInfo);

        var badChars = unwantedChars.Where(it => argInfo.ActualValue?.Contains(it) == true).JoinString(", ");

        if (badChars.IsNotEmpty()) {
            throw argInfo.GetException($"Contained disallowed substrings: [{badChars}]");
        }

        return argInfo.ActualValue!;
    }

    #endregion

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

    #region Contain (string, substring)

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

    #region ExistWithContent

    public static FileInfo ExistWithContent(FileInfo? actualValue, string parameterName, string methodName) => ExistWithContent(new ArgInfo<FileInfo?>(actualValue, parameterName, methodName));

    public static FileInfo ExistWithContent(ArgInfo<FileInfo?> argInfo) {
        if (argInfo is { ActualValue: { Exists: true, Length: > 0 } }) {
            return argInfo.ActualValue;
        }

        string[] additionalReasons = new[] { "Did not exist OR was empty!" };
        throw new FileNotFoundException(argInfo.WithReason(additionalReasons).GetLongMessage());
    }

    #endregion

    #endregion

    #region NotBeEmpty

    public static FileInfo NotBeEmpty(ArgInfo<FileInfo?> argInfo) {
        if (argInfo.ActualValue?.ExistsWithContent() == true) {
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

    public static FileInfo BeEmptyOrMissing(ArgInfo<FileInfo> argInfo) {
        if (argInfo.ActualValue.IsEmptyOrMissing()) {
            return argInfo.ActualValue;
        }

        throw argInfo.WasNotEmpty();
    }

    public static FileInfo BeEmptyOrMissing(FileInfo actualValue, string parameterName, string methodName) => BeEmptyOrMissing(new ArgInfo<FileInfo>(actualValue, parameterName, methodName));

    public static DirectoryInfo BeEmptyOrMissing(ArgInfo<DirectoryInfo> argInfo) {
        if (argInfo.ActualValue.IsEmptyOrMissing()) {
            return argInfo.ActualValue;
        }

        throw argInfo.WasNotEmpty();
    }

    public static DirectoryInfo BeEmptyOrMissing(DirectoryInfo actualValue, string parameterName, string methodName) => BeEmptyOrMissing(new ArgInfo<DirectoryInfo>(actualValue, parameterName, methodName));

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

    #region Collections

    #region Contain (Index)

    public static T ContainIndex<T>(
        ArgInfo<T> argInfo,
        int       requiredIndex
    ) where T : ICollection {
        Must.BePositive(requiredIndex, nameof(requiredIndex), nameof(ContainIndex));
        var size = argInfo.ActualValue?.Count;
        if (requiredIndex >= 0 && requiredIndex < size) {
            return argInfo.ActualValue!;
        }

        throw new ArgumentOutOfRangeException(argInfo.ParameterName, argInfo.ActualValue, argInfo.WithReason($"Collection of size {size} did not contain the index {requiredIndex}!").GetLongMessage());
    }

    public static T ContainIndex<T>(
        ArgInfo<T?> argInfo,
        Index      index
    ) where T : ICollection {
        NotBeNull(argInfo);
        var count = argInfo.ActualValue!.Count;
        var offset = index.GetOffset(count);

        if(offset >= 0 && offset < count) {
            return argInfo.ActualValue;
        }

        argInfo.WithReason($"The collection of length {count} didn't contain the index {index} (which would have an offset of {offset})!");
        throw new ArgumentOutOfRangeException(argInfo.ParameterName, argInfo.ActualValue, argInfo.GetLongMessage());
    }

    #endregion

    #endregion
}

internal interface IArgInfo {
    object? ValueAsObject { get; }
}

public record ArgInfo<T>(T ActualValue, string ParameterName, string MethodName, ISet<string>? Reasons = default) : IArgInfo {
    public        object?      ValueAsObject => ActualValue;
    private const string       Icon          = "🚮";
    private const string       ReasonIcon    = "🙅";
    private const string       DefaultReason = "<no reason 🤷>";
    private       ISet<string> Reasons { get; } = Reasons ?? new HashSet<string>();

    public string Preamble => $"{Icon} {MethodString}({ParamString})";

    public static ArgInfo<T> For(T actualValue, string parameterName, string methodName) {
        return new ArgInfo<T>(actualValue, parameterName, methodName);
    }

    private string ParamString =>
        typeof(T).Name.Equals(ParameterName, StringComparison.OrdinalIgnoreCase)
            ? $"[{ParameterName}]"
            : $"[{typeof(T).Name}]{ParameterName}";

    private string MethodString => MethodName.EnsureEndingPattern(new Regex("(.*)"), "()", 1);

    public ArgInfo<T> WithReason(string reason) {
        return WithReason(new string[] { reason });
    }

    public ArgInfo<T> WithReason(params string[] reasons) {
        Reasons.UnionWith(reasons);
        return this;
    }

    public string GetLongMessage() {
        return $"{Preamble} {ReasonString}";
    }

    public string GetShortMessage() {
        return $"{Icon} {ParamString} {ReasonString}";
    }

    private string ReasonString =>
        Reasons switch {
            { Count: 0 } => DefaultReason,
            { Count: 1 } => $"{ReasonIcon} {Reasons.Single()}",
            _            => $"{ReasonIcon} {Reasons.Select(it => $"[{it}]").JoinString(" ")}",
        };

    public ArgumentException GetException(string reason) {
        return new ArgumentException(WithReason(reason).GetLongMessage(), ParameterName);
    }

    public ArgumentException GetException() {
        return new ArgumentException(GetLongMessage(), ParameterName);
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
        return new ArgumentNullException(argInfo.ParameterName, argInfo.Preamble);
    }

    public static ArgumentException WasNotNull<T>(this ArgInfo<T> argInfo) {
        return argInfo.WithReason(nameof(WasNotNull)).GetException();
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
        return new FileNotFoundException(argInfo.WithReason(new[] { nameof(DidNotExist) }).GetLongMessage());
    }

    public static IOException AlreadyExisted<T>(this ArgInfo<T> argInfo)
        where T : FileSystemInfo? {
        return new IOException(argInfo.WithReason(new[] { nameof(AlreadyExisted) }).GetLongMessage());
    }

    #region WasEmpty

    public static IOException HadLengthZero(this ArgInfo<FileInfo?> argInfo) {
        return new IOException(argInfo.WithReason(new[] { $"Had a {nameof(FileInfo)}.{nameof(FileInfo.Length)}" }).GetLongMessage());
    }

    public static IOException HadNoContents(this ArgInfo<DirectoryInfo?> argInfo) {
        return new IOException(argInfo.WithReason(new[] { $"{argInfo.ActualValue?.GetType().Name ?? nameof(DirectoryInfo)} didn't contain any files or subdirectories!" }).GetLongMessage());
    }

    #endregion

    #region WasNotEmpty

    public static IOException WasNotEmpty(this ArgInfo<FileInfo> argInfo) {
        return new IOException(argInfo.WithReason(new[] { $"Existed and had a {nameof(FileInfo.Length)} > 0!" }).GetLongMessage());
    }

    public static IOException WasNotEmpty(this ArgInfo<DirectoryInfo> argInfo) {
        return new IOException(argInfo.WithReason(new[] { $"Existed and contained files or sub-directories!" }).GetLongMessage());
    }

    #endregion
    #endregion
}