using System.IO;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Clerical;

namespace FowlFever.BSharp.Exceptions;

/// <remarks>
/// 📎 Don't call <see cref="FileSystemInfo.Refresh"/> every time you access the files:
///
/// <code><![CDATA[
///|       Method |          Mean |         Error |        StdDev |
///|------------- |--------------:|--------------:|--------------:|
///|    JustExist |      2.011 ns |     0.0485 ns |     0.0663 ns |
///| RefreshFirst | 27,044.709 ns | 1,431.9845 ns | 4,177.1660 ns |
/// ]]></code>
/// </remarks>
public static partial class Must {
    #region Files

    #region Exist

    public static T Exist<T>(
        [NotNull] T fileSystemInfo,
        [CallerArgumentExpression("fileSystemInfo")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : FileSystemInfo {
        return Be(
            fileSystemInfo,
            it => it?.Refreshed().Exists == true,
            parameterName,
            methodName,
            "must exist"
        );
    }

    #region ExistWithContent

    public static FileInfo ExistWithContent(
        FileInfo? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return Be(
            actualValue,
            it => it?.ExistsWithContent() == true,
            parameterName,
            methodName,
            "must exist AND have content"
        );
    }

    #endregion

    #endregion

    #region NotBeEmpty

    public static FileInfo NotBeEmpty(
        FileInfo? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return ExistWithContent(actualValue, parameterName, methodName);
    }

    public static DirectoryInfo NotBeEmpty(
        DirectoryInfo? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return Be(
            actualValue,
            it => it?.IsNotEmpty() == true,
            parameterName,
            methodName
        );
    }

    #endregion

    #region BeEmptyOrMissing

    public static FileInfo BeEmptyOrMissing(
        FileInfo actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return Be(
            actualValue,
            FileInfoExtensions.IsEmptyOrMissing,
            parameterName,
            methodName
        );
    }

    public static DirectoryInfo BeEmptyOrMissing(
        DirectoryInfo actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return Be(actualValue, DirectoryInfoExtensions.IsEmptyOrMissing, parameterName, methodName);
    }

    #endregion

    #region NotExist

    public static T NotExist<T>(
        T fileSystemInfo,
        [CallerArgumentExpression("fileSystemInfo")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : FileSystemInfo {
        return Be(
            fileSystemInfo,
            it => it?.Exists == true,
            parameterName,
            methodName
        );
    }

    #endregion

    #endregion
}